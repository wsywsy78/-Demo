using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AirSimple", menuName = "Player/Air/Air Simple")]
public class PlayerAirSimple : PlayerAirSOBase
{
    /// <summary>
    /// ��Ծ�ĸ߶�
    /// </summary>
    [SerializeField] private float jumpHeight = 1;
    /// <summary>
    /// �ڵ���ʱ��ͬ�����Խ��к������룬����Ӱ��ϵ��
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
            //�������Ծ�Ļ������费ֱ�ٶ�
            applyVector.y = Mathf.Sqrt(-2f * jumpHeight * virtualGravity);
        }
        else
        {
            //����ֱ�ٶ���Ϊ0
            applyVector.y = 0;
        }
        //�̳������Ծʱ��ˮƽ�ٶ�
        //applyVector.x = m_playerController.velocity.x;
        //applyVector.z = m_playerController.velocity.z;
        //ע��һ��ʼֱ����CharacterController.velocity��ȡ�ٶȣ������ڴ���ǰ��velocity������쳣���󣬵��´��ͺ󡰳塱��ȥ
        //���������޹����Ʋ��Ǵ����漰��������������ݣ���������ٶ�ֱ����ǰһ֡��applyVector����ֵ
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

        //�������ڿ���ʱ�Ѿ���ˮƽ�ĳ��ٶȣ���������ٶ���ˮƽ�ٶȷ����෴����ή�����ˮƽ������ٶȣ�
        //��������ٶ�����ٶȷ���һ�£�����������ٶȣ����������ͻ���޶����ĸ�Զ
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

        //�������ڱ�ʩ�ӵ��������Ե���applyVector
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
    /// ���ֶ���
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
            //�ڽ�����ת��ΪmaintainMomentum��ԭ�ȵĴ�ֱ�ٶ�Ӧ�ù���
            applyVector = Vector3.zero;
        }
    }
}
