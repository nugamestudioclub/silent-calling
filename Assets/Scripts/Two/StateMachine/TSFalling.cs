using System;
using UnityEngine;

public class TSFalling : TSBaseAirborneState
{
    public TSFalling(CharacterController c, Transform t, Action<TwoState> a, Action<Vector3, float, bool> f) : base(c, t, a, f) 
    {
        StateType = TwoState.Falling;
    }

    // TODO: slower air movement???
    public override void PhysicsProcess()
    {
        // sets the yvelo to either Max Fall Speed, or that monstrosity.
        // lol it's not even that bad, just adding a bit of gravity + increasing it by a multiplier.
        _yvelo = Mathf.Max(_yvelo + _FALL_MULTIPLIER * _GRAVITY * Time.deltaTime, _MAX_FALL_SPEED);  // deltaTime is already done in ProcessMovement

        if (_cc.isGrounded)
        {
            // do we hit the ground idle or running? Depends on the input in the air.
            ChangeState(_currentInput == Vector2.zero ? TwoState.Idle : _running ? TwoState.Running : TwoState.Move);

            return;
        }

        ProcessMovement();
    }
}
