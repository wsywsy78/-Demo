using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class PickObjectController : MonoBehaviour
{
    /// <summary>
    /// ����
    /// </summary>
    public static PickObjectController Instance;
    /// <summary>
    /// ���
    /// </summary>
    [SerializeField] private Transform player;
    /// <summary>
    /// ����ǹ
    /// </summary>
    [SerializeField] private Transform portalGun;
    /// <summary>
    /// ����ǹλ�û���
    /// </summary>
    private List<Vector3> portalGunVecBuffer = new List<Vector3>();
    /// <summary>
    /// ��ʰȡ��Ʒ��������
    /// </summary>
    [Range(0f, 3f)]
    [SerializeField] private float maxPickDistance;
    /// <summary>
    /// �����������Ϣ
    /// </summary>
    private RaycastHit hit;
    /// <summary>
    /// �Ѿ�ʰȡ����Ʒ
    /// </summary>
    public GameObject pickedObject { get; private set; }
    /// <summary>
    /// �Ƿ�����������
    /// </summary>
    private bool isPickUp;
    /// <summary>
    /// ������������ڴ���ǹ��λ��
    /// </summary>
    [SerializeField] private Vector3 relativePosition;
    /// <summary>
    /// ������������ڴ���ǹ����ת
    /// </summary>
    [SerializeField] private Quaternion relativeRotation;
    /// <summary>
    /// ��������ĸ���
    /// </summary>
    private Rigidbody rb;
    /// <summary>
    /// ���������Ƿ��ڴ�������
    /// </summary>
    public bool isInPortal { get; private set; } = false;
    /// <summary>
    /// �봫����
    /// </summary>
    private Transform inPortal;
    /// <summary>
    /// ��������
    /// </summary>
    private Transform outPortal;
    /// <summary>
    /// �����
    /// </summary>
    private Transform playerCamera;
    /// <summary>
    /// ����ĽǶ����Ʒ�Χ����ֹ������ڷ����ϣ�
    /// </summary>
    [SerializeField] private float restrictXRotate = 45f;
    /// <summary>
    /// �����봫��ǹ�ǶȵĲ�ֵ�޶ȣ������˻�Ͽ���
    /// </summary>
    [SerializeField] private float disconnectAngle = 30;
    /// <summary>
    /// �Ͽ��������ӵ�ʱ��
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
    /// ʰȡ������һ������
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void PickUpObject(object sender, EventArgs e)
    {
        //����Ѿ������ˣ������
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
            //������е�����Tag����PickUpObject�������е����岻�Ǵ����Ż����Ǵ����ŵ��Ǿ�"�����"û�л���PickUpObject,����
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
    /// ��������ĸ���
    /// </summary>
    private void PickObjectFollowed()
    {
        //Debug.Log($"{isInPortal}");
        //pickedObject.transform.DOMove(portalGun.position + portalGun.TransformDirection(relativePosition), 0.05f);
        if (!isInPortal)
        {
            //����ʰ������壬����ͬ��ֻҪ��תy�ᣬ���������ֱ�ۣ���������Ҳ�������ģ�,
            Quaternion _relativeRot = portalGun.rotation * relativeRotation;
            _relativeRot.x = 0f;
            _relativeRot.z = 0f;
            //pickedObject.transform.rotation = _relativeRot;
            rb.rotation = _relativeRot.normalized;

            //������պñ����͵������ź�������Ὣ������...,��relativePosition��Ҫ����ʵʱ�޸�
            //�����ƶ��Ļ����Ƿ���ᴩǽ�����ֶ�������ֱ��AddForce���Ѿ�ȷ���������λ�ã������������������ٶ�
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

            //�����ƶ�����һ�������ź����ת
            Quaternion _relativeRot = portalGun.rotation * relativeRotation;
            _relativeRot.x = 0f;
            _relativeRot.z = 0f;
            _relativeRot = Quaternion.Inverse(inPortal.rotation) * _relativeRot;
            _relativeRot = Quaternion.Euler(0, 180, 0) * _relativeRot;
            _relativeRot = outPortal.rotation * _relativeRot;
            //pickedObject.transform.rotation = _relativeRot;
            rb.rotation = _relativeRot.normalized;

            //�����ƶ�����һ�������ź��λ��
            Vector3 _relativePos = portalGun.position + portalGun.TransformDirection(relativePosition);
            _relativePos = inPortal.InverseTransformPoint(_relativePos);
            _relativePos = Quaternion.Euler(new Vector3(0, 180, 0)) * _relativePos;
            rb.MovePosition(outPortal.TransformPoint(_relativePos));
        }
        //Debug.Log($"object:{rb.position}");
        //���˿̸����λ�ô��뻺����
        SetGunVecBuffer(rb.position);
    }

    /// <summary>
    /// ���������崫��ʱ����ȡ�봫���źͳ������ŵ���Ϣ
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
    /// �����ŷ�������
    /// </summary>
    /// <param name="ray">ԭʼ����</param>
    /// <returns>���߷�����Ƿ��䵽����</returns>
    private bool PortalRaycast(Ray ray)
    {
        if(hit.collider.CompareTag("BluePortal") || hit.collider.CompareTag("OrangePortal"))
        {
            Transform _hitPortal = hit.transform;
            //���ߵ�ʣ�����
            float _remainderDistance = maxPickDistance - hit.distance;
            //�õ���һ�������ŵ�Transform������
            Transform _anotherPortal = _hitPortal.GetComponent<Portal>()?.anotherPortal.transform;
            //��Ϊ�����ŵ���ײ���Ǻ��ͣ���˻�Ҫ�����еĵ��ڴ������ϵ�ͶӰ��ľֲ�����������
            Vector3 _newRaycastPoint = _hitPortal.InverseTransformPoint(hit.point);
            float _length = Vector3.Dot(_newRaycastPoint, -_hitPortal.forward);
            Vector3 _subVec = -_hitPortal.transform.forward * _length;
            _newRaycastPoint = _newRaycastPoint - _subVec;

            //��ͶӰ�ó�������ת������һ����������
            _newRaycastPoint = Quaternion.Euler(0, 180, 0) * _newRaycastPoint;
            _newRaycastPoint = _anotherPortal.TransformPoint(_newRaycastPoint);
            Debug.Log($"$��ʼ:{hit.point}, ͶӰ:{_newRaycastPoint}");
            //�õ����߷�������ڴ����ŵ�ת��
            Vector3 _newRaycastDirection = _anotherPortal.InverseTransformDirection(ray.direction);
            Debug.Log($"��ʼ������{_newRaycastDirection}");
            _newRaycastDirection = Quaternion.Euler(0, 180, 0) * _newRaycastDirection;
            _newRaycastDirection = _anotherPortal.rotation * _newRaycastDirection;
            Debug.Log($"ת��������{_newRaycastDirection}");

            Ray _relfectRay = new Ray(_newRaycastPoint, _newRaycastDirection);
            //RaycastHit _relfectHit;
            //��⵽����
            if (Physics.Raycast(_relfectRay, out hit, _remainderDistance, ~0))
            {
                if (!hit.collider.CompareTag("PickUpObject"))
                {
                    return false;
                }
                Debug.Log($"�ɹ���{hit.collider.GetInstanceID()}");
                inPortal = _hitPortal;
                outPortal = _anotherPortal;
                isInPortal = true;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// ����ҷ���������ʱ������������һ�����ٶ�
    /// </summary>
    private void SetVelocityAfterDrop()
    {
        Vector3 _addForce = (portalGunVecBuffer[portalGunVecBuffer.Count - 1] - portalGunVecBuffer[0]) / (Time.fixedDeltaTime * 5) * 0.8f;
        _addForce = ClampVector(_addForce, new Vector3(0, 0, 0), new Vector3(5, 5, 5));
        rb.AddForce(_addForce, ForceMode.Impulse);
        Debug.Log($"AddForce:{_addForce}");
    }

    /// <summary>
    /// ��¼һ��ʱ���ڴ���ǹ��λ��
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
    /// ��鴫��ǹ�����������ĽǶȵĲ�ֵ������ֵ�ﵽһ���Ƕ�ʱ�Ͽ�����
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
