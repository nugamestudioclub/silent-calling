using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;


public enum TwoState
{
    Idle,
    Move,
    Rising,
    Falling
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
        CharacterController c = GetComponent<CharacterController>();
        TwoCameraScript tcs = Camera.main.GetComponent<TwoCameraScript>();

        // use a HashMap to store all the states so I can avoid an icky Factory Pattern
        // seriously what is so great about those, I don't get it.
        //
        // I use to have a StateLibrary that was functionally similar, but this method just cuts
        // out the middleman. So much better.
        map = new Dictionary<TwoState, TwoBaseState>();
        map.Add(TwoState.Idle, new TSIdle(c, tcs.transform, ChangeStateFunc));
        map.Add(TwoState.Move, new TSMove(c, tcs.transform, ChangeStateFunc));
        map.Add(TwoState.Falling, new TSFalling(c, tcs.transform, ChangeStateFunc));
        map.Add(TwoState.Rising, new TSRising(c, tcs.transform, ChangeStateFunc));

        // Initial value is Idle, because obviously.
        current_state = map[TwoState.Idle];
    }

    private void Start()
    {
        PersistentHook();
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
        current_state.InUpdate();
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

    // do i even need to have this?
    // it might be nice to have an extra button for something
    // maybe the UI will hook into the same UnityEvent that calls this key?
    private void WithBackButton(InputAction.CallbackContext context)
    {
        current_state.HandleButton3(context);
    }
    #endregion

    #region Input Nexus Hooks
    protected override void Free()
    {
        INS.LateralBind -= WithInputVector;
        INS.ButtonBind -= WithButton;
        INS.UseBind -= WithUseButton;
        INS.BackBind -= WithBackButton;
    }

    protected override void FreePersistentHook()
    {
        INS.EmptyBind -= Hook;
    }

    protected override void Hook()
    {
        INS.LateralBind += WithInputVector;
        INS.ButtonBind += WithButton;
        INS.UseBind += WithUseButton;
        INS.BackBind += WithBackButton;
    }

    protected override void PersistentHook()
    {
        INS.EmptyBind += Hook;
    }

    #endregion
}

// naming convention:
// TS(State Name), stands for Two State (state name)
// e.g. TSIdle, TSMove, TSAirborne
// Two's base State for his StateMachine. This is a template class.
public abstract class TwoBaseState
{
    protected const float _GRAVITY = -9.81f; // bruh
    protected const float _MOVE_SPEED = 5f; // bruh
    protected const float _JUMP_FORCE = 10f; // bruh
    protected const float _FALL_MULTIPLIER = 4f; // how much faster the fall should be than the rising portion of a jump
    protected const float _DECEL = 0.04f; // how fast the rise flattens out
    protected const float _MAX_FALL_SPEED = -45f; // bruh
    protected const float _COYOTE_TIME = 30f; // how long can we not be on something and still be able to jump?

    protected CharacterController _cc;
    protected Transform _cameraTransform;
    protected Action<TwoState> _ChangeStateAction;

    protected Vector2 _currentInput; // bruh
    protected float _yvelo = 0f; // stores the current yvelo bc CharacterController can't do that
    protected float slopeLimit; // internally stores the CharacterController slope limit so i dont have to do _cc.slopeLimit each time
    protected RaycastHit r; // stores Raycast info that Idle and Move need
    protected Transform moving_body; // both Idle and Move need to keep track of what moving object they are on
    protected bool onMovingPlatform; // both Idle and Move needed this, so I put it here.

    public TwoBaseState(CharacterController c, Transform cam, Action<TwoState> a)
    {
        _cc = c;
        _cameraTransform = cam;
        _ChangeStateAction = a;

        StateStart();
    }

    // called when a State needs to change to another State.
    // invokes the SMScript's ChangeStateFunc which actually does the work.
    protected virtual void ChangeState(TwoState next)
    {
        StateEnd();

        _ChangeStateAction.Invoke(next);
    }

    // cast a SphereRay (ooh so fancy) downwards to get information.
    protected bool CastRay()
    {
        Ray ray = new Ray(_cc.transform.position, Vector3.down); // our downwards Ray

        return Physics.SphereCast(ray, _cc.radius, out r, 2f);

        //return Physics.Raycast(ray, out r, 2f); Old and outdated, but keep just in case
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

        v.x *= _MOVE_SPEED;
        v.y = _yvelo; // assign the yvelo
        v.z *= _MOVE_SPEED;

        _cc.Move(v * Time.deltaTime);

        v.y = 0f; // we only want a rotation in the XZ plane

        if (v != Vector3.zero) // cant have a zero rotation Quaternion
        {
            _cc.transform.rotation = Quaternion.LookRotation(v);
        }
    }

    // Abstracts
    public abstract void Handle2DMovement(InputAction.CallbackContext c);
    public abstract void HandleButton1(InputAction.CallbackContext c);
    public abstract void HandleButton2(InputAction.CallbackContext c);
    public abstract void HandleButton3(InputAction.CallbackContext c);
    public abstract void InUpdate();
    public abstract void StateStart();
    protected abstract void StateEnd();
}
