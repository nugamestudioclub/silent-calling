using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public enum TwoState
{
    Idle,
    Move,
    Rising,
    Falling,
    Running,
    Stance
}

// StateMachine for Two
public class TwoSMScript : PossessableObject
{
    private TwoBaseState prior_state; // not used now, but might be important later
    private TwoBaseState current_state; // stores our current player state

    public delegate void StateChange(TwoBaseState state);
    public event StateChange OnStateChanged; // fires when the state is changed, with the State that was changed to
                                             // soon to link up with the Animation system.

    Dictionary<TwoState, TwoBaseState> map; // see below

    void Awake()
    {
        GameObject spear = GameObject.FindGameObjectWithTag("TwoSpear");

        CharacterController c = GetComponent<CharacterController>();
        TwoCameraScript tcs = Camera.main.GetComponent<TwoCameraScript>();
        CharacterControllerForceScript ccfs = GetComponent<CharacterControllerForceScript>();
        SpearControlBehavior scb = spear.GetComponent<SpearControlBehavior>();

        // use a HashMap to store all the states so I can avoid an icky Factory Pattern
        // seriously what is so great about those, I don't get it.
        //
        // I use to have a StateLibrary that was functionally similar, but this method just cuts
        // out the middleman. So much better.
        map = new Dictionary<TwoState, TwoBaseState>();
        map.Add(TwoState.Idle, new TSIdle(c, tcs.transform, ChangeStateFunc));
        map.Add(TwoState.Move, new TSMove(c, tcs.transform, ChangeStateFunc));
        map.Add(TwoState.Falling, new TSFalling(c, tcs.transform, ChangeStateFunc, ccfs.AddImpact));
        map.Add(TwoState.Rising, new TSRising(c, tcs.transform, ChangeStateFunc, ccfs.AddImpact));
        map.Add(TwoState.Running, new TSRun(c, tcs.transform, ChangeStateFunc));
        map.Add(TwoState.Stance, new TSStance(c, tcs.transform, ChangeStateFunc, scb));

        // Initial value is Idle, because obviously.
        current_state = map[TwoState.Idle];
    }

    /// <summary>
    /// Takes in a TwoState enum and changes the state to that State.
    /// Fires the OnStateChanged event and Updates + Starts the next state.
    /// </summary>
    /// <param name="n"></param>
    public void ChangeStateFunc(TwoState n)
    {
        TwoBaseState next = map[n];

        prior_state = current_state;

        OnStateChanged?.Invoke(next);

        current_state = next;

        Debug.Log("State: " + next.StateType);

        current_state.UpdateState(prior_state);

        current_state.StateStart();
    }

    /// <summary>
    /// Unused currently, but returns to the prior state, essentially swapping their values.
    /// Identical to ChangeStateFunc otherwise.
    /// </summary>
    public void RegressState()
    {
        TwoBaseState cache = current_state;

        current_state = prior_state;

        OnStateChanged.Invoke(prior_state);

        current_state.UpdateState(prior_state);

        current_state.StateStart();

        prior_state = cache;
    }

    #region Callback Hooks
    // All these functions just hook the current state's Handles into the callbacks.
    void Update()
    {
        current_state.PhysicsProcess();
    }

    private void WithInputVector(InputAction.CallbackContext context)
    {
        current_state.Handle2DMovement(context);
    }

    private void WithButton(InputAction.CallbackContext context)
    {
        current_state.HandleButton1(context);
    }

    private void WithUseButton(InputAction.CallbackContext context)
    {
        current_state.HandleButton2(context);
    }

    private void WithBackButton(InputAction.CallbackContext context)
    {
        current_state.HandleButton3(context);
    }

    private void WithExtraButton(InputAction.CallbackContext context)
    {
        current_state.HandleStanceInput(context);
    }

    #endregion

    #region Input Nexus Hooks
    protected override void Free()
    {

        #if UNITY_EDITOR
        if (INS == null)
        {
            Debug.LogWarning("If you see this message when not exiting playmode, then you are missing an INS!");

            return;
        }

        Debug.Log(string.Format("Links for {0} were decoupled w/o race condition error.", name));
        #endif

        INS.LateralBind -= WithInputVector;
        INS.ButtonBind -= WithButton;
        INS.UseBind -= WithUseButton;
        INS.BackBind -= WithBackButton;
        INS.ExtraBind -= WithExtraButton;
    }

    protected override void Hook()
    {
        INS.LateralBind += WithInputVector;
        INS.ButtonBind += WithButton;
        INS.UseBind += WithUseButton;
        INS.BackBind += WithBackButton;
        INS.ExtraBind += WithExtraButton;
    }

    #endregion
}

// naming convention:
// TS(State Name), stands for Two State (state name)
// e.g. TSIdle, TSMove, TSAirborne
// Two's base State for her StateMachine. This is a template class.
public abstract class TwoBaseState
{
    protected const float _GRAVITY = -9.81f; // bruh
    protected const int _LAYER_MASK = ~(1 << 2); // haha see TwoCameraScript hehe
    protected const float _MOVE_SPEED = 5f; // bruh
    protected const float _RUN_SPEED_MULTIPLIER = 3f; // bruh
    protected const float _JUMP_FORCE = 15f; // bruh
    protected const float _FALL_MULTIPLIER = 4f; // how much faster the fall should be than the rising portion of a jump
    protected const float _DECEL = 0.04f; // how fast the rise flattens out
    protected const float _MAX_FALL_SPEED = -45f; // bruh
    protected const float _COYOTE_TIME = 10f; // how long can we not be on something and still be able to jump?
    protected const float _ROTATION_LERP = 0.05f * 150f; // how quickly does the player smoothly rotate?
                                                         // this is affected by deltaTime, so it needs to be big.
    protected const float _WALLKICK_FORCE = 10f;
    protected const float _AIRJUMP_DRAG = 2.75f;

