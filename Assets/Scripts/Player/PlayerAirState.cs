using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player _player, PlayerStateMachine _playerStateMachine, string _animBoolName) : base(_player, _playerStateMachine, _animBoolName)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();

        player.playerAirBaseInstance.DoAnimationTriggerEventLogic();
    }

    public override void EnterState()
    {
        base.EnterState();

        player.playerAirBaseInstance.DoEnterLogic();
    }

    public override void ExitState()
    {
        base.ExitState();

        player.playerAirBaseInstance.DoExitLogic();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        player.playerAirBaseInstance.DoFrameUpdateLogic();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.playerAirBaseInstance?.DoPhysicsUpdateLogic();
    }
}
