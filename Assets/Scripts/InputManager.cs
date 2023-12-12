using UnityEngine;

public class InputManager : MonoBehaviour
{
    /// <summary>
    /// ����ϵͳ
    /// </summary>
    private InputSystem inputSystem;
    /// <summary>
    /// �������
    /// </summary>
    private InputSystem.PlayerControllerActions playerInput;
    /// <summary>
    /// ����
    /// </summary>
    public static InputManager Instance;
    /// <summary>
    /// ��ҵ��ƶ�����
    /// </summary>
    [HideInInspector] public Vector2 groundMove;
    /// <summary>
    /// ��ҵ��������
    /// </summary>
    [HideInInspector] public Vector2 mouseInput;
    /// <summary>
    /// ��ҵ���Ծ����
    /// </summary>
    [HideInInspector] public bool jumpInput;
    /// <summary>
    /// ��ҵ������������
    /// </summary>
    [HideInInspector] public bool leftMouseFire;
    /// <summary>
    /// ��ҵ��Ҽ���������
    /// </summary>
    [HideInInspector] public bool rightMouseFire;
    /// <summary>
    /// ��ǰ����λ��
    /// </summary>
    [HideInInspector] public Vector2 mousePosition;
    /// <summary>
    /// �Ƿ�����ʰȡ��ť
    /// </summary>
    [HideInInspector] public bool pickUp;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
        //��ʼ������
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
