using UnityEngine;

public class InputManager : MonoBehaviour
{
    /// <summary>
    /// 输入系统
    /// </summary>
    private InputSystem inputSystem;
    /// <summary>
    /// 玩家输入
    /// </summary>
    private InputSystem.PlayerControllerActions playerInput;
    /// <summary>
    /// 单例
    /// </summary>
    public static InputManager Instance;
    /// <summary>
    /// 玩家的移动输入
    /// </summary>
    [HideInInspector] public Vector2 groundMove;
    /// <summary>
    /// 玩家的鼠标输入
    /// </summary>
    [HideInInspector] public Vector2 mouseInput;
    /// <summary>
    /// 玩家的跳跃输入
    /// </summary>
    [HideInInspector] public bool jumpInput;
    /// <summary>
    /// 玩家的左键开火输入
    /// </summary>
    [HideInInspector] public bool leftMouseFire;
    /// <summary>
    /// 玩家的右键开火输入
    /// </summary>
    [HideInInspector] public bool rightMouseFire;
    /// <summary>
    /// 当前鼠标的位置
    /// </summary>
    [HideInInspector] public Vector2 mousePosition;
    /// <summary>
    /// 是否按下了拾取按钮
    /// </summary>
    [HideInInspector] public bool pickUp;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
        //初始化操作
        inputSystem = new InputSystem();
        playerInput = inputSystem.PlayerController;

        playerInput.Move.performed += ctx => groundMove = ctx.ReadValue<Vector2>();
        playerInput.Jump.started += ctx => jumpInput = true;
        playerInput.Jump.canceled += ctx => jumpInput = false;
        playerInput.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
        playerInput.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
        playerInput.FireLeft.started += ctx => this.TriggerEvent(EventName.LaunchPortal, new EventParameter { eventInt = 0 });
        playerInput.FireRight.started += ctx => this.TriggerEvent(EventName.LaunchPortal, new EventParameter { eventInt = 1 });
        playerInput.MousePosition.performed += ctx => mousePosition = ctx.ReadValue<Vector2>();
        playerInput.PickUp.started += ctx => this.TriggerEvent(EventName.PickUp);
    }
    private void OnEnable()
    {
        inputSystem.Enable();
    }
    private void OnDisable()
    {
        inputSystem.Disable();
    }
}
