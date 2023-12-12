using System;
using UnityEngine;

public class PlayerStateSOBase : ScriptableObject
{
    /// <summary>
    /// 玩家对象
    /// </summary>
    protected Player player;
    /// <summary>
    /// 玩家对象Transform
    /// </summary>
    protected Transform transform;
    /// <summary>
    /// 实例化脚本对象
    /// </summary>
    protected GameObject gameObject;
    /// <summary>
    /// 玩家的角色控制器
    /// </summary>
    protected CharacterController m_playerController;
    /// <summary>
    /// 计时器，防止多次产生传送门
    /// </summary>
    protected float launchTimer = 0;
    /// <summary>
    /// InputManager的单例的引用
    /// </summary>
    protected InputManager inputManager;
    /// <summary>
    /// 模拟重力
    /// </summary>
    [SerializeField] protected float virtualGravity = -10f;
    /// <summary>
    /// 玩家的输入的运动矢量
    /// </summary>
    protected Vector3 motionVector = Vector3.zero;
    /// <summary>
    /// 玩家已经被施加的运动矢量
    /// </summary>
    protected Vector3 applyVector = Vector3.zero;
    /// <summary>
    /// 玩家所保持的动量
    /// </summary>
    protected Vector3 maintainMomentum = Vector3.zero;
    /// <summary>
    /// 记录玩家脚下的碰撞信息
    /// </summary>
    protected RaycastHit groundHit;
    /// <summary>
    /// 脚下碰撞体的id信息
    /// </summary>
    protected int groundColliderId;
    /// <summary>
    /// 所踩的地面是否是传送门
    /// </summary>
    protected bool isGroundPortal;
    /// <summary>
    /// 传送门层级
    /// </summary>
    protected int portalLayer;
    /// <summary>
    /// 是否为在平台上
    /// </summary>
    protected bool isOnPlatform;
    /// <summary>
    /// 得到PlayerIsGround的引用
    /// </summary>
    protected PlayerIsGround playerIsGround;
    /// <summary>
    /// 初始化状态脚本对象,使得状态对象的gameObject,transform覆盖为玩家的
    /// </summary>
    /// <param name="_gameObject">状态脚本对象</param>
    /// <param name="_player">玩家对象</param>
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
    /// 进入状态逻辑
    /// </summary>
    /// <param name="_animBoolName">动画名称</param>
    public virtual void DoEnterLogic() {}
    /// <summary>
    /// 退出状态逻辑
    /// </summary>
    /// <param name="_animBoolName">动画名称</param>
    public virtual void DoExitLogic() { ResetValues(); }
    /// <summary>
    /// 每帧调用事件
    /// </summary>
    public virtual void DoFrameUpdateLogic()
    {
        if (Physics.Raycast(m_playerController.bounds.center, Vector3.down, out groundHit, m_playerController.bounds.extents.y + 0.2f))
        {
            groundColliderId = groundHit.collider.GetInstanceID();
        }
        isGroundPortal = groundColliderId == PortalTeleportManager.Instance.cacheCollidersId;
        //检查玩家的着地状态及跳跃输入
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
    /// 每物理帧调用事件
    /// </summary>
    public virtual void DoPhysicsUpdateLogic()
    {
        ////如果玩家不再地面上，则对其施加重力
        //if (!playerIsGround.IsGround() || isGroundPortal)
        //{
        //    //重力属于被施加的力，所以调整applyVector
        //    applyVector.y += virtualGravity * Time.deltaTime;            
        //}
        //限制玩家的垂直速度的最大和最小值
        applyVector.y = Mathf.Clamp(applyVector.y, -20, 20);


        //玩家的运动矢量取决于玩家的输入和玩家被施加的运动矢量
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

        //记录玩家这一帧的各种移动参数
        //RecordVelocity(motionVector, applyVector, maintainMomentum);
        RecordVelocity(motionVector, applyVector, maintainMomentum);
    }
    /// <summary>
    /// 动画触发事件
    /// </summary>
    public virtual void DoAnimationTriggerEventLogic() { }
    /// <summary>
    /// 回滚改变值
    /// </summary>
    public virtual void ResetValues() { }
    /// <summary>
    /// 检查是否有异常速度
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
    /// 记录这一帧玩家的各种移动的速度
    /// </summary>
    protected virtual void RecordVelocity(Vector3 _motionVec, Vector3 _applyVec, Vector3 _maintainMomentum)
    {
        GetPhysicsProperty.SetPrevFrameVec(_motionVec, _applyVec, _maintainMomentum);
    }

}
