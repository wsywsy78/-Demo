using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FollowSimple", menuName = "Player/Follow/Follow Simple")]
public class PlayerFollowSimple : PlayerFollowSOBase
{
    /// <summary>
    /// 平台移动速度
    /// </summary>
    private Vector3 platformVelocity;
    /// <summary>
    /// 移动速度
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
        //由于平台是每一帧移动而不是每个物理帧移动的，要保持在平台上移动的流畅性，需要在Update中进行跟随平台移动
        m_playerController.Move(new Vector3(0, -3f, 0) * Time.deltaTime);
        m_playerController.Move(playerIsGround.platformVelocity * Time.deltaTime);
        if (!playerIsGround.isOnPlatform)
        {
            player.stateMachine.ChangeState(player.playerAirState);
        }
    }

    public override void DoPhysicsUpdateLogic()
    {
        //单位时间移动速度矢量
        motionVector = transform.right * player.playerMove.x + transform.forward * player.playerMove.y;
        motionVector *= moveSpeed;

        //将平台的移动速度赋给applyVector
        //applyVector = playerIsGroundInstance.platformVelocity;
        //Debug.Log($"player:{applyVector}");
        base.DoPhysicsUpdateLogic();
    }

    public override void Initialize(GameObject _gameObject, Player _player)
    {
        base.Initialize(_gameObject, _player);
    }
}
