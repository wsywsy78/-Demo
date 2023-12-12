using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerIsGround : MonoBehaviour
{
    /// <summary>
    /// 单例
    /// </summary>
    public static PlayerIsGround Instance;
    /// <summary>
    /// 胶囊碰撞体的底点
    /// </summary>
    private Vector3 poinCenter;
    /// <summary>
    /// 胶囊碰撞体的半径
    /// </summary>
    private float radius;
    /// <summary>
    /// 胶囊碰撞体的高度
    /// </summary>
    private float height;
    /// <summary>
    /// 底部偏移
    /// </summary>
    [SerializeField] private float offset = 0.1f;
    /// <summary>
    /// 玩家的Layer层
    /// </summary>
    [SerializeField] private LayerMask playerLayer;
    /// <summary>
    /// 检测到的碰撞体缓存
    /// </summary>
    private Collider[] checkCollider;
    /// <summary>
    /// 玩家是否在平台上
    /// </summary>
    public bool isOnPlatform { get ; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);    
    }

    private void Start()
    {
        checkCollider = new Collider[10];
        radius = GetComponent<CharacterController>().radius;
        height = GetComponent<CharacterController>().height;
    }

    /// <summary>
    /// 检测玩家是否在地面
    /// </summary>
    /// <returns></returns>
    public bool IsGround()
    {
        //球形底部
        poinCenter = transform.position + transform.up * radius - transform.up * offset;
        //通过底部球形检测
        int _colliderCount = Physics.OverlapSphereNonAlloc(poinCenter, radius, checkCollider, ~playerLayer);

        if (_colliderCount != 0)
        {
            //检查玩家是否在平台上
            IsOnPlatform(_colliderCount);
            return true;
        }
        else
        {
            isOnPlatform = false;
            return false;
        }
    }

    /// <summary>
    /// 检测玩家是否在平台上
    /// </summary>
    public void IsOnPlatform(int _count)
    {
        //检查脚下所有的碰撞体中是否有平台
        for(int i = 0; i < _count; i++)
        {
            if (checkCollider[i].CompareTag("Platform"))
            {                
                isOnPlatform = true;
                //得到平台的速度
                GetPlatformVelocity(checkCollider[i]);
                return;
            }
        }
        //如果没有平台，将isOnPlatform设为false,平台速度置为0
        isOnPlatform = false;
        platformVelocity = Vector3.zero;
    }

    /// <summary>
    /// 平台速度
    /// </summary>
    public Vector3 platformVelocity { get; private set; }
    /// <summary>
    /// 得到平台的速度
    /// </summary>
    /// <param name="_platformCollider">平台的碰撞体</param>
    private void GetPlatformVelocity(Collider _platformCollider)
    {
        //得到platform上的刚体组件，如果没有，返回
        //var platformRb = _platformCollider.GetComponentInChildren<Platform>();
        //由于要得到接口，这里使用TryGetComponent
        var _platformVec = _platformCollider.GetComponentInParent<IPlatform>();
        if (_platformVec == null)
        {
            return;
        }        
        //platformVelocity = platformRb.moveVelocity;
        platformVelocity = _platformVec.GetPlatformVelocity();
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawSphere(poinCenter, radius);
    //}
}
