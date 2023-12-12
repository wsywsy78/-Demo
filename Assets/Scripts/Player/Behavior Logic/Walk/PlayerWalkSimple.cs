using UnityEngine;

[CreateAssetMenu(fileName = "WalkSimple", menuName = "Player/Walk/Walk Simple")]
public class PlayerWalkSimple : PlayerWalkSOBase
{
    /// <summary>
    /// 移动速度
    /// </summary>
    [SerializeField] private float moveSpeed = 2f;
    /// <summary>
    /// x轴（竖直）的移动输入
    /// </summary>
    private float xMove;
    /// <summary>
    /// y轴（水平）的移动输入
    /// </summary>
    private float yMove;

    public override void DoAnimationTriggerEventLogic()
    {
        base.DoAnimationTriggerEventLogic();
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();
        if (player.playerMove == new Vector2(0, 0))
        {
            player.stateMachine.ChangeState(player.playerIdleState);
            return;
        }
        if (!playerIsGround.IsGround())
        {
            player.stateMachine.ChangeState(player.playerAirState);
            return;
        }
        //检查玩家的是否在平台上，如果是并且玩家没有向上的速度，转换为特殊的平台状态
        if (playerIsGround.isOnPlatform && m_playerController.velocity.y < 0.1f)
        {
            player.stateMachine.ChangeState(player.playerFollowState);
            return;
        }
        //xMove = Mathf.Clamp(player.playerMove.x, -1, 1);
        //yMove = Mathf.Clamp(player.playerMove.y, -1, 1);
        //player.anim.SetFloat("xMove", xMove);
        //player.anim.SetFloat("yMove", yMove);
    }

    public override void DoPhysicsUpdateLogic()
    {
        //单位时间移动速度矢量
        motionVector = transform.right * player.playerMove.x + transform.forward * player.playerMove.y;

        //将玩家移到相应位置
        motionVector *= moveSpeed;

        if (playerIsGround.IsGround())
            applyVector.y = -5f;


        base.DoPhysicsUpdateLogic();
        CheckForExceptionVelocity();
        //Debug.Log($"vec2:{m_playerController.velocity}");
        //if (m_playerController.velocity.sqrMagnitude > 12)
        //    Debug.Log($"mo:{motionVector},apply:{applyVector},keep:{maintainMomentum}");
    }

    public override void Initialize(GameObject _gameObject, Player _player)
    {
        base.Initialize(_gameObject, _player);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }

    protected override void RecordVelocity(Vector3 _motionVec, Vector3 _applyVec, Vector3 _maintainMomentum)
    {
        _applyVec.y = m_playerController.velocity.y;
        base.RecordVelocity(_motionVec, _applyVec, _maintainMomentum);
    }
}
