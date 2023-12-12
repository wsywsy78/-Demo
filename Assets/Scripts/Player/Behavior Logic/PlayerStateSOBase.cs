using System;
using UnityEngine;

public class PlayerStateSOBase : ScriptableObject
{
    /// <summary>
    /// ��Ҷ���
    /// </summary>
    protected Player player;
    /// <summary>
    /// ��Ҷ���Transform
    /// </summary>
    protected Transform transform;
    /// <summary>
    /// ʵ�����ű�����
    /// </summary>
    protected GameObject gameObject;
    /// <summary>
    /// ��ҵĽ�ɫ������
    /// </summary>
    protected CharacterController m_playerController;
    /// <summary>
    /// ��ʱ������ֹ��β���������
    /// </summary>
    protected float launchTimer = 0;
    /// <summary>
    /// InputManager�ĵ���������
    /// </summary>
    protected InputManager inputManager;
    /// <summary>
    /// ģ������
    /// </summary>
    [SerializeField] protected float virtualGravity = -10f;
    /// <summary>
    /// ��ҵ�������˶�ʸ��
    /// </summary>
    protected Vector3 motionVector = Vector3.zero;
    /// <summary>
    /// ����Ѿ���ʩ�ӵ��˶�ʸ��
    /// </summary>
    protected Vector3 applyVector = Vector3.zero;
    /// <summary>
    /// ��������ֵĶ���
    /// </summary>
    protected Vector3 maintainMomentum = Vector3.zero;
    /// <summary>
    /// ��¼��ҽ��µ���ײ��Ϣ
    /// </summary>
    protected RaycastHit groundHit;
    /// <summary>
    /// ������ײ���id��Ϣ
    /// </summary>
    protected int groundColliderId;
    /// <summary>
    /// ���ȵĵ����Ƿ��Ǵ�����
    /// </summary>
    protected bool isGroundPortal;
    /// <summary>
    /// �����Ų㼶
    /// </summary>
    protected int portalLayer;
    /// <summary>
    /// �Ƿ�Ϊ��ƽ̨��
    /// </summary>
    protected bool isOnPlatform;
    /// <summary>
    /// �õ�PlayerIsGround������
    /// </summary>
    protected PlayerIsGround playerIsGround;
    /// <summary>
    /// ��ʼ��״̬�ű�����,ʹ��״̬�����gameObject,transform����Ϊ��ҵ�
    /// </summary>
    /// <param name="_gameObject">״̬�ű�����</param>
    /// <param name="_player">��Ҷ���</param>
    public virtual void Initialize(GameObject _gameObject, Player _player)
    {
        this.gameObject = _gameObject;
        transform = _gameObject.transform;
        this.player = _player;
        m_playerController = this.player.playerController;
        inputManager = InputManager.Instance;
        playerIsGround = player.playerIsGround;        
        //playerIsGround = PlayerIsGround.Instance;        
    }

    /// <summary>
    /// ����״̬�߼�
    /// </summary>
    /// <param name="_animBoolName">��������</param>
    public virtual void DoEnterLogic() {}
    /// <summary>
    /// �˳�״̬�߼�
    /// </summary>
    /// <param name="_animBoolName">��������</param>
    public virtual void DoExitLogic() { ResetValues(); }
    /// <summary>
    /// ÿ֡�����¼�
    /// </summary>
    public virtual void DoFrameUpdateLogic()
    {
        if (Physics.Raycast(m_playerController.bounds.center, Vector3.down, out groundHit, m_playerController.bounds.extents.y + 0.2f))
        {
            groundColliderId = groundHit.collider.GetInstanceID();
        }
        isGroundPortal = groundColliderId == PortalTeleportManager.Instance.cacheCollidersId;
        //�����ҵ��ŵ�״̬����Ծ����
        //if (m_playerController.isGrounded && inputManager.jumpInput)
        if (playerIsGround.IsGround() && inputManager.jumpInput)
        {
            GetPhysicsProperty.isJump = true;
            player.stateMachine.ChangeState(player.playerAirState);
            return;
        }
        //if ((m_playerController.velocity.y <= 0 && !m_playerController.isGrounded) || isGroundPortal)
        //if (m_playerController.velocity.y < 0f && !playerIsGround)
        //{
        //    player.stateMachine.ChangeState(player.playerFallState);
        //}
    }
    /// <summary>
    /// ÿ����֡�����¼�
    /// </summary>
    public virtual void DoPhysicsUpdateLogic()
    {
        ////�����Ҳ��ٵ����ϣ������ʩ������
        //if (!playerIsGround.IsGround() || isGroundPortal)
        //{
        //    //�������ڱ�ʩ�ӵ��������Ե���applyVector
        //    applyVector.y += virtualGravity * Time.deltaTime;            
        //}
        //������ҵĴ�ֱ�ٶȵ�������Сֵ
        applyVector.y = Mathf.Clamp(applyVector.y, -20, 20);


        //��ҵ��˶�ʸ��ȡ������ҵ��������ұ�ʩ�ӵ��˶�ʸ��
        m_playerController.Move((motionVector + applyVector + maintainMomentum) * Time.deltaTime);
        //if (inputManager.leftMouseFire && launchTimer < 0)
        //{
        //    player.protalController.LaunchProtal(0);
        //    launchTimer = .2f;
        //}
        //if (inputManager.rightMouseFire && launchTimer < 0)
        //{
        //    player.protalController.LaunchProtal(1);
        //    launchTimer = .2f;
        //}
        //launchTimer -= Time.deltaTime;

        //��¼�����һ֡�ĸ����ƶ�����
        //RecordVelocity(motionVector, applyVector, maintainMomentum);
        RecordVelocity(motionVector, applyVector, maintainMomentum);
    }
    /// <summary>
    /// ���������¼�
    /// </summary>
    public virtual void DoAnimationTriggerEventLogic() { }
    /// <summary>
    /// �ع��ı�ֵ
    /// </summary>
    public virtual void ResetValues() { }
    /// <summary>
    /// ����Ƿ����쳣�ٶ�
    /// </summary>
    protected virtual void CheckForExceptionVelocity() 
    {
        Vector3 totalVelocity = motionVector + applyVector + maintainMomentum;
        if ((m_playerController.velocity.sqrMagnitude - totalVelocity.sqrMagnitude) > 300f)
        {
            Physics.IgnoreLayerCollision(6, 3);
            m_playerController.Move(-(m_playerController.velocity - totalVelocity) * Time.deltaTime);
            Physics.IgnoreLayerCollision(6, 3, false);
        }
    }

    /// <summary>
    /// ��¼��һ֡��ҵĸ����ƶ����ٶ�
    /// </summary>
    protected virtual void RecordVelocity(Vector3 _motionVec, Vector3 _applyVec, Vector3 _maintainMomentum)
    {
        GetPhysicsProperty.SetPrevFrameVec(_motionVec, _applyVec, _maintainMomentum);
    }

}
