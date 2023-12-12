using UnityEngine;

[CreateAssetMenu(fileName = "IdleSimple", menuName = "Player/Idle/Idle Simple")]
public class PlayerIdleSimple : PlayerIdleSOBase
{
    /// <summary>
    /// ��ת��ΪIdle״̬ʱ���ٶȱ��ֵ�ʱ��
    /// </summary>
    [SerializeField] private float maxKeepTimer;
    /// <summary>
    /// ��ʱ��
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
        //�����⵽�ƶ����룬�л�������״̬
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
        //�����ҵ��Ƿ���ƽ̨�ϣ�����ǲ������û�����ϵ��ٶȣ�ת��Ϊ�����ƽ̨״̬
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

        //�����Ҳ��ٵ����ϣ������ʩ������
        if (!playerIsGround.IsGround() || isGroundPortal)
        {
            //�������ڱ�ʩ�ӵ��������Ե���applyVector
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
