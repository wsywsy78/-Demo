using UnityEngine;

/// <summary>
/// ��ҽű�����
/// </summary>
public class Player : MonoBehaviour
{
    #region ״̬������
    /// <summary>
    /// ״̬��
    /// </summary>
    public PlayerStateMachine stateMachine;
    /// <summary>
    /// վ��״̬
    /// </summary>
    public PlayerIdleState playerIdleState;
    /// <summary>
    /// ����״̬
    /// </summary>
    public PlayerWalkState playerWalkState;
    /// <summary>
    /// ��Ծ��״̬
    /// </summary>
    public PlayerJumpstate playerJumpState;
    /// <summary>
    /// �����״̬
    /// </summary>
    public PlayerFallState playerFallState;
    /// <summary>
    /// ����״̬
    /// </summary>
    public PlayerFollowState playerFollowState;
    /// <summary>
    /// ����״̬
    /// </summary>
    public PlayerAirState playerAirState;
    #endregion

    #region ����״̬���󻯽ű�
    [SerializeField] private PlayerIdleSOBase playerIdleBase;
    [SerializeField] private PlayerWalkSOBase playerWalkBase;
    [SerializeField] private PlayerJumpSOBase playerJumpBase;
    [SerializeField] private PlayerFallSOBase playerFallBase;
    [SerializeField] private PlayerFollowSOBase playerFollowBase;
    [SerializeField] private PlayerAirSOBase playerAirBase;
    /// <summary>
    /// ���վ��״̬�ű�������ʵ��
    /// </summary>
    public PlayerIdleSOBase playerIdleBaseInstance { get; private set; }
    /// <summary>
    /// �������״̬�ű�������ʵ��
    /// </summary>
    public PlayerWalkSOBase playerWalkBaseInstance { get; private set; }
    /// <summary>
    /// �����Ծ״̬�ű�������ʵ��
    /// </summary>
    public PlayerJumpSOBase playerJumpBaseInstance { get; private set; }
    /// <summary>
    /// �������״̬�ű�������ʵ��
    /// </summary>
    public PlayerFallSOBase playerFallBaseInstance { get; private set; }
    /// <summary>
    /// ��Ҹ���״̬�ű�������ʵ��
    /// </summary>
    public PlayerFollowSOBase playerFollowBaseInstance { get; private set; }
    /// <summary>
    /// ��ҿ���״̬�ű�������ʵ��
    /// </summary>
    public PlayerAirSOBase playerAirBaseInstance { get; private set; }
    #endregion
    /// <summary>
    /// ��Ҷ���������
    /// </summary>
    public Animator anim { get; private set; }
    /// <summary>
    /// ��ҵĽ�ɫ������
    /// </summary>
    [HideInInspector] public CharacterController playerController;
    /// <summary>
    /// ��ҵ��ƶ�����
    /// </summary>
    public Vector2 playerMove => InputManager.Instance.groundMove;
    /// <summary>
    /// �����Ź�����
    /// </summary>
    public PortalLaunchController protalController;
    /// <summary>
    /// ʰȡ���������
    /// </summary>
    public PickObjectController pickObjectController;

    public PlayerIsGround playerIsGround;
    private void Awake()
    {
        //״̬��ʼ��
        stateMachine = new PlayerStateMachine();
        playerIdleState = new PlayerIdleState(this, stateMachine, "isIdle");
        playerWalkState = new PlayerWalkState(this, stateMachine, "isMove");
        playerJumpState = new PlayerJumpstate(this, stateMachine, "isJump");
        playerFallState = new PlayerFallState(this, stateMachine, "isFall");
        playerFollowState = new PlayerFollowState(this, stateMachine, "isIdle");
        playerAirState = new PlayerAirState(this, stateMachine, "isAir");

        //״̬ʵ������
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
        //״̬�ű��������ʼ��
        playerIdleBaseInstance.Initialize(gameObject, this);
        playerWalkBaseInstance.Initialize(gameObject, this);
        playerJumpBaseInstance.Initialize(gameObject, this);
        playerFallBaseInstance.Initialize(gameObject, this);
        playerFollowBaseInstance.Initialize(gameObject, this);
        playerAirBaseInstance.Initialize(gameObject, this);

        //״̬����ʼ��
        stateMachine.Initialize(playerIdleState);

    }

    private void Update()
    {
        //��ǰ״̬��֡����
        stateMachine.currentState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        //��ǰ״̬������֡����
        stateMachine.currentState.PhysicsUpdate();
    }

}
