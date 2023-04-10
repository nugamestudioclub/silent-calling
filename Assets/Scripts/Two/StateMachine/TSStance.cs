using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TSStance : TwoBaseState
{
    SpearControlBehavior spearControlBehavior;

    public TSStance(CharacterController c, Transform cam, Action<TwoState> a, SpearControlBehavior spearControlBehavior) : base(c, cam, a) 
    {
        this.spearControlBehavior = spearControlBehavior;
    }

    public override void UpdateState(TwoBaseState b)
    {
        // not important because we dont move.
        // base.UpdateState(b);

        spearControlBehavior.ToggleStance(true);
    }

    public override void HandleStanceInput(InputAction.CallbackContext c)
    {
        // q = spear spin
        // e = burst

        if (c.started)
        {
            spearControlBehavior.DoBehaviorWithValue(c.ReadValue<float>());
        }
    }

    public override void Handle2DMovement(InputAction.CallbackContext c)
    {
        // move spear
    }

    void ExitToIdle()
    {
        spearControlBehavior.ToggleStance(false);

        ChangeState(TwoState.Idle);
    }

    public override void HandleButton1(InputAction.CallbackContext c)
    {
        if (c.started)
        {
            ExitToIdle();
        }
    }

    public override void HandleButton2(InputAction.CallbackContext c)
    {
        if (c.started)
        {
            ExitToIdle();
        }
    }

    public override void HandleButton3(InputAction.CallbackContext c)
    {
        if (c.started)
        {
            ExitToIdle();
        }
    }

    #region Unimportant Overrides
    public override void PhysicsProcess()
    {
        // pass
        // i might need this to be defined in the case you get shoved off of a thing in stance, but eh
    }

    protected override void ProcessMovement()
    {
        // pass. We never move in this stance.
    }

    public override void StateStart()
    {
        // pass
    }

    protected override void StateEnd()
    {
        // pass
    }

    #endregion
}
