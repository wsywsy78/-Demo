using DG.Tweening;
using UnityEngine;

public class Button : MonoBehaviour
{
    /// <summary>
    /// 按钮对应的门
    /// </summary>
    [SerializeField] private Door matchDoor;
    /// <summary>
    /// 按钮的Transform
    /// </summary>
    [SerializeField] private Transform button;
    /// <summary>
    /// 按钮上是否有物体压住
    /// </summary>
    [SerializeField] private bool isTrigger;
    /// <summary>
    /// 按钮在被按下后物体离开
    /// </summary>
    [SerializeField] private bool isReturn;
    /// <summary>
    /// 按下/回弹动画是否正在触发
    /// </summary>
    private bool isPlayed = false;
    /// <summary>
    /// 按钮最小触发重量
    /// </summary>
    [SerializeField] private float minTriggerMass = 0.9f;
    /// <summary>
    /// 同时在按钮上的物体数量
    /// </summary>
    private int triggerObjectCount = 0;
    /// <summary>
    /// 触发按钮碰撞体
    /// </summary>
    private BoxCollider triggerCollider;

    private void Start()
    {
        triggerCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (triggerObjectCount > 0)
        {
            isTrigger = true;
        }
        else
        {
            if (isTrigger)
            {
                isTrigger = false;
                isReturn = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isPlayed)
        {
            return;
        }
        if (isTrigger)
        {
            button.DOLocalMove(new Vector3(0, -0.2f, 0), 0.3f).SetEase(Ease.Linear).OnComplete(
                () =>
                {
                    isPlayed = false;
                });
            //matchDoor.OpenAndCloseDoor(true);
            this.TriggerEvent(EventName.UnlockOneKey, new EventParameter { eventInt = gameObject.GetInstanceID() });
            isPlayed = true;
        }
        else if (isReturn)
        {
            button.DOLocalMove(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.Linear).OnComplete(
                () =>
                {
                    isPlayed = false;
                    isReturn = false;
                });
            //matchDoor.OpenAndCloseDoor(false);
            this.TriggerEvent(EventName.LockOneKey, new EventParameter { eventInt = gameObject.GetInstanceID() });
            isPlayed = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || CheckForMass(other))
        {
            triggerObjectCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || CheckForMass(other))
        {
            //保证triggerObjectCount不能小于0
            triggerObjectCount = Mathf.Clamp(--triggerObjectCount, 0, 10);
        }
    }

    /// <summary>
    /// 检查按钮上的物体的质量是否超过最小限度
    /// </summary>
    /// <param name="_triggerObject"></param>
    /// <returns></returns>
    private bool CheckForMass(Collider _triggerObject)
    {
        if (_triggerObject.GetComponent<Rigidbody>() == null)
            return false;
        var _rb = _triggerObject.GetComponent<Rigidbody>();
        if (_rb.mass > minTriggerMass)
            return true;
        return false;
    }
}
