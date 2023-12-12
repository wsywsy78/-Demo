public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _playerStateMachine, string _animBoolName) : base(_player, _playerStateMachine, _animBoolName)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();

        player.playerIdleBaseInstance.DoAnimationTriggerEventLogic();
    }

    public override void EnterState()
    {
        base.EnterState();

        player.playerIdleBaseInstance.DoEnterLogic();
    }

    public override void ExitState()
    {
        base.ExitState();

        player.playerIdleBaseInstance.DoExitLogic();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        player.playerIdleBaseInstance.DoFrameUpdateLogic();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.playerIdleBaseInstance.DoPhysicsUpdateLogic();
    }
}
