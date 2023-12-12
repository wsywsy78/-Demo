public class PlayerWalkState : PlayerState
{
    public PlayerWalkState(Player _player, PlayerStateMachine _playerStateMachine, string _animBoolName) : base(_player, _playerStateMachine, _animBoolName)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();

        player.playerWalkBaseInstance.DoAnimationTriggerEventLogic();
    }

    public override void EnterState()
    {
        base.EnterState();

        player.playerWalkBaseInstance.DoEnterLogic();
    }

    public override void ExitState()
    {
        base.ExitState();

        player.playerWalkBaseInstance.DoExitLogic();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        player.playerWalkBaseInstance.DoFrameUpdateLogic();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.playerWalkBaseInstance.DoPhysicsUpdateLogic();
    }
}
