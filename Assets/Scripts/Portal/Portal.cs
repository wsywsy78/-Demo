using UnityEngine;

public class Portal : MonoBehaviour
{
    /// <summary>
    /// 传送门上的Render组件
    /// </summary>
    private Renderer render;
    /// <summary>
    /// 传送门的材质
    /// </summary>
    private Material material;
    /// <summary>
    /// 传送门所依附物体的碰撞体
    /// </summary>
    public Collider attachCollider { get; private set; }
    /// <summary>
    /// 传送门是否被放置
    /// </summary>
    [SerializeField] public bool isPlaced = false;
    /// <summary>
    /// 获取PortalTeleportManager的单例引用
    /// </summary>
    private PortalTeleportManager portalTeleport;
    /// <summary>
    /// 另一个传送门
    /// </summary>
    public Portal anotherPortal { get; private set; }
    /// <summary>
    /// 父组件
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
    /// 将材质的主纹理设为传入的RenderTexture
    /// </summary>
    /// <param name="_tex">传入的纹理</param>
    public void SetTexture(RenderTexture _tex)
    {
        material.mainTexture = _tex;
    }
    /// <summary>
    /// 该传送门是否可见
    /// </summary>
    /// <returns>返回传送门的可见性</returns>
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
            ////当玩家吸附物体离开传送门的时候，要相应的取消特定状态
            //if(other.CompareTag("Player") && PickObjectController.Instance.pickedObject != null)
            //{
            //    Debug.Log("RecoverPick");
            //    this.TriggerEvent(EventName.PickGetInfo, new EventParameter { eventTransforms = null });
            //}
        }
    }

}
