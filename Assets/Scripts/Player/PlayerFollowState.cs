using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowState : PlayerState
{
    public PlayerFollowState(Player _player, PlayerStateMachine _playerStateMachine, string _animBoolName) : base(_player, _playerStateMachine, _animBoolName)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();

        player.playerFollowBaseInstance.DoAnimationTriggerEventLogic();
    }

    public override void EnterState()
    {
        base.EnterState();

        player.playerFollowBaseInstance.DoEnterLogic();
    }

    public override void ExitState()
    {
        base.ExitState();

        player.playerFollowBaseInstance.DoExitLogic();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        player.playerFollowBaseInstance.DoFrameUpdateLogic();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.playerFollowBaseInstance.DoPhysicsUpdateLogic();
    }
}
