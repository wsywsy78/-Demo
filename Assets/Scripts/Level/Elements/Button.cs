using DG.Tweening;
using UnityEngine;

public class Button : MonoBehaviour
{
    /// <summary>
    /// ��ť��Ӧ����
    /// </summary>
    [SerializeField] private Door matchDoor;
    /// <summary>
    /// ��ť��Transform
    /// </summary>
    [SerializeField] private Transform button;
    /// <summary>
    /// ��ť���Ƿ�������ѹס
    /// </summary>
    [SerializeField] private bool isTrigger;
    /// <summary>
    /// ��ť�ڱ����º������뿪
    /// </summary>
    [SerializeField] private bool isReturn;
    /// <summary>
    /// ����/�ص������Ƿ����ڴ���
    /// </summary>
    private bool isPlayed = false;
    /// <summary>
    /// ��ť��С��������
    /// </summary>
    [SerializeField] private float minTriggerMass = 0.9f;
    /// <summary>
    /// ͬʱ�ڰ�ť�ϵ���������
    /// </summary>
    private int triggerObjectCount = 0;
    /// <summary>
    /// ������ť��ײ��
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
            //��֤triggerObjectCount����С��0
            triggerObjectCount = Mathf.Clamp(--triggerObjectCount, 0, 10);
        }
    }

    /// <summary>
    /// ��鰴ť�ϵ�����������Ƿ񳬹���С�޶�
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
