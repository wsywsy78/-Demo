using UnityEngine;

public class Portal : MonoBehaviour
{
    /// <summary>
    /// �������ϵ�Render���
    /// </summary>
    private Renderer render;
    /// <summary>
    /// �����ŵĲ���
    /// </summary>
    private Material material;
    /// <summary>
    /// �������������������ײ��
    /// </summary>
    public Collider attachCollider { get; private set; }
    /// <summary>
    /// �������Ƿ񱻷���
    /// </summary>
    [SerializeField] public bool isPlaced = false;
    /// <summary>
    /// ��ȡPortalTeleportManager�ĵ�������
    /// </summary>
    private PortalTeleportManager portalTeleport;
    /// <summary>
    /// ��һ��������
    /// </summary>
    public Portal anotherPortal { get; private set; }
    /// <summary>
    /// �����
    /// </summary>
    private Transform parent;

    private void Awake()
    {
        render = GetComponent<Renderer>();
        material = render.material;
        parent = transform.parent;
    }

    private void Start()
    {
        portalTeleport = PortalTeleportManager.Instance;
        for(int i = 0; i < parent.childCount; i++)
        {
            if(parent.GetChild(i) != this.transform)
            {
                anotherPortal = parent.GetChild(i).GetComponent<Portal>();
                break;
            }
        }
    }

    /// <summary>
    /// �����ʵ���������Ϊ�����RenderTexture
    /// </summary>
    /// <param name="_tex">���������</param>
    public void SetTexture(RenderTexture _tex)
    {
        material.mainTexture = _tex;
    }
    /// <summary>
    /// �ô������Ƿ�ɼ�
    /// </summary>
    /// <returns>���ش����ŵĿɼ���</returns>
    public bool IsRenderVisible()
    {
        return render.isVisible;
    }

    public void SetAttachCollider(Collider _attachCollider)
    {
        this.attachCollider = _attachCollider;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "IsPortalable" && isPlaced && anotherPortal.isPlaced)
        {
            //Physics.IgnoreCollision(other, attachCollider);
            //portalTeleport.SetObjectInPortal(other.gameObject, this.transform, anotherPortal.transform);
            //Debug.Log($"{attachCollider.GetInstanceID()},portal:{name}");
            this.TriggerEvent(EventName.GetInPortal, new EventParameter
            {
                eventGetInPortal = new EventParameter.InPortal
                {
                    objectCollider = other,
                    attachCollider = attachCollider,
                    inportal = transform,
                    outportal = anotherPortal.transform
                }
            });
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "IsPortalable" && isPlaced && anotherPortal.isPlaced)
        {
            //Physics.IgnoreCollision(other, attachCollider, false);
            //portalTeleport.TeleportObjectOutPortal(other.gameObject);
            //Debug.Log($"Exit:{other.name},portal:{name}");
            this.TriggerEvent(EventName.GetOutPortal, new EventParameter
            {
                eventGetOutPortal = other.GetInstanceID()
            });
            ////��������������뿪�����ŵ�ʱ��Ҫ��Ӧ��ȡ���ض�״̬
            //if(other.CompareTag("Player") && PickObjectController.Instance.pickedObject != null)
            //{
            //    Debug.Log("RecoverPick");
            //    this.TriggerEvent(EventName.PickGetInfo, new EventParameter { eventTransforms = null });
            //}
        }
    }

}
