using JetBrains.Annotations;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class PortalLaunchController : MonoBehaviour
{
    /// <summary>
    /// ���͹��߻��е�������Ϣ
    /// </summary>
    private RaycastHit hit;
    /// <summary>
    /// ���������߼��ĵĲ㼶
    /// </summary>
    [SerializeField] private LayerMask Detectedlayer;
    /// <summary>
    /// �ܲ��������ŵĲ㼶
    /// </summary>
    [SerializeField] private LayerMask enablePortalLayer;
    /// <summary>
    /// �ܹ��赲���������ߵĲ㼶
    /// </summary>
    [SerializeField] private LayerMask resistPortalLayer;
    /// <summary>
    /// ���������ڵĲ㼶
    /// </summary>
    [SerializeField] private LayerMask portalLayer;
    /// <summary>
    /// ������Ԥ����
    /// </summary>
    [SerializeField] private GameObject[] portals = new GameObject[2];
    /// <summary>
    /// ��ɫ������
    /// </summary>
    private GameObject bluePortal;
    /// <summary>
    /// ��ɫ������
    /// </summary>
    private GameObject orangeProtal;
    /// <summary>
    /// ���ɴ����ŵĸ����
    /// </summary>
    [SerializeField] private Transform portalsParent;
    /// <summary>
    /// ����������ǰ��ԭʼλ��
    /// </summary>
    private Vector3 originalPos;
    /// <summary>
    /// ����������ǰ��ԭʼ��ת
    /// </summary>
    private Quaternion orignalRot;
    /// <summary>
    /// ������ײ���id
    /// </summary>
    private int hitCollider;
    /// <summary>
    /// �����ŵ��ĸ��߽��
    /// </summary>
    private Vector3[] portalBounds;
    /// <summary>
    /// �����ŵ��ĸ��ǵ�
    /// </summary>
    private Vector3[] portalProjectBounds;
    /// <summary>
    /// ���������ϵ����������ĸ�����
    /// </summary>
    private Vector3[] portalDirection;
    /// <summary>
    /// �����ŵ��ĸ��ǵ�ָ�����ĵķ���
    /// </summary>
    private Vector3[] portalProjectDirection;
    /// <summary>
    /// �߽��ƫ�Ƽ��ʱ�õ���RaycastHit
    /// </summary>
    private RaycastHit[] hitBound;
    /// <summary>
    /// �߽����ײ����ʱ����ײ�建��
    /// </summary>
    private Collider[] hitBoundCollider;

    private void Awake()
    {
        //��Ϸ��ʼʱ�Ͳ������������ţ��������ƶ�����ҿ������ĵط������������跴��������
        bluePortal = Instantiate(portals[0], portalsParent);
        orangeProtal = Instantiate(portals[1], portalsParent);
        bluePortal.transform.position = new Vector3(0, 0, -1000);
        orangeProtal.transform.position = new Vector3(0, 0, -1000);

        portalBounds = new Vector3[4]
        {
            new Vector3(0.8f, 0f, 0.01f),
            new Vector3(-0.8f, 0f, 0.01f),
            new Vector3(0f, 1.5f, 0.01f),
            new Vector3(0f, -1.5f, 0.01f)
        };

        portalProjectBounds = new Vector3[4]
        {
            new Vector3(0.8f, 1.5f, 0.01f),
            new Vector3(-0.8f, 1.5f, 0.01f),
            new Vector3(0.8f, -1.5f, 0.01f),
            new Vector3(-0.8f, -1.5f, 0.01f)
        };

        portalDirection = new Vector3[4]
        {
            -Vector3.right,
            Vector3.right,
            -Vector3.up,
            Vector3.up
        };

        portalProjectDirection = new Vector3[4]
        {
            new Vector3(-0.8f, -1.5f, 0).normalized,
            new Vector3(0.8f, -1.5f, 0).normalized,
            new Vector3(-0.8f, 1.5f, 0).normalized,
            new Vector3(0.8f, 1.5f, 0).normalized
        };

        hitBoundCollider = new Collider[4];
    }

    private void Start()
    {
        EventManager.Instance.AddListener(EventName.LaunchPortal, LaunchProtal);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.LaunchPortal, LaunchProtal);
    }

    /// <summary>
    /// ���ɴ�����
    /// </summary>
    /// <param name="_blueOrOrange">��ɫ�����Ż��ɫ������</param>
    public void LaunchProtal(object sender, EventArgs e)
    {
        int _blueOrOrange = -1;
        var info = e as EventParameter;
        if (info != null)
            _blueOrOrange = info.eventInt;
        int _layer = Detectedlayer;
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.mousePosition);
        if (Physics.Raycast(ray, out hit, 100f, _layer, QueryTriggerInteraction.Ignore))
        {
            if (((1 << hit.collider.gameObject.layer) & resistPortalLayer) != 0)
            {
                return;
            }
            //���е�
            Vector3 _hitPoint = hit.point;
            //����ƽ��ķ���
            Vector3 _hitNormal = hit.normal;
            //���е���ײ���id
            hitCollider = hit.colliderInstanceID;
            if(Mathf.Abs(_hitNormal.y) <= 0.9)
            {
                FirstStepForGenerate(_blueOrOrange, _hitNormal, _hitPoint);
                if (!CheckForAround(_blueOrOrange))
                {
                    ResetPortal(_blueOrOrange);
                    return;
                }
            }
            else
            {
                //�����ڻ���ƽ����ϵ�ͶӰ
                Vector3 _projection = Vector3.ProjectOnPlane(ray.direction, _hitNormal);
                FirstStepForGenerate(_blueOrOrange, _hitNormal, _hitPoint, _projection);
                if (!CheckForPorjectAround(_blueOrOrange))
                {
                    ResetPortal(_blueOrOrange);
                    return;
                }
            }
            if (CheckForAnother(_blueOrOrange, _hitPoint, _hitNormal))
            {
                ResetPortal(_blueOrOrange);
                return;
            }
            GeneratePortal(_blueOrOrange, _hitNormal);
            #region ����
            ////�����е㸽���Ƿ�����һ��������(����У����޷�����)
            //if (CheckForCurrentPortal(hit, _hitNormal, _hitPoint, _blueOrOrange))
            //    return;
            ////�жϣ����ݻ��е�ƽ�治ͬ�Բ�ͬ��ʽ���ɣ���ԭ���������ʵ�֣�
            ////������е�����ֱƽ�������ɵĴ����Ŷ�����y��ƽ�У���ˮƽƽ�����ⶼ����ƽ������Ĵ�ֱ����ƽ�У�
            ////������е���ˮƽƽ�棬�����������ƽ���ϵ�ͶӰ��������
            //if (Mathf.Abs(_hitNormal.y) > .9f)
            //{
            //    //�����ڻ���ƽ����ϵ�ͶӰ
            //    Vector3 _projection = Vector3.ProjectOnPlane(ray.direction, _hitNormal);
            //    if (CheckForGenerate(hit, _hitPoint, _blueOrOrange))
            //    {
            //        GeneratePortal(_projection, _blueOrOrange, _hitPoint, _hitNormal);
            //    }
            //    else
            //    {
            //        //ƫ�ƻ��е�
            //        _hitPoint = OffsetPoint(hit, _hitPoint, _projection);
            //        //�ٴμ����е㸽���Ƿ�����һ�������ţ���Ϊ����ƫ���˻��е㣬���Ե�ʱ������˴������ص���
            //        if (CheckForCurrentPortal(hit, _hitNormal, _hitPoint, _blueOrOrange))
            //            return;
            //        GeneratePortal(_projection, _blueOrOrange, _hitPoint, _hitNormal);
            //    }
            //}
            //else
            //{
            //    if (CheckForGenerate(hit, _hitPoint, _blueOrOrange))
            //    {
            //        //����������
            //        GeneratePortal(hit.transform.up, _blueOrOrange, _hitPoint, _hitNormal);
            //    }
            //    else
            //    {
            //        //ƫ�ƻ��е�
            //        _hitPoint = OffsetPoint(hit, _hitPoint);
            //        //�ٴμ����е㸽���Ƿ�����һ�������ţ���Ϊ����ƫ���˻��е㣬���Ե�ʱ������˴������ص���
            //        if (CheckForCurrentPortal(hit, _hitNormal, _hitPoint, _blueOrOrange))
            //            return;
            //        GeneratePortal(hit.transform.up, _blueOrOrange, _hitPoint, _hitNormal);
            //    }
            //}
            #endregion
        }
    }

    #region ���÷���(ֻ�����ں�����ײ��)
    ///// <summary>
    ///// ����Ƿ�����ײ����Ϊ�������ɴ�����
    ///// </summary>
    ///// <param name="_hit">ײ����hit��Ϣ</param>
    ///// <param name="_hitPoint">ײ����λ��</param>
    ///// <param name="_id">������id</param>
    ///// <returns></returns>
    //private bool CheckForGenerate(RaycastHit _hit, Vector3 _hitPoint, int _id)
    //{
    //    //�ʼ�ķ�������bounds����⣬���ǲ��Ժ��ֵ���ײ����x��z��תʱ������bounds��ֻ��y��ת�ģ����Ի��������������ײ����ȴ���е����
    //    ////�����ŵ��ĸ��߽��
    //    //Vector3 _vec0 =  _hitPoint + _hit.transform.right * 0.5f;
    //    //Vector3 _vec1 = _hitPoint - _hit.transform.right * 0.5f;
    //    //Vector3 _vec2 = _hitPoint - _hit.transform.up * 1f;
    //    //Vector3 _vec3 = _hitPoint + _hit.transform.up * 1f;
    //    ////����ĸ��߽���Ƿ�����ײ����(��Χ��)
    //    //if(_hit.collider.bounds.Contains(_vec0) && _hit.collider.bounds.Contains(_vec1)
    //    //    && _hit.collider.bounds.Contains(_vec2) && _hit.collider.bounds.Contains(_vec3))
    //    //{
    //    //    return true;
    //    //}

    //    //�����ŵ��ĸ��߽��
    //    Vector3 _vec0 = _hitPoint + _hit.transform.right * 0.5f;
    //    Vector3 _vec1 = _hitPoint - _hit.transform.right * 0.5f;
    //    Vector3 _vec2 = _hitPoint - _hit.transform.up * 1f;
    //    Vector3 _vec3 = _hitPoint + _hit.transform.up * 1f;
    //    if (isContains(_hit, _vec0) && isContains(_hit, _vec1) && isContains(_hit, _vec2) && isContains(_hit, _vec3))
    //        return true;
    //    return false;
    //}
    ///// <summary>
    ///// ���Ƿ�����ײ����
    ///// </summary>
    ///// <param name="_hit"></param>
    ///// <param name="_hitPoint"></param>
    ///// <returns></returns>
    //private bool isContains(RaycastHit _hit, Vector3 _hitPoint)
    //{
    //    //��������
    //    Vector3 _distance = _hitPoint - _hit.transform.position;
    //    Quaternion _rotation = _hit.transform.rotation;
    //    _rotation = Quaternion.Inverse(_rotation);
    //    //ת��Ϊ������ײ������ĵ���������
    //    Vector3 _newWorldPosition = _rotation * _distance + _hit.transform.position;
    //    Vector3 _center = _hit.transform.position;
    //    //�������ĵ㵽ÿ������߽�İ뾶
    //    Vector3 _halfSize = _hit.transform.lossyScale * 0.5f;
    //    Vector3 _max = _center + _halfSize;
    //    Vector3 _min = _center - _halfSize;
    //    if (ApproximatelySub(_max.x, _newWorldPosition.x) && ApproximatelySub(_max.y, _newWorldPosition.y)
    //        && ApproximatelySub(_max.z, _newWorldPosition.z) && ApproximatelySub(_newWorldPosition.x, _min.x)
    //        && ApproximatelySub(_newWorldPosition.y, _min.y) && ApproximatelySub(_newWorldPosition.z, _min.z))
    //        return true;
    //    return false;
    //}
    ///// <summary>
    ///// ����������֮���Ƿ�������ڣ�Լ���ڣ�����������У�
    ///// </summary>
    ///// <param name="_num1"></param>
    ///// <param name="_num2"></param>
    ///// <returns></returns>
    //private bool ApproximatelySub(float _num1, float _num2)
    //{
    //    if ((_num1 - _num2) > 0 || Mathf.Abs(_num1 - _num2) < 0.01f)
    //        return true;
    //    return false;
    //}

    ///// <summary>
    ///// �жϻ��е㸽���Ƿ�����һ��������
    ///// </summary>
    ///// <param name="_hit">���е��hit</param>
    ///// <param name="_hitPoint">���е�����</param>
    ///// <param name="_id">������id</param>
    ///// <returns></returns>
    //private bool CheckForCurrentPortal(RaycastHit _hit, Vector3 _hitNormal, Vector3 _hitPoint, int _id)
    //{
    //    //����һ�������ŵ�id
    //    int _otherId = _id == 0 ? 1 : 0;
    //    Vector3 _distance;
    //    //�жϴ������Ƿ��ѡ�������
    //    if (_otherId == 0 && bluePortal.GetComponent<Portal>().isPlaced)
    //    {
    //        //����һ��������е�ľ�������
    //        _distance = bluePortal.transform.position - _hitPoint;

    //    }
    //    else if (_otherId == 1 && orangeProtal.GetComponent<Portal>().isPlaced)
    //    {
    //        _distance = orangeProtal.transform.position - _hitPoint;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //    //�������������ֱ�ڻ���ƽ��ķ��򣬿�����Ϊ���е����һ������������ͬһ��ƽ���ϵ�
    //    if (Vector3.Dot(_distance, _hitNormal) < 0.02f)
    //    {
    //        //������������ڻ���ƽ�������x��y���ϵ�ͶӰ
    //        float _rightProjection = Vector3.Dot(_distance, _hit.transform.right);
    //        float _upProjection = Vector3.Dot(_distance, _hit.transform.up);
    //        //���ھ��η�Χ�ڣ����ؼ٣�

    //        if (Mathf.Abs(_rightProjection) > 1.25f || Mathf.Abs(_upProjection) > 2.5f)
    //            return false;
    //        else
    //            return true;
    //    }
    //    else
    //        return false;
    //}

    ///// <summary>
    ///// ����������
    ///// </summary>
    ///// <param name="_id">������id</param>
    ///// <param name="_hitPoint">ײ����</param>
    ///// <param name="_normal">ײ��ƽ�淨��</param>
    //private void GeneratePortal(Vector3 _hitUp, int _id, Vector3 _hitPoint, Vector3 _normal)
    //{
    //    if (_id == 0)
    //    {
    //        //���㴫���ŵ���ת����
    //        Quaternion _rotation = Quaternion.LookRotation(-_normal, _hitUp);
    //        ////����������
    //        //GameObject newPortal = Instantiate(portals[0], _hitPoint, Quaternion.identity);
    //        //�ƶ�����ת������
    //        bluePortal.transform.position = _hitPoint;
    //        bluePortal.transform.rotation = _rotation;
    //        //�������Ĵ����ŵ�λ�ü���һ��΢С������ƽ�淨���ϵ�ֵ����û�мӵ�ʱ������z-Fighting��
    //        bluePortal.transform.position += _normal * 0.01f;
    //        //��ɫ�������Ƿ������Ϊ��
    //        bluePortal.GetComponent<Portal>().isPlaced = true;
    //        //���������������������ײ�崫��
    //        bluePortal.GetComponent<Portal>().SetAttachCollider(hit.collider);
    //    }
    //    else if (_id == 1)
    //    {
    //        //���㴫���ŵ���ת����
    //        Quaternion _rotation = Quaternion.LookRotation(-_normal, _hitUp);
    //        ////����������
    //        //GameObject newPortal = Instantiate(portals[0], _hitPoint, Quaternion.identity);
    //        //�ƶ�����ת������
    //        orangeProtal.transform.position = _hitPoint;
    //        orangeProtal.transform.rotation = _rotation;
    //        //�������Ĵ����ŵ�λ�ü���һ��΢С������ƽ�淨���ϵ�ֵ����û�мӵ�ʱ������z-Fighting��
    //        orangeProtal.transform.position += _normal * 0.01f;
    //        //��ɫ�������Ƿ������Ϊ��
    //        orangeProtal.GetComponent<Portal>().isPlaced = true;
    //        //���������������������ײ�崫��
    //        orangeProtal.GetComponent<Portal>().SetAttachCollider(hit.collider);
    //    }
    //}
    ///// <summary>
    ///// ƫ�ƻ��е�
    ///// </summary>
    ///// <param name="_hit">���е��hit��Ϣ</param>
    ///// <param name="_hitPoint">���е�����</param>
    ///// <returns></returns>
    //private Vector3 OffsetPoint(RaycastHit _hit, Vector3 _hitPoint)
    //{
    //    //���е������ƽ�����ĵľ���ʸ��
    //    Vector3 _distance = _hit.transform.position - _hitPoint;
    //    //����ʸ���ڻ���ƽ�������x��y���ͶӰ
    //    float _xProjection = Vector3.Dot(_distance, _hit.transform.right);
    //    float _yProjection = Vector3.Dot(_distance, _hit.transform.up);
    //    //x,y�ϵ���߽�Ĳ�ֵ
    //    float _xOffset = _hit.transform.lossyScale.x / 2 - Mathf.Abs(_xProjection);
    //    float _yOffset = _hit.transform.lossyScale.y / 2 - Mathf.Abs(_yProjection);
    //    float _newAddX = 0;
    //    float _newAddy = 0;
    //    if (_xOffset <= 0.7f)
    //        _newAddX = _xProjection < 0 ? _xOffset - 0.8f : 0.8f - _xOffset;
    //    if (_yOffset <= 1.4)
    //        _newAddy = _yProjection < 0 ? _yOffset - 1.4f : 1.4f - _yOffset;
    //    //�µ����ĵ������
    //    Vector3 _newHitPoint = _hitPoint + _hit.transform.right * _newAddX + _hit.transform.up * _newAddy;
    //    return _newHitPoint;
    //}
    ///// <summary>
    ///// ƫ�ƻ��е�
    ///// </summary>
    ///// <param name="_hit">���е�hit��Ϣ</param>
    ///// <param name="_hitPoint">���е�����</param>
    ///// <param name="_projectVec">����ͶӰ</param>
    ///// <returns></returns>
    //private Vector3 OffsetPoint(RaycastHit _hit, Vector3 _hitPoint, Vector3 _projectVec)
    //{
    //    //ͶӰ������right��up��ļн�
    //    float _angleRight = Vector3.Angle(_projectVec, _hit.transform.right);
    //    float _angleUp = Vector3.Angle(_projectVec, _hit.transform.up);
    //    //ǡ��������ʱ�����ĵ����߽���ľ���
    //    float _rightDistance = 1.0f * Mathf.Abs(Mathf.Cos(_angleRight)) + 0.5f * Mathf.Abs(Mathf.Sin(_angleRight));
    //    float _upDistance = 1.0f * Mathf.Abs(Mathf.Cos(_angleUp)) + 0.5f * Mathf.Abs(Mathf.Sin(_angleUp));
    //    //���е������ƽ�����ĵľ���ʸ��
    //    Vector3 _distance = _hit.transform.position - _hitPoint;
    //    //����ʸ���ڻ���ƽ�������x��y���ͶӰ
    //    float _xProjection = Vector3.Dot(_distance, _hit.transform.right);
    //    float _yProjection = Vector3.Dot(_distance, _hit.transform.up);
    //    //x,y�ϵ���߽�Ĳ�ֵ
    //    float _xOffset = _hit.transform.lossyScale.x / 2 - Mathf.Abs(_xProjection);
    //    float _yOffset = _hit.transform.lossyScale.y / 2 - Mathf.Abs(_yProjection);
    //    float _newAddX = 0;
    //    float _newAddY = 0;
    //    if (_xOffset < _rightDistance)
    //        _newAddX = _xProjection < 0 ? _xOffset - _rightDistance - 0.1f : _rightDistance + 0.1f - _xOffset;
    //    if (_yOffset < _upDistance)
    //        _newAddY = _yProjection < 0 ? _yOffset - _upDistance - 0.1f : _upDistance + 0.1f - _yOffset;
    //    Vector3 _newHitPoint = _hitPoint + _hit.transform.right * _newAddX + _hit.transform.up * _newAddY;
    //    return _newHitPoint;
    //}
    #endregion

    /// <summary>
    /// ��һ�����Ƚ��������Ƶ�����λ��
    /// </summary>
    /// <param name="_id">������id</param>
    /// <param name="_hitNormal">���з���</param>
    /// <param name="_hitPoint">���е�</param>
    private void FirstStepForGenerate(int _id, Vector3 _hitNormal, Vector3 _hitPoint)
    {
        //���ƽ���Right��
        Vector3 _hitRight = Vector3.Cross(Vector3.up, _hitNormal);
        //���������ƽ���Up��
        Vector3 _hitUp = Vector3.Cross(_hitNormal, _hitRight);
        //����������ŵĳ���
        Quaternion _portalRot = Quaternion.LookRotation(-_hitNormal, _hitUp);
        //�ж����ĸ�������
        var _portal = _id == 0 ? bluePortal : orangeProtal;
        //��¼ԭ�ȵ�λ�ú���ת�������޷�����ʱ����
        originalPos = _portal.transform.position;
        orignalRot = _portal.transform.rotation;
        //�Ƚ��������Ƶ���Ӧλ�ò���ת��֮����к�������
        _portal.transform.position = _hitPoint;
        _portal.transform.rotation = _portalRot;
    }

    /// <summary>
    /// ��һ�����Ƚ��������Ƶ�����λ��
    /// </summary>
    /// <param name="_id">������id</param>
    /// <param name="_hitNormal">���з���</param>
    /// <param name="_hitPoint">���е�</param>
    /// <param name="_hitUp">����Up��</param>
    private void FirstStepForGenerate(int _id, Vector3 _hitNormal, Vector3 _hitPoint, Vector3 _hitUp)
    {
        //����������ŵĳ���
        Quaternion _portalRot = Quaternion.LookRotation(-_hitNormal, _hitUp);
        //�ж����ĸ�������
        var _portal = _id == 0 ? bluePortal : orangeProtal;
        //��¼ԭ�ȵ�λ�ú���ת�������޷�����ʱ����
        originalPos = _portal.transform.position;
        orignalRot = _portal.transform.rotation;
        //�Ƚ��������Ƶ���Ӧλ�ò���ת��֮����к�������
        _portal.transform.position = _hitPoint;
        _portal.transform.rotation = _portalRot;
    }

    /// <summary>
    /// ��鴫���ŵı߽��Ƿ񳬳�ǽ��ı߽磬����ǣ���ƫ��
    /// </summary>
    /// <param name="_id">������id</param>
    private bool CheckForAround(int _id)
    {
        //�õ����ɴ����ŵ�id
        var _portalTransform = _id == 0 ? bluePortal.transform : orangeProtal.transform;
        //����ƫ�Ƽ�����ߵ����
        Vector3 _raycastPos;
        //����ƫ�Ƽ�����ߵķ���
        Vector3 _raycastDir;
        for(int i = 0; i < 4; i++)
        {
            //�������ŵ��ĸ��߽��ת������������ϵ
            _raycastPos = _portalTransform.TransformPoint(portalBounds[i]);
            //������������ĸ�����ת������������ϵ
            _raycastDir = _portalTransform.TransformDirection(portalDirection[i]);
            //���ĸ��߽���ϼ���Ƿ�����ײ��
            int _colliderCount = Physics.OverlapSphereNonAlloc(_raycastPos, 0.03f, hitBoundCollider, enablePortalLayer);
            //����⵽�߽������ײ����ʱ��ֱ��Continue
            bool isSkip = false;
            if (_colliderCount > 0)
            {
                for(int j = 0; j < _colliderCount; j++)
                {
                    //ֻ����ײ���Ǵ����������ŵ���ײ�����
                    if (hitBoundCollider[j].GetInstanceID() == hitCollider)
                        isSkip = true;
                }
                if (isSkip)
                {
                    continue;
                }
            }

            //�ӱ߽�㷢������
            hitBound = Physics.RaycastAll(_raycastPos, _raycastDir, 1.5f, enablePortalLayer);
            if (hitBound.Length > 0)
            {
                for(int j = 0; j < hitBound.Length; j++)
                {
                    //�����߼�⵽�����Ÿ��ű߽��ʱ
                    if (hitBound[j].colliderInstanceID == hitCollider)
                    {
                        //�����߽���ƫ����
                        Vector3 _offset = hitBound[j].point - _raycastPos;
                        _portalTransform.Translate(_offset, Space.World);
                        break;
                    }
                }
            }
            //����߽�㼴������ײ�����ֲ���ƫ�ƣ��򷵻�false
            else
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckForPorjectAround(int _id)
    {
        //�õ����ɴ����ŵ�id
        var _portalTransform = _id == 0 ? bluePortal.transform : orangeProtal.transform;
        //����ƫ�Ƽ�����ߵ����
        Vector3 _raycastPos;
        //����ƫ�Ƽ�����ߵķ���
        Vector3 _raycastDir;
        //���յ�ƫ����
        Vector3 _offset = Vector3.zero;
        for(int i = 0; i < 4; i++)
        {
            _raycastPos = _portalTransform.TransformPoint(portalProjectBounds[i]);
            _raycastDir = _portalTransform.TransformDirection(portalProjectDirection[i]);
            //���ĸ��ǵ��ϼ���Ƿ�����ײ��
            int _colliderCount = Physics.OverlapSphereNonAlloc(_raycastPos, 0.05f, hitBoundCollider, enablePortalLayer);
            //����⵽�ǵ�����ײ����ʱ��ֱ��Continue
            bool isSkip = false;
            if (_colliderCount > 0)
            {
                for (int j = 0; j < _colliderCount; j++)
                {
                    //ֻ����ײ���Ǵ����������ŵ���ײ�����
                    if (hitBoundCollider[j].GetInstanceID() == hitCollider)
                        isSkip = true;
                }
                if (isSkip)
                    continue;
            }
            //�ӱ߽�㷢������
            hitBound = Physics.RaycastAll(_raycastPos, _raycastDir, 1.5f, enablePortalLayer);
            if (hitBound.Length > 0)
            {
                for (int j = 0; j < hitBound.Length; j++)
                {
                    //�����߼�⵽�����Ÿ��ű߽��ʱ
                    if (hitBound[j].colliderInstanceID == hitCollider)
                    {
                        //�����߽���ƫ����
                        //if(_offset.sqrMagnitude < (hitBound[j].point - _raycastPos).sqrMagnitude)
                        _offset = hitBound[j].point - _raycastPos;
                        _portalTransform.Translate(_offset, Space.World);
                        break;
                    }
                }
            }
            //����߽�㼴������ײ�����ֲ���ƫ�ƣ��򷵻�false
            else
            {
                return false;
            }
        }
        //Debug.Log($"offset:{_offset}");
        return true;
    }

    /// <summary>
    /// ����������
    /// </summary>
    /// <param name="_id">������id</param>
    /// <param name="_hitPoint">ײ����</param>
    /// <param name="_normal">ײ��ƽ�淨��</param>
    private void GeneratePortal(int _id, Vector3 _normal)
    {
        var _portal = _id == 0 ? bluePortal : orangeProtal;
        _portal.transform.position += _normal * 0.001f;
        _portal.GetComponent<Portal>().isPlaced = true;
        _portal.GetComponent<Portal>().SetAttachCollider(hit.collider);
        //if (_id == 0)
        //{
        //    //�������Ĵ����ŵ�λ�ü���һ��΢С������ƽ�淨���ϵ�ֵ����û�мӵ�ʱ������z-Fighting��
        //    bluePortal.transform.position += _normal * 0.01f;
        //    //��ɫ�������Ƿ������Ϊ��
        //    bluePortal.GetComponent<Portal>().isPlaced = true;
        //    //���������������������ײ�崫��
        //    bluePortal.GetComponent<Portal>().SetAttachCollider(hit.collider);
        //}
        //else if (_id == 1)
        //{
        //    //�������Ĵ����ŵ�λ�ü���һ��΢С������ƽ�淨���ϵ�ֵ����û�мӵ�ʱ������z-Fighting��
        //    orangeProtal.transform.position += _normal * 0.01f;
        //    //��ɫ�������Ƿ������Ϊ��
        //    orangeProtal.GetComponent<Portal>().isPlaced = true;
        //    //���������������������ײ�崫��
        //    orangeProtal.GetComponent<Portal>().SetAttachCollider(hit.collider);
        //}
    }

    /// <summary>
    /// �����е㸽���Ƿ�����һ��������
    /// </summary>
    /// <param name="_id"></param>
    /// <param name="_hitPoint"></param>
    /// <param name="_hitNormal"></param>
    /// <returns></returns>
    private bool CheckForAnother(int _id, Vector3 _hitPoint, Vector3 _hitNormal)
    {
        //����һ�������ŵ�id
        int _otherId = _id == 0 ? 1 : 0;
        var _portalTransform = _id == 0 ? bluePortal.transform : orangeProtal.transform;
        string _anotherTag;
        Vector3 _distance;
        //�жϴ������Ƿ��ѡ�������
        if (_otherId == 0 && bluePortal.GetComponent<Portal>().isPlaced)
        {
            //����һ��������е�ľ�������
            _distance = bluePortal.transform.position - _hitPoint;
            _anotherTag = bluePortal.tag;
        }
        else if (_otherId == 1 && orangeProtal.GetComponent<Portal>().isPlaced)
        {
            _distance = orangeProtal.transform.position - _hitPoint;
            _anotherTag = orangeProtal.tag;
        }
        else
        {
            return false;
        }
        //�������������ֱ�ڻ���ƽ��ķ��򣬿�����Ϊ���е����һ������������ͬһ��ƽ���ϵ�
        if (Vector3.Dot(_distance, _hitNormal) < 0.01f)
        {
            ////������������ڻ���ƽ�������x��y���ϵ�ͶӰ
            //float _rightProjection = Vector3.Dot(_distance, hit.transform.right);
            //float _upProjection = Vector3.Dot(_distance, hit.transform.up);
            ////���ھ��η�Χ�ڣ����ؼ٣�
            //if (Mathf.Abs(_rightProjection) > 1.25f || Mathf.Abs(_upProjection) > 2.5f)
            //    return false;
            //else
            //    return true;

            //ͨ�����߼���жϻ��е�ĸ����Ƿ�����һ��������
            Vector3 _raycastPos;
            for(int i = 0; i < 4; i++)
            {
                _raycastPos = _portalTransform.TransformPoint(portalBounds[i]);
                int _hitCount = Physics.OverlapSphereNonAlloc(_raycastPos, 0.05f, hitBoundCollider, portalLayer);
                for(int j = 0; j < _hitCount; j++)
                {
                    if (hitBoundCollider[j].CompareTag(_anotherTag))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    private void ResetPortal(int _id)
    {
        var _portalTransform = _id == 0 ? bluePortal.transform : orangeProtal.transform;
        _portalTransform.position = originalPos;
        _portalTransform.rotation = orignalRot;
    }
}
