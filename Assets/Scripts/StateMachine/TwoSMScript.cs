using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;


public enum TwoState
{
    Idle,
    Move
}

// StateMachine for Two
public class TwoSMScript : PossessableObject
{
    private TwoBaseState prior_state;
    private TwoBaseState current_state;

    public delegate void StateChange(TwoBaseState state);
    public StateChange OnStateChanged;

    Dictionary<TwoState, TwoBaseState> map;

    void Awake()
    {
        CharacterController c = GetComponent<CharacterController>();
        TwoCameraScript tcs = Camera.main.GetComponent<TwoCameraScript>();

        map = new Dictionary<TwoState, TwoBaseState>();
        map.Add(TwoState.Idle, new TSIdle(c, tcs.transform, ChangeState));
        map.Add(TwoState.Move, new TSMove(c, tcs.transform, ChangeState));

        current_state = map[TwoState.Idle];
    }

    private void Start()
    {
        PersistentHook();
    }

    public void ChangeState(TwoState n)
    {
        TwoBaseState next = map[n];

        prior_state = current_state;

        OnStateChanged?.Invoke(next);

        current_state = next;
    }

    public void RegressState()
    {
        TwoBaseState cache = current_state;

        current_state = prior_state;

        OnStateChanged.Invoke(prior_state);

        prior_state = cache;
    }

    // Update is called once per frame
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
}

// naming convention:
// TS(State Name), stands for Two State (state name)
// e.g. TSIdle, TSMove, TSAirborne
public abstract class TwoBaseState
{
    protected CharacterController _cc;
    protected Transform _cameraTransform;
    protected Action<TwoState> _ChangeStateAction;

    protected Vector2 _currentInput;

    public TwoBaseState(CharacterController c, Transform cam, Action<TwoState> a)
    {
        _cc = c;
        _cameraTransform = cam;
        _ChangeStateAction = a;

        StateStart();
    }

    protected virtual void ChangeState(TwoState next)
    {
        StateEnd();

        _ChangeStateAction.Invoke(next);
    }

    protected abstract void StateStart();
    protected abstract void StateEnd();

    public abstract void Handle2DMovement(InputAction.CallbackContext c);
    public abstract void HandleButton1(InputAction.CallbackContext c);
    public abstract void HandleButton2(InputAction.CallbackContext c);
    public abstract void HandleButton3(InputAction.CallbackContext c);
    public abstract void InUpdate();
}
