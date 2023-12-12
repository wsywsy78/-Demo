using UnityEngine;

/// <summary>
/// 玩家脚本基类
/// </summary>
public class Player : MonoBehaviour
{
    #region 状态机种类
    /// <summary>
    /// 状态机
    /// </summary>
    public PlayerStateMachine stateMachine;
    /// <summary>
    /// 站立状态
    /// </summary>
    public PlayerIdleState playerIdleState;
    /// <summary>
    /// 行走状态
    /// </summary>
    public PlayerWalkState playerWalkState;
    /// <summary>
    /// 跳跃的状态
    /// </summary>
    public PlayerJumpstate playerJumpState;
    /// <summary>
    /// 下落的状态
    /// </summary>
    public PlayerFallState playerFallState;
    /// <summary>
    /// 跟随状态
    /// </summary>
    public PlayerFollowState playerFollowState;
    /// <summary>
    /// 空中状态
    /// </summary>
    public PlayerAirState playerAirState;
    #endregion

    #region 人物状态对象化脚本
    [SerializeField] private PlayerIdleSOBase playerIdleBase;
    [SerializeField] private PlayerWalkSOBase playerWalkBase;
    [SerializeField] private PlayerJumpSOBase playerJumpBase;
    [SerializeField] private PlayerFallSOBase playerFallBase;
    [SerializeField] private PlayerFollowSOBase playerFollowBase;
    [SerializeField] private PlayerAirSOBase playerAirBase;
    /// <summary>
    /// 玩家站立状态脚本化对象实例
    /// </summary>
    public PlayerIdleSOBase playerIdleBaseInstance { get; private set; }
    /// <summary>
    /// 玩家行走状态脚本化对象实例
    /// </summary>
    public PlayerWalkSOBase playerWalkBaseInstance { get; private set; }
    /// <summary>
    /// 玩家跳跃状态脚本化对象实例
    /// </summary>
    public PlayerJumpSOBase playerJumpBaseInstance { get; private set; }
    /// <summary>
    /// 玩家下落状态脚本化对象实例
    /// </summary>
    public PlayerFallSOBase playerFallBaseInstance { get; private set; }
    /// <summary>
    /// 玩家跟随状态脚本化对象实例
    /// </summary>
    public PlayerFollowSOBase playerFollowBaseInstance { get; private set; }
    /// <summary>
    /// 玩家空中状态脚本化对象实例
    /// </summary>
    public PlayerAirSOBase playerAirBaseInstance { get; private set; }
    #endregion
    /// <summary>
    /// 玩家动画控制器
    /// </summary>
    public Animator anim { get; private set; }
    /// <summary>
    /// 玩家的角色控制器
    /// </summary>
    [HideInInspector] public CharacterController playerController;
    /// <summary>
    /// 玩家的移动输入
    /// </summary>
    public Vector2 playerMove => InputManager.Instance.groundMove;
    /// <summary>
    /// 传送门管理器
    /// </summary>
    public PortalLaunchController protalController;
    /// <summary>
    /// 拾取物体管理器
    /// </summary>
    public PickObjectController pickObjectController;

    public PlayerIsGround playerIsGround;
    private void Awake()
    {
        //状态初始化
        stateMachine = new PlayerStateMachine();
        playerIdleState = new PlayerIdleState(this, stateMachine, "isIdle");
        playerWalkState = new PlayerWalkState(this, stateMachine, "isMove");
        playerJumpState = new PlayerJumpstate(this, stateMachine, "isJump");
        playerFallState = new PlayerFallState(this, stateMachine, "isFall");
        playerFollowState = new PlayerFollowState(this, stateMachine, "isIdle");
        playerAirState = new PlayerAirState(this, stateMachine, "isAir");

        //状态实例建立
        playerIdleBaseInstance = Instantiate(playerIdleBase);
        playerWalkBaseInstance = Instantiate(playerWalkBase);
        playerJumpBaseInstance = Instantiate(playerJumpBase);
        playerFallBaseInstance = Instantiate(playerFallBase);
        playerFollowBaseInstance = Instantiate(playerFollowBase);
        playerAirBaseInstance = Instantiate(playerAirBase);

        playerController = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        playerIsGround = GetComponent<PlayerIsGround>();
    }

    private void Start()
    {
        //状态脚本化对象初始化
        playerIdleBaseInstance.Initialize(gameObject, this);
        playerWalkBaseInstance.Initialize(gameObject, this);
        playerJumpBaseInstance.Initialize(gameObject, this);
        playerFallBaseInstance.Initialize(gameObject, this);
        playerFollowBaseInstance.Initialize(gameObject, this);
        playerAirBaseInstance.Initialize(gameObject, this);

        //状态机初始化
        stateMachine.Initialize(playerIdleState);

    }

    private void Update()
    {
        //当前状态的帧调用
        stateMachine.currentState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        //当前状态的物理帧调用
        stateMachine.currentState.PhysicsUpdate();
    }

}
