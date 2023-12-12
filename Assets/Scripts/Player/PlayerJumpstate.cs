public class PlayerJumpstate : PlayerState
{
    public PlayerJumpstate(Player _player, PlayerStateMachine _playerStateMachine, string _animBoolName) : base(_player, _playerStateMachine, _animBoolName)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();

        player.playerJumpBaseInstance.DoAnimationTriggerEventLogic();
    }

    public override void EnterState()
    {
        base.EnterState();

        player.playerJumpBaseInstance.DoEnterLogic();
    }

    public override void ExitState()
    {
        base.ExitState();

        player.playerJumpBaseInstance.DoExitLogic();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        player.playerJumpBaseInstance.DoFrameUpdateLogic();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.playerJumpBaseInstance.DoPhysicsUpdateLogic();
    }
}
