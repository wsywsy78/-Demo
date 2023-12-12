using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class PickObjectController : MonoBehaviour
{
    /// <summary>
    /// 单例
    /// </summary>
    public static PickObjectController Instance;
    /// <summary>
    /// 玩家
    /// </summary>
    [SerializeField] private Transform player;
    /// <summary>
    /// 传送枪
    /// </summary>
    [SerializeField] private Transform portalGun;
    /// <summary>
    /// 传送枪位置缓存
    /// </summary>
    private List<Vector3> portalGunVecBuffer = new List<Vector3>();
    /// <summary>
    /// 可拾取物品的最大距离
    /// </summary>
    [Range(0f, 3f)]
    [SerializeField] private float maxPickDistance;
    /// <summary>
    /// 击中物体的信息
    /// </summary>
    private RaycastHit hit;
    /// <summary>
    /// 已经拾取的物品
    /// </summary>
    public GameObject pickedObject { get; private set; }
    /// <summary>
    /// 是否已吸附物体
    /// </summary>
    private bool isPickUp;
    /// <summary>
    /// 吸附物体相较于传送枪的位置
    /// </summary>
    [SerializeField] private Vector3 relativePosition;
    /// <summary>
    /// 吸附物体相对于传送枪的旋转
    /// </summary>
    [SerializeField] private Quaternion relativeRotation;
    /// <summary>
    /// 吸附物体的刚体
    /// </summary>
    private Rigidbody rb;
    /// <summary>
    /// 吸附物体是否在传送门内
    /// </summary>
    public bool isInPortal { get; private set; } = false;
    /// <summary>
    /// 入传送门
    /// </summary>
    private Transform inPortal;
    /// <summary>
    /// 出传送门
    /// </summary>
    private Transform outPortal;
    /// <summary>
    /// 摄像机
    /// </summary>
    private Transform playerCamera;
    /// <summary>
    /// 方块的角度限制范围（防止人物踩在方块上）
    /// </summary>
    [SerializeField] private float restrictXRotate = 45f;
    /// <summary>
    /// 方块与传送枪角度的差值限度（超过了会断开）
    /// </summary>
    [SerializeField] private float disconnectAngle = 30;
    /// <summary>
    /// 断开物体链接的时间
    /// </summary>
    private float disconnectTimer = 0.1f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
    }

    private void Start()
    {
        EventManager.Instance.AddListener(EventName.PickUp, PickUpObject);
        EventManager.Instance.AddListener(EventName.PickGetInfo, GetPortalInfo);

        playerCamera = Camera.main.transform;
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.PickUp, PickUpObject);
        EventManager.Instance.RemoveListener(EventName.PickGetInfo, GetPortalInfo);
    }

    private void FixedUpdate()
    {
        if(pickedObject != null)
        {
            PickObjectFollowed();
            CheckAngle();
        }
    }

    /// <summary>
    /// 拾取并吸引一个物体
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void PickUpObject(object sender, EventArgs e)
    {
        //如果已经吸附了，则放下
        if(isPickUp)
        {
            rb.velocity *= 0.4f;
            rb.useGravity = true;
            pickedObject = null;
            isPickUp = false;
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.mousePosition);
        if (Physics.Raycast(ray, out hit, maxPickDistance, ~0, QueryTriggerInteraction.Collide))
        {
            //如果射中的物体Tag不是PickUpObject并且射中的物体不是传送门或者是传送门但是经"反射后"没有击中PickUpObject,返回
            if (!hit.collider.CompareTag("PickUpObject") && !PortalRaycast(ray))
                return;
            pickedObject = hit.collider.gameObject;
            rb = pickedObject.GetComponent<Rigidbody>();
            rb.useGravity = false;
            relativeRotation = Quaternion.LookRotation(portalGun.forward, Vector3.up);
            relativeRotation = Quaternion.Inverse(portalGun.rotation) * relativeRotation;
            isPickUp = true;
        }
    }

    /// <summary>
    /// 吸附物体的跟随
    /// </summary>
    private void PickObjectFollowed()
    {
        //Debug.Log($"{isInPortal}");
        //pickedObject.transform.DOMove(portalGun.position + portalGun.TransformDirection(relativePosition), 0.05f);
        if (!isInPortal)
        {
            //对于拾起的物体，我们同样只要旋转y轴，这样会更加直观（传送门中也是这样的）,
            Quaternion _relativeRot = portalGun.rotation * relativeRotation;
            _relativeRot.x = 0f;
            _relativeRot.z = 0f;
            //pickedObject.transform.rotation = _relativeRot;
            rb.rotation = _relativeRot.normalized;

            //当物体刚好被传送到传送门后，这句代码会将其拉回...,故relativePosition需要做到实时修改
            //这样移动的坏处是方块会穿墙，出现抖动现象，直接AddForce很难精确调整方块的位置，所以这里调整方块的速度
            //rb.MovePosition(portalGun.position + portalGun.TransformDirection(relativePosition));
            float _gunXRot = portalGun.eulerAngles.x;
            _gunXRot = _gunXRot - Mathf.RoundToInt(_gunXRot / 360f) * 360;
            Vector3 _velocity;
            if (Mathf.Abs(_gunXRot) > restrictXRotate)
            {
                _gunXRot = _gunXRot > 0 ? restrictXRotate : -restrictXRotate;
                //Quaternion _restrictRot = Quaternion.Euler(_gunXRot, portalGun.rotation.eulerAngles.y, portalGun.rotation.eulerAngles.z);
                //Vector3 _virtualPosition = _restrictRot * new Vector3(0.12f, -0.17f, 0.2f);
                //_virtualPosition = playerCamera.position + _virtualPosition;
                //Vector3 _relativePos = Quaternion.Euler(relativeRotation.eulerAngles) * relativePosition;
                //_virtualPosition += _relativePos;
                //Debug.Log($"1:{_virtualPosition},2:{portalGun.TransformPoint(relativePosition)}");
                Quaternion _defaultCameraRot = playerCamera.transform.rotation;
                playerCamera.rotation = Quaternion.Euler(_gunXRot, playerCamera.rotation.eulerAngles.y, playerCamera.rotation.eulerAngles.z);
                _velocity = (portalGun.TransformPoint(relativePosition) - rb.position) / Time.deltaTime;
                playerCamera.rotation = _defaultCameraRot;
            }
            else
            {
                _velocity = (portalGun.TransformPoint(relativePosition) - rb.position) / Time.deltaTime;
            }
            rb.velocity = _velocity;
            //Debug.Log($"Cube:{portalGun.TransformDirection(relativePosition)}");
        }
        else
        {
            //Vector3 _mirrorPortalGunPos = inPortal.InverseTransformPoint(portalGun.position);
            //_mirrorPortalGunPos = Quaternion.Euler(new Vector3(0, 180, 0)) * _mirrorPortalGunPos;
            //_mirrorPortalGunPos = outPortal.TransformPoint(_mirrorPortalGunPos);

            //relativePosition = inPortal.InverseTransformDirection(relativePosition);
            //relativePosition = Quaternion.Euler(new Vector3(0, 180, 0)) * relativePosition;
            //relativePosition = outPortal.TransformDirection(relativePosition);

            //计算移动到另一个传送门后的旋转
            Quaternion _relativeRot = portalGun.rotation * relativeRotation;
            _relativeRot.x = 0f;
            _relativeRot.z = 0f;
            _relativeRot = Quaternion.Inverse(inPortal.rotation) * _relativeRot;
            _relativeRot = Quaternion.Euler(0, 180, 0) * _relativeRot;
            _relativeRot = outPortal.rotation * _relativeRot;
            //pickedObject.transform.rotation = _relativeRot;
            rb.rotation = _relativeRot.normalized;

            //计算移动到另一个传送门后的位置
            Vector3 _relativePos = portalGun.position + portalGun.TransformDirection(relativePosition);
            _relativePos = inPortal.InverseTransformPoint(_relativePos);
            _relativePos = Quaternion.Euler(new Vector3(0, 180, 0)) * _relativePos;
            rb.MovePosition(outPortal.TransformPoint(_relativePos));
        }
        //Debug.Log($"object:{rb.position}");
        //将此刻刚体的位置传入缓存中
        SetGunVecBuffer(rb.position);
    }

    /// <summary>
    /// 在吸附物体传送时，获取入传送门和出传送门的信息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GetPortalInfo(object sender, EventArgs e)
    {
        var info = e as EventParameter;
        if (info.eventTransforms != null)
        {
            inPortal = info.eventTransforms[0];
            outPortal = info.eventTransforms[1];
            isInPortal = true;
        }
        else
        {
            inPortal = null;
            outPortal = null;
            isInPortal = false;
        }
    }

    /// <summary>
    /// 传送门反射射线
    /// </summary>
    /// <param name="ray">原始射线</param>
    /// <returns>射线反射后是否射到物体</returns>
    private bool PortalRaycast(Ray ray)
    {
        if(hit.collider.CompareTag("BluePortal") || hit.collider.CompareTag("OrangePortal"))
        {
            Transform _hitPortal = hit.transform;
            //射线的剩余距离
            float _remainderDistance = maxPickDistance - hit.distance;
            //得到另一个传送门的Transform的引用
            Transform _anotherPortal = _hitPortal.GetComponent<Portal>()?.anotherPortal.transform;
            //因为传送门的碰撞体是盒型，因此还要将集中的点在传送门上的投影点的局部坐标计算出来
            Vector3 _newRaycastPoint = _hitPortal.InverseTransformPoint(hit.point);
            float _length = Vector3.Dot(_newRaycastPoint, -_hitPortal.forward);
            Vector3 _subVec = -_hitPortal.transform.forward * _length;
            _newRaycastPoint = _newRaycastPoint - _subVec;

            //在投影得出来后将其转换到另一个传送门上
            _newRaycastPoint = Quaternion.Euler(0, 180, 0) * _newRaycastPoint;
            _newRaycastPoint = _anotherPortal.TransformPoint(_newRaycastPoint);
            Debug.Log($"$初始:{hit.point}, 投影:{_newRaycastPoint}");
            //得到射线方向相对于传送门的转置
            Vector3 _newRaycastDirection = _anotherPortal.InverseTransformDirection(ray.direction);
            Debug.Log($"初始向量：{_newRaycastDirection}");
            _newRaycastDirection = Quaternion.Euler(0, 180, 0) * _newRaycastDirection;
            _newRaycastDirection = _anotherPortal.rotation * _newRaycastDirection;
            Debug.Log($"转置向量：{_newRaycastDirection}");

            Ray _relfectRay = new Ray(_newRaycastPoint, _newRaycastDirection);
            //RaycastHit _relfectHit;
            //检测到物体
            if (Physics.Raycast(_relfectRay, out hit, _remainderDistance, ~0))
            {
                if (!hit.collider.CompareTag("PickUpObject"))
                {
                    return false;
                }
                Debug.Log($"成功：{hit.collider.GetInstanceID()}");
                inPortal = _hitPortal;
                outPortal = _anotherPortal;
                isInPortal = true;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 当玩家放下吸附体时，给予吸附体一定的速度
    /// </summary>
    private void SetVelocityAfterDrop()
    {
        Vector3 _addForce = (portalGunVecBuffer[portalGunVecBuffer.Count - 1] - portalGunVecBuffer[0]) / (Time.fixedDeltaTime * 5) * 0.8f;
        _addForce = ClampVector(_addForce, new Vector3(0, 0, 0), new Vector3(5, 5, 5));
        rb.AddForce(_addForce, ForceMode.Impulse);
        Debug.Log($"AddForce:{_addForce}");
    }

    /// <summary>
    /// 记录一定时间内传送枪得位置
    /// </summary>
    /// <param name="_pos"></param>
    private void SetGunVecBuffer(Vector3 _pos)
    {
        if(portalGunVecBuffer.Count < 4)
        {
            portalGunVecBuffer.Add(_pos);
        }
        else
        {
            portalGunVecBuffer.RemoveAt(0);
            portalGunVecBuffer.Add(_pos);
        }
    }

    /// <summary>
    /// 检查传送枪与吸附物体间的角度的差值，当差值达到一定角度时断开连接
    /// </summary>
    private void CheckAngle()
    {
        if (isInPortal)
            return;
        Vector3 _direction = pickedObject.transform.position - portalGun.position;
        float _diffAngle = Vector3.Angle(portalGun.forward, _direction);
        if(Mathf.Abs(_diffAngle) > disconnectAngle)
        {
            disconnectTimer -= Time.fixedDeltaTime;
            if (disconnectTimer > 0)
            {
                disconnectTimer = 0.1f;
                return;            
            }
            Debug.Log($"Disconnect:{_diffAngle}");
            rb.useGravity = true;
            pickedObject = null;
            isPickUp = false;
            disconnectTimer = 0.1f;
        }
    }

    private Vector3 ClampVector(Vector3 _vector3, Vector3 _minVector3, Vector3 _maxVector3)
    {
        _minVector3 = new Vector3(-Mathf.Abs(_minVector3.x), -Mathf.Abs(_minVector3.y), -Mathf.Abs(_minVector3.z));
        _maxVector3 = new Vector3(Mathf.Abs(_maxVector3.x), Mathf.Abs(_maxVector3.y), Mathf.Abs(_maxVector3.z));

        _vector3.x = Mathf.Clamp(_vector3.x, _minVector3.x, _maxVector3.x);
        _vector3.y = Mathf.Clamp(_vector3.y, _minVector3.y, _maxVector3.y);
        _vector3.z = Mathf.Clamp(_vector3.z, _minVector3.z, _maxVector3.z);
        return _vector3;
    }
}
