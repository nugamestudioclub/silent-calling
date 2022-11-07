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

    }

    protected override void Free()
    {
        throw new System.NotImplementedException();
    }

    protected override void FreePersistentHook()
    {
        throw new System.NotImplementedException();
    }

    protected override void Hook()
    {
        throw new System.NotImplementedException();
    }

    protected override void PersistentHook()
    {
        throw new System.NotImplementedException();
    }
}

public abstract class TwoBaseState
{
    public abstract void Handle2DMovement(InputAction.CallbackContext c);
    public abstract void HandleButton1(InputAction.CallbackContext c);
    public abstract void HandleButton2(InputAction.CallbackContext c);
    public abstract void HandleButton3(InputAction.CallbackContext c);
}
