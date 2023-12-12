using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FallSimple", menuName = "Player/Fall/Fall Simple")]
public class PlayerFallSimple : PlayerFallSOBase
{
    /// <summary>
    /// 在掉落时的同样可以进行横向输入，这是影响系数
    /// </summary>
    [SerializeField] private float moveEffect = 1f;

    public override void DoAnimationTriggerEventLogic()
    {
        base.DoAnimationTriggerEventLogic();
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        //将垂直速度设为0
        applyVector.y = 0;
        //继承玩家跳跃时的水平速度
        //applyVector.x = m_playerController.velocity.x;
        //applyVector.z = m_playerController.velocity.z;
        //注：一开始直接用CharacterController.velocity获取速度，但是在传送前后，velocity会出现异常增大，导致传送后“冲”出去
        //查阅资料无果，推测是传送涉及到物体引擎的内容，故这里的速度直接用前一帧的applyVector来赋值
        applyVector.x = GetPhysicsProperty.prevFrameApplyVec.x;
        applyVector.z = GetPhysicsProperty.prevFrameApplyVec.z;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();

        maintainMomentum = Vector3.zero;
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        //if (m_playerController.isGrounded)
        //if (playerIsGround)
        //{
        //    if (isGroundPortal && m_playerController.velocity.y < 0)
        //        return;
        //    Debug.Log($"Change, vec:{m_playerController.velocity.y}");
        //    //Debug.Log($"ground: {groundColliderId}, other: {PortalTeleportManager.Instance.cacheCollidersId}");
        //    player.stateMachine.ChangeState(player.playerIdleState);
        //}

        float _yVelocity = Mathf.Clamp(m_playerController.velocity.y, -1, 1);
        player.anim.SetFloat("yVelocity", _yVelocity);
    }

    public override void DoPhysicsUpdateLogic()
    {
        motionVector = (transform.right * player.playerMove.x + transform.forward * player.playerMove.y) * moveEffect;
        //if (applyVector.x > 0)
        //    applyVector.x = motionVector.x >= 0 ? applyVector.x : 0;
        //if (applyVector.z > 0)
        //    applyVector.z = motionVector.z >= 0 ? applyVector.z : 0;

        if ((applyVector.x < 0 && motionVector.x > 0) || (applyVector.x > 0 && motionVector.x < 0))
            applyVector.x = applyVector.x * 0.85f;
        else
            motionVector.x = 0;
        if ((applyVector.z < 0 && motionVector.z > 0) || (applyVector.z > 0 && motionVector.z < 0))
            applyVector.z = applyVector.z * 0.85f;
        else
            motionVector.z = 0;

        base.DoPhysicsUpdateLogic();
        //Debug.Log($"Fall: {maintainMomentum}, vec:{m_playerController.velocity}");

        if (playerIsGround.IsGround() && Mathf.Abs(m_playerController.velocity.y) < 0.1f)
        {
            if (isGroundPortal)
                return;
            Debug.Log($"Change, vec:{m_playerController.velocity.y}");
            //Debug.Log($"ground: {groundColliderId}, other: {PortalTeleportManager.Instance.cacheCollidersId}");
            player.stateMachine.ChangeState(player.playerIdleState);
        }
    }

    public override void Initialize(GameObject _gameObject, Player _player)
    {
        base.Initialize(_gameObject, _player);

        EventManager.Instance.AddListener(EventName.MaintainMomentum, MaintainMomentum);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.MaintainMomentum, MaintainMomentum);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }

    /// <summary>
    /// 保持动量
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MaintainMomentum(object sender, EventArgs e)
    {
        var info = e as EventParameter;
        if (info.maintainVec != Vector3.zero)
        {            
            maintainMomentum = info.maintainVec;
            //在将动量转化为maintainMomentum，原先的垂直速度应该归零
            applyVector = Vector3.zero;
        }
    }
}