    protected CharacterController _cc;
    protected Transform _cameraTransform;
    protected Action<TwoState> _ChangeStateAction;

    protected Vector2 _currentInput; // bruh
    protected float _yvelo = 0f; // stores the current yvelo bc CharacterController can't do that
    protected float _slopeLimit; // internally stores the CharacterController slope limit so i dont have to do _cc.slopeLimit each time
    protected RaycastHit _raycastInfo; // stores Raycast info that Idle and Move need
    protected Transform _movingBody; // both Idle and Move need to keep track of what moving object they are on
    protected bool _onMovingPlatform; // both Idle and Move needed this, so I put it here.

    public TwoState StateType; // used for the Animator hook.
    public TwoState PastState;

    protected bool _running = false; // to be removed when refactored

    public TwoBaseState(CharacterController c, Transform cam, Action<TwoState> a)
    {
        _cc = c;
        _cameraTransform = cam;
        _ChangeStateAction = a;

        _slopeLimit = _cc.slopeLimit;

        StateStart();
    }

    // called when a State needs to change to another State.
    // invokes the SMScript's ChangeStateFunc which actually does the work.
    protected virtual void ChangeState(TwoState next)
    {
        StateEnd();

        PastState = StateType;

        _ChangeStateAction.Invoke(next);
    }

    // cast a SphereRay (ooh so fancy) downwards to get information.
    // > not so fancy anymore, sphereRay is whack
    protected bool CastRay()
    {
        Ray ray = new Ray(_cc.transform.position, Vector3.down); // our downwards Ray

        //return Physics.SphereCast(ray, _cc.radius, out _raycastInfo, 2f); // this is the weirdest thing

        return Physics.Raycast(ray, out _raycastInfo, 2f, _LAYER_MASK);
    }

    // returns true if we are on a slope with an angle greater than the slope limit.
    protected bool OnIllegalSlope()
    {
        return !(Vector3.Angle(Vector3.up, _raycastInfo.normal) < _slopeLimit);
    }

    // public getters for the UpdateState function only
    public Vector3 GetInputVector()
    {
        return _currentInput;
    }
    public float GetYVelo()
    {
        return _yvelo;
    }
    public bool IsRunning()
    {
        return _running;
    }

    /// <summary>
    /// Given the previous state, updates the current yvelo and input to those
    /// of the previous state. This ensures that no data is lost between transitions.
    /// </summary>
    /// <param name="b"></param>
    public virtual void UpdateState(TwoBaseState b)
    {
        _yvelo = b.GetYVelo();
        _currentInput = b.GetInputVector();
    }

    // TODO
    // coalesce all the changes made to ProcessMovement into one so we don't have these
    // weird differing definitions between Move/Idle and Rising/Falling.

    /// <summary>
    /// Applies the current movement input vector to the character, multiplied by MOVESPEED.
    /// Y velocity is determined by the current yvelo.
    /// This definition isn't completely functionally correct as TSMove has the better
    /// version. However, that version is overcomplicating what other classes need.
    /// </summary>
    protected virtual void ProcessMovement()
    {
        Vector3 v = new Vector3(_currentInput.x, 0, _currentInput.y);
        v = _cameraTransform.TransformDirection(v); // align correctly with camera view

        float val = _running ? _RUN_SPEED_MULTIPLIER * _MOVE_SPEED: _MOVE_SPEED;

        v.x *= val;
        v.y = _yvelo; // assign the yvelo
        v.z *= val;

        _cc.Move(v * Time.deltaTime);//(v * Time.deltaTime);

        v.y = 0f; // we only want a rotation in the XZ plane

        if (v != Vector3.zero) // cant have a zero rotation Quaternion
        {
            // unsmoothened variant
            // _cc.transform.rotation = Quaternion.LookRotation(v); // look in the direction of movement

            // smoothened
            // look in the direction of movement, but smoothly. Can result in intermittent angles if control
            // is released before the Lerp completes
            _cc.transform.rotation = Quaternion.Lerp(
                _cc.transform.rotation,
                Quaternion.LookRotation(v),
                _ROTATION_LERP * Time.deltaTime);
        }
    }

    public virtual void HandleStanceInput(InputAction.CallbackContext c)
    {
        if (c.started)
        {
            ChangeState(TwoState.Stance);
        }
    }

    // Abstracts
    public abstract void Handle2DMovement(InputAction.CallbackContext c);
    public abstract void HandleButton1(InputAction.CallbackContext c);
    public abstract void HandleButton2(InputAction.CallbackContext c);
    public abstract void HandleButton3(InputAction.CallbackContext c);
    public abstract void PhysicsProcess();
    public abstract void StateStart();
    protected abstract void StateEnd();
}
