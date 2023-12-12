using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AirSimple", menuName = "Player/Air/Air Simple")]
public class PlayerAirSimple : PlayerAirSOBase
{
    /// <summary>
    /// 跳跃的高度
    /// </summary>
    [SerializeField] private float jumpHeight = 1;
    /// <summary>
    /// 在掉落时的同样可以进行横向输入，这是影响系数
    /// </summary>
    [SerializeField] private float moveEffect = 3f;
    public override void DoAnimationTriggerEventLogic()
    {
        base.DoAnimationTriggerEventLogic();
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        if (GetPhysicsProperty.isJump)
        {
            //如果是跳跃的话，给予垂直速度
            applyVector.y = Mathf.Sqrt(-2f * jumpHeight * virtualGravity);
        }
        else
        {
            //将垂直速度设为0
            applyVector.y = 0;
        }
        //继承玩家跳跃时的水平速度
        //applyVector.x = m_playerController.velocity.x;
        //applyVector.z = m_playerController.velocity.z;
        //注：一开始直接用CharacterController.velocity获取速度，但是在传送前后，velocity会出现异常增大，导致传送后“冲”出去
        //查阅资料无果，推测是传送涉及到物体引擎的内容，故这里的速度直接用前一帧的applyVector来赋值
        applyVector.x = GetPhysicsProperty.prevFrameApplyVec.x + GetPhysicsProperty.prevFrameMotionVec.x;
        applyVector.z = GetPhysicsProperty.prevFrameApplyVec.z + GetPhysicsProperty.prevFrameMotionVec.z;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        maintainMomentum = Vector3.zero;
        GetPhysicsProperty.isJump = false;
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();
        if (playerIsGround.isOnPlatform && m_playerController.velocity.y < 0.5f)
        {
            player.stateMachine.ChangeState(player.playerFollowState);
            return;
        }
    }

    public override void DoPhysicsUpdateLogic()
    {
        motionVector = (transform.right * player.playerMove.x + transform.forward * player.playerMove.y) * moveEffect;

        //如果玩家在空中时已经有水平的初速度，当输入的速度与水平速度方向相反，则会降低玩家水平方向的速度，
        //如果输入速度与初速度方向一致，则忽略输入速度，不能让玩家突破限度跳的更远
        if ((applyVector.x < 0 && motionVector.x > 0) || (applyVector.x > 0 && motionVector.x < 0))
        {
            applyVector.x = applyVector.x * 0.85f;    
        }
        else if((applyVector.x > 0 && motionVector.x > 0) || (applyVector.x < 0 && motionVector.x < 0))
        {
            applyVector.x = motionVector.x;
            motionVector.x = 0;            
        }
        if ((applyVector.z < 0 && motionVector.z > 0) || (applyVector.z > 0 && motionVector.z < 0))
        {
            applyVector.z = applyVector.z * 0.85f;
        }
        else if((applyVector.z > 0 && motionVector.z > 0) || (applyVector.z < 0 && motionVector.z < 0))
        {
            applyVector.z = motionVector.z;
            motionVector.z = 0;            
        }

        //重力属于被施加的力，所以调整applyVector
        applyVector.y += virtualGravity * Time.deltaTime;

        base.DoPhysicsUpdateLogic();

        if (playerIsGround.IsGround() && Mathf.Abs(m_playerController.velocity.y) < 0.1f)
        {
            if (isGroundPortal)
                return;
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
            Debug.Log("trigger");
            maintainMomentum = info.maintainVec;
            //在将动量转化为maintainMomentum，原先的垂直速度应该归零
            applyVector = Vector3.zero;
        }
    }
}
