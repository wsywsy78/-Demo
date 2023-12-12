using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PortalTeleportManager : MonoBehaviour
{
    /// <summary>
    /// 单例
    /// </summary>
    public static PortalTeleportManager Instance;
    /// <summary>
    /// 进入传送门的物体
    /// </summary>
    [SerializeField] private List<PortalableObject> inPortalObjs = new List<PortalableObject>();
    /// <summary>
    /// 缓存的碰撞体字典
    /// </summary>
    private Dictionary<int, Collider[]> cacheColliders;
    /// <summary>
    /// 碰撞体id
    /// </summary>
    public int cacheCollidersId { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
    }

    private void Start()
    {
        cacheColliders = new Dictionary<int, Collider[]>();

        EventManager.Instance.AddListener(EventName.GetInPortal, GetInPortal);
        EventManager.Instance.AddListener(EventName.GetOutPortal, GetOutPortal);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.GetInPortal, GetInPortal);
        EventManager.Instance.RemoveListener(EventName.GetOutPortal, GetOutPortal);
    }

    private void LateUpdate()
    {
        //每个在克隆列表中的物体的更新调用
        if (inPortalObjs.Count > 0)
        {
            for(int i = 0; i < inPortalObjs.Count; i++)
            {
                inPortalObjs[i].Update();
            }
        }
    }

    public void SetObjectInPortal(GameObject _cloneObject, Transform _inPortal, Transform _outPortal)
    {
        ////取消传送物体和传送门所附加的物体的碰撞体间的碰撞
        //Physics.IgnoreCollision(_attachCollider, _teleportCollider);
        inPortalObjs.Add(new PortalableObject(_cloneObject, _inPortal, _outPortal));
    }

    public void TeleportObjectOutPortal(GameObject _removeObject)
    {
        ////恢复传送物体和传送门附加物体的碰撞
        //Physics.IgnoreCollision(_attachCollider, _teleportCollider, false);
        for (int i = 0; i < inPortalObjs.Count; i++)
        {
            if (_removeObject.name == inPortalObjs[i].cloneObject.name)
            {
                Destroy(inPortalObjs[i].cloneObject);
                inPortalObjs.RemoveAt(i);
                break;
            }
        }
    }

    /// <summary>
    /// 进入传送门的委托事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GetInPortal(object sender, EventArgs e)
    {
        var info = e as EventParameter;
        if(!info.eventGetInPortal.Equals(default(EventParameter.InPortal)))
        {
            var _temInfo = info.eventGetInPortal;
            Physics.IgnoreCollision(_temInfo.objectCollider, _temInfo.attachCollider);
            //Debug.Log($"进入：{_temInfo.objectCollider.name}，附加：{_temInfo.attachCollider.name}");
            if (!cacheColliders.ContainsKey(_temInfo.objectCollider.GetInstanceID()))
                cacheColliders.Add(_temInfo.objectCollider.GetInstanceID(), new Collider[2] { _temInfo.objectCollider, _temInfo.attachCollider });
            else
                return;
            SetObjectInPortal(_temInfo.objectCollider.gameObject, _temInfo.inportal, _temInfo.outportal);

            if (_temInfo.objectCollider.CompareTag("Player"))
            {
                cacheCollidersId = _temInfo.attachCollider.GetInstanceID();
                //Debug.Log($"id:{cacheCollidersId}");
            }
        }

    }

    /// <summary>
    /// 离开传送门的委托事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GetOutPortal(object sender, EventArgs e)
    {
        var info = e as EventParameter;
        if (info.eventGetOutPortal != 0)
        {
            var _temInfo = info.eventGetOutPortal;
            if (cacheColliders.ContainsKey(_temInfo))
            {
                Physics.IgnoreCollision(cacheColliders[_temInfo][0], cacheColliders[_temInfo][1], false);
                TeleportObjectOutPortal(cacheColliders[_temInfo][0].gameObject);
                if (cacheColliders[_temInfo][0].CompareTag("Player"))
                {
                    cacheCollidersId = 0;
                    //Debug.Log($"id:{cacheCollidersId}");
                }
                cacheColliders.Remove(_temInfo);

            }
        }
    }
}

