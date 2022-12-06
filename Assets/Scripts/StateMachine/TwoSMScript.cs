using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// StateMachine for Two
public class TwoSMScript : PossessableObject
{
    private TwoBaseState prior_state;
    private TwoBaseState current_state;

    public delegate void StateChange(TwoBaseState state);
    public StateChange OnStateChanged;

    public void ChangeState(TwoBaseState next)
    {
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // remember to use Time.deltaTime for unscaled movement
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

public abstract class TwoBaseState
{
    protected abstract void StateStart();
    protected abstract void StateEnd();

    public abstract void Handle2DMovement(InputAction.CallbackContext c);
    public abstract void HandleButton1(InputAction.CallbackContext c);
    public abstract void HandleButton2(InputAction.CallbackContext c);
    public abstract void HandleButton3(InputAction.CallbackContext c);
    public abstract void InUpdate();
}
