using UnityEngine;

[CreateAssetMenu(fileName = "IdleSimple", menuName = "Player/Idle/Idle Simple")]
public class PlayerIdleSimple : PlayerIdleSOBase
{
    /// <summary>
    /// 当转换为Idle状态时，速度保持的时间
    /// </summary>
    [SerializeField] private float maxKeepTimer;
    /// <summary>
    /// 计时器
    /// </summary>
    private float timer;
    public override void DoAnimationTriggerEventLogic()
    {
        base.DoAnimationTriggerEventLogic();
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        applyVector = GetPhysicsProperty.prevFrameMotionVec + GetPhysicsProperty.prevFrameApplyVec;
        timer = maxKeepTimer;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();
        //如果检测到移动输入，切换至行走状态
        if (player.playerMove != new Vector2(0, 0))
        {
            player.stateMachine.ChangeState(player.playerWalkState);
            return;
        }
        if (!playerIsGround.IsGround())
        {
            player.stateMachine.ChangeState(player.playerAirState);
            return;
        }
        //检查玩家的是否在平台上，如果是并且玩家没有向上的速度，转换为特殊的平台状态
        if (playerIsGround.isOnPlatform && Mathf.Abs(m_playerController.velocity.y) < 0.1f)
        {
            player.stateMachine.ChangeState(player.playerFollowState);
            return;
        }

        timer -= Time.deltaTime;
    }

    public override void DoPhysicsUpdateLogic()
    {
        if (timer < 0)
            applyVector = Vector3.zero;

        //如果玩家不再地面上，则对其施加重力
        if (!playerIsGround.IsGround() || isGroundPortal)
        {
            //重力属于被施加的力，所以调整applyVector
            applyVector.y += virtualGravity * Time.deltaTime;
        }
        base.DoPhysicsUpdateLogic();
    }

    public override void Initialize(GameObject _gameObject, Player _player)
    {
        base.Initialize(_gameObject, _player);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
}
