using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FallSimple", menuName = "Player/Fall/Fall Simple")]
public class PlayerFallSimple : PlayerFallSOBase
{
    /// <summary>
    /// �ڵ���ʱ��ͬ�����Խ��к������룬����Ӱ��ϵ��
    /// </summary>
    [SerializeField] private float moveEffect = 1f;

    public override void DoAnimationTriggerEventLogic()
    {
        base.DoAnimationTriggerEventLogic();
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        //����ֱ�ٶ���Ϊ0
        applyVector.y = 0;
        //�̳������Ծʱ��ˮƽ�ٶ�
        //applyVector.x = m_playerController.velocity.x;
        //applyVector.z = m_playerController.velocity.z;
        //ע��һ��ʼֱ����CharacterController.velocity��ȡ�ٶȣ������ڴ���ǰ��velocity������쳣���󣬵��´��ͺ󡰳塱��ȥ
        //���������޹����Ʋ��Ǵ����漰��������������ݣ���������ٶ�ֱ����ǰһ֡��applyVector����ֵ
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
    /// ���ֶ���
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MaintainMomentum(object sender, EventArgs e)
    {
        var info = e as EventParameter;
        if (info.maintainVec != Vector3.zero)
        {            
            maintainMomentum = info.maintainVec;
            //�ڽ�����ת��ΪmaintainMomentum��ԭ�ȵĴ�ֱ�ٶ�Ӧ�ù���
            applyVector = Vector3.zero;
        }
    }
}
