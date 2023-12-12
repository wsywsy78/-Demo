using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerIsGround : MonoBehaviour
{
    /// <summary>
    /// ����
    /// </summary>
    public static PlayerIsGround Instance;
    /// <summary>
    /// ������ײ��ĵ׵�
    /// </summary>
    private Vector3 poinCenter;
    /// <summary>
    /// ������ײ��İ뾶
    /// </summary>
    private float radius;
    /// <summary>
    /// ������ײ��ĸ߶�
    /// </summary>
    private float height;
    /// <summary>
    /// �ײ�ƫ��
    /// </summary>
    [SerializeField] private float offset = 0.1f;
    /// <summary>
    /// ��ҵ�Layer��
    /// </summary>
    [SerializeField] private LayerMask playerLayer;
    /// <summary>
    /// ��⵽����ײ�建��
    /// </summary>
    private Collider[] checkCollider;
    /// <summary>
    /// ����Ƿ���ƽ̨��
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
    /// �������Ƿ��ڵ���
    /// </summary>
    /// <returns></returns>
    public bool IsGround()
    {
        //���εײ�
        poinCenter = transform.position + transform.up * radius - transform.up * offset;
        //ͨ���ײ����μ��
        int _colliderCount = Physics.OverlapSphereNonAlloc(poinCenter, radius, checkCollider, ~playerLayer);

        if (_colliderCount != 0)
        {
            //�������Ƿ���ƽ̨��
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
    /// �������Ƿ���ƽ̨��
    /// </summary>
    public void IsOnPlatform(int _count)
    {
        //���������е���ײ�����Ƿ���ƽ̨
        for(int i = 0; i < _count; i++)
        {
            if (checkCollider[i].CompareTag("Platform"))
            {                
                isOnPlatform = true;
                //�õ�ƽ̨���ٶ�
                GetPlatformVelocity(checkCollider[i]);
                return;
            }
        }
        //���û��ƽ̨����isOnPlatform��Ϊfalse,ƽ̨�ٶ���Ϊ0
        isOnPlatform = false;
        platformVelocity = Vector3.zero;
    }

    /// <summary>
    /// ƽ̨�ٶ�
    /// </summary>
    public Vector3 platformVelocity { get; private set; }
    /// <summary>
    /// �õ�ƽ̨���ٶ�
    /// </summary>
    /// <param name="_platformCollider">ƽ̨����ײ��</param>
    private void GetPlatformVelocity(Collider _platformCollider)
    {
        //�õ�platform�ϵĸ�����������û�У�����
        //var platformRb = _platformCollider.GetComponentInChildren<Platform>();
        //����Ҫ�õ��ӿڣ�����ʹ��TryGetComponent
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
