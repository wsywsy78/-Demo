using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FollowSimple", menuName = "Player/Follow/Follow Simple")]
public class PlayerFollowSimple : PlayerFollowSOBase
{
    /// <summary>
    /// ƽ̨�ƶ��ٶ�
    /// </summary>
    private Vector3 platformVelocity;
    /// <summary>
    /// �ƶ��ٶ�
    /// </summary>
    [SerializeField] private float moveSpeed = 3f;

    public override void DoAnimationTriggerEventLogic()
    {
        base.DoAnimationTriggerEventLogic();
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        applyVector = Vector3.zero;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        platformVelocity = PlayerIsGround.Instance.platformVelocity;

        //if (playerIsGround)
        //    Debug.Log($"isGroundOnPlatform");
        //����ƽ̨��ÿһ֡�ƶ�������ÿ������֡�ƶ��ģ�Ҫ������ƽ̨���ƶ��������ԣ���Ҫ��Update�н��и���ƽ̨�ƶ�
        m_playerController.Move(new Vector3(0, -3f, 0) * Time.deltaTime);
        m_playerController.Move(playerIsGround.platformVelocity * Time.deltaTime);
        if (!playerIsGround.isOnPlatform)
        {
            player.stateMachine.ChangeState(player.playerAirState);
        }
    }

    public override void DoPhysicsUpdateLogic()
    {
        //��λʱ���ƶ��ٶ�ʸ��
        motionVector = transform.right * player.playerMove.x + transform.forward * player.playerMove.y;
        motionVector *= moveSpeed;

        //��ƽ̨���ƶ��ٶȸ���applyVector
        //applyVector = playerIsGroundInstance.platformVelocity;
        //Debug.Log($"player:{applyVector}");
        base.DoPhysicsUpdateLogic();
    }

    public override void Initialize(GameObject _gameObject, Player _player)
    {
        base.Initialize(_gameObject, _player);
    }
}
