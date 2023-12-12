using JetBrains.Annotations;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class PortalLaunchController : MonoBehaviour
{
    /// <summary>
    /// 传送光线击中的物体信息
    /// </summary>
    private RaycastHit hit;
    /// <summary>
    /// 传送门射线检测的的层级
    /// </summary>
    [SerializeField] private LayerMask Detectedlayer;
    /// <summary>
    /// 能产生传送门的层级
    /// </summary>
    [SerializeField] private LayerMask enablePortalLayer;
    /// <summary>
    /// 能够阻挡传送门射线的层级
    /// </summary>
    [SerializeField] private LayerMask resistPortalLayer;
    /// <summary>
    /// 传送门所在的层级
    /// </summary>
    [SerializeField] private LayerMask portalLayer;
    /// <summary>
    /// 传送门预制体
    /// </summary>
    [SerializeField] private GameObject[] portals = new GameObject[2];
    /// <summary>
    /// 蓝色传送门
    /// </summary>
    private GameObject bluePortal;
    /// <summary>
    /// 橙色传送门
    /// </summary>
    private GameObject orangeProtal;
    /// <summary>
    /// 生成传送门的父组件
    /// </summary>
    [SerializeField] private Transform portalsParent;
    /// <summary>
    /// 产生传送门前的原始位置
    /// </summary>
    private Vector3 originalPos;
    /// <summary>
    /// 产生传送门前的原始旋转
    /// </summary>
    private Quaternion orignalRot;
    /// <summary>
    /// 击中碰撞体的id
    /// </summary>
    private int hitCollider;
    /// <summary>
    /// 传送门的四个边界点
    /// </summary>
    private Vector3[] portalBounds;
    /// <summary>
    /// 传送门的四个角点
    /// </summary>
    private Vector3[] portalProjectBounds;
    /// <summary>
    /// 传送门面上的上下左右四个方向
    /// </summary>
    private Vector3[] portalDirection;
    /// <summary>
    /// 传送门的四个角点指向中心的方向
    /// </summary>
    private Vector3[] portalProjectDirection;
    /// <summary>
    /// 边界点偏移检测时用到的RaycastHit
    /// </summary>
    private RaycastHit[] hitBound;
    /// <summary>
    /// 边界点碰撞体检测时的碰撞体缓存
    /// </summary>
    private Collider[] hitBoundCollider;

    private void Awake()
    {
        //游戏开始时就产生两个传送门，将它们移动到玩家看不到的地方，这样就无需反复创建了
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
    /// 生成传送门
    /// </summary>
    /// <param name="_blueOrOrange">蓝色传送门或橙色传送门</param>
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
            //击中点
            Vector3 _hitPoint = hit.point;
            //击中平面的法向
            Vector3 _hitNormal = hit.normal;
            //击中点碰撞体的id
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
                //射线在击中平面的上的投影
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
            #region 弃用
            ////检查击中点附近是否有另一个传送门(如果有，则无法产生)
            //if (CheckForCurrentPortal(hit, _hitNormal, _hitPoint, _blueOrOrange))
            //    return;
            ////判断，根据击中的平面不同以不同方式生成（还原传送门里的实现）
            ////如果击中的是竖直平面则生成的传送门都会与y轴平行（除水平平面以外都是与平面自身的垂直方向平行）
            ////如果击中的是水平平面，则根据射线在平面上的投影决定方向
            //if (Mathf.Abs(_hitNormal.y) > .9f)
            //{
            //    //射线在击中平面的上的投影
            //    Vector3 _projection = Vector3.ProjectOnPlane(ray.direction, _hitNormal);
            //    if (CheckForGenerate(hit, _hitPoint, _blueOrOrange))
            //    {
            //        GeneratePortal(_projection, _blueOrOrange, _hitPoint, _hitNormal);
            //    }
            //    else
            //    {
            //        //偏移击中点
            //        _hitPoint = OffsetPoint(hit, _hitPoint, _projection);
            //        //再次检查击中点附近是否有另一个传送门（因为这里偏移了击中点，测试的时候出现了传送门重叠）
            //        if (CheckForCurrentPortal(hit, _hitNormal, _hitPoint, _blueOrOrange))
            //            return;
            //        GeneratePortal(_projection, _blueOrOrange, _hitPoint, _hitNormal);
            //    }
            //}
            //else
            //{
            //    if (CheckForGenerate(hit, _hitPoint, _blueOrOrange))
            //    {
            //        //产生传送门
            //        GeneratePortal(hit.transform.up, _blueOrOrange, _hitPoint, _hitNormal);
            //    }
            //    else
            //    {
            //        //偏移击中点
            //        _hitPoint = OffsetPoint(hit, _hitPoint);
            //        //再次检查击中点附近是否有另一个传送门（因为这里偏移了击中点，测试的时候出现了传送门重叠）
            //        if (CheckForCurrentPortal(hit, _hitNormal, _hitPoint, _blueOrOrange))
            //            return;
            //        GeneratePortal(hit.transform.up, _blueOrOrange, _hitPoint, _hitNormal);
            //    }
            //}
            #endregion
        }
    }

    #region 弃用方法(只适用于盒型碰撞体)
    ///// <summary>
    ///// 检查是否能以撞击点为中心生成传送门
    ///// </summary>
    ///// <param name="_hit">撞击点hit信息</param>
    ///// <param name="_hitPoint">撞击点位置</param>
    ///// <param name="_id">传送门id</param>
    ///// <returns></returns>
    //private bool CheckForGenerate(RaycastHit _hit, Vector3 _hitPoint, int _id)
    //{
    //    //最开始的方法，用bounds来检测，但是测试后发现当碰撞体绕x、z旋转时，由于bounds是只绕y轴转的，所以会出现明明不在碰撞体内却误判的情况
    //    ////传送门的四个边界点
    //    //Vector3 _vec0 =  _hitPoint + _hit.transform.right * 0.5f;
    //    //Vector3 _vec1 = _hitPoint - _hit.transform.right * 0.5f;
    //    //Vector3 _vec2 = _hitPoint - _hit.transform.up * 1f;
    //    //Vector3 _vec3 = _hitPoint + _hit.transform.up * 1f;
    //    ////检测四个边界点是否在碰撞体内(包围盒)
    //    //if(_hit.collider.bounds.Contains(_vec0) && _hit.collider.bounds.Contains(_vec1)
    //    //    && _hit.collider.bounds.Contains(_vec2) && _hit.collider.bounds.Contains(_vec3))
    //    //{
    //    //    return true;
    //    //}

    //    //传送门的四个边界点
    //    Vector3 _vec0 = _hitPoint + _hit.transform.right * 0.5f;
    //    Vector3 _vec1 = _hitPoint - _hit.transform.right * 0.5f;
    //    Vector3 _vec2 = _hitPoint - _hit.transform.up * 1f;
    //    Vector3 _vec3 = _hitPoint + _hit.transform.up * 1f;
    //    if (isContains(_hit, _vec0) && isContains(_hit, _vec1) && isContains(_hit, _vec2) && isContains(_hit, _vec3))
    //        return true;
    //    return false;
    //}
    ///// <summary>
    ///// 点是否在碰撞体内
    ///// </summary>
    ///// <param name="_hit"></param>
    ///// <param name="_hitPoint"></param>
    ///// <returns></returns>
    //private bool isContains(RaycastHit _hit, Vector3 _hitPoint)
    //{
    //    //距离向量
    //    Vector3 _distance = _hitPoint - _hit.transform.position;
    //    Quaternion _rotation = _hit.transform.rotation;
    //    _rotation = Quaternion.Inverse(_rotation);
    //    //转换为击中碰撞体的中心的世界坐标
    //    Vector3 _newWorldPosition = _rotation * _distance + _hit.transform.position;
    //    Vector3 _center = _hit.transform.position;
    //    //计算中心点到每个方向边界的半径
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
    ///// 计算两个数之间是否满足大于，约等于（否则会有误判）
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
    ///// 判断击中点附近是否有另一个传送门
    ///// </summary>
    ///// <param name="_hit">击中点的hit</param>
    ///// <param name="_hitPoint">击中点坐标</param>
    ///// <param name="_id">传送门id</param>
    ///// <returns></returns>
    //private bool CheckForCurrentPortal(RaycastHit _hit, Vector3 _hitNormal, Vector3 _hitPoint, int _id)
    //{
    //    //另外一个传送门的id
    //    int _otherId = _id == 0 ? 1 : 0;
    //    Vector3 _distance;
    //    //判断传送门是否已“产生”
    //    if (_otherId == 0 && bluePortal.GetComponent<Portal>().isPlaced)
    //    {
    //        //另外一个门与击中点的距离向量
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
    //    //如果距离向量垂直于击中平面的法向，可以认为击中点和另一个传送门是在同一个平面上的
    //    if (Vector3.Dot(_distance, _hitNormal) < 0.02f)
    //    {
    //        //计算距离向量在击中平面自身的x，y轴上的投影
    //        float _rightProjection = Vector3.Dot(_distance, _hit.transform.right);
    //        float _upProjection = Vector3.Dot(_distance, _hit.transform.up);
    //        //不在矩形范围内，返回假，

    //        if (Mathf.Abs(_rightProjection) > 1.25f || Mathf.Abs(_upProjection) > 2.5f)
    //            return false;
    //        else
    //            return true;
    //    }
    //    else
    //        return false;
    //}

    ///// <summary>
    ///// 产生传送门
    ///// </summary>
    ///// <param name="_id">传送门id</param>
    ///// <param name="_hitPoint">撞击点</param>
    ///// <param name="_normal">撞击平面法线</param>
    //private void GeneratePortal(Vector3 _hitUp, int _id, Vector3 _hitPoint, Vector3 _normal)
    //{
    //    if (_id == 0)
    //    {
    //        //计算传送门的旋转向量
    //        Quaternion _rotation = Quaternion.LookRotation(-_normal, _hitUp);
    //        ////创建传送门
    //        //GameObject newPortal = Instantiate(portals[0], _hitPoint, Quaternion.identity);
    //        //移动并旋转传送门
    //        bluePortal.transform.position = _hitPoint;
    //        bluePortal.transform.rotation = _rotation;
    //        //将创建的传送门的位置加上一个微小的向着平面法向上的值（在没有加的时候会出现z-Fighting）
    //        bluePortal.transform.position += _normal * 0.01f;
    //        //蓝色传送门是否存在设为真
    //        bluePortal.GetComponent<Portal>().isPlaced = true;
    //        //将传送门依附的物体的碰撞体传递
    //        bluePortal.GetComponent<Portal>().SetAttachCollider(hit.collider);
    //    }
    //    else if (_id == 1)
    //    {
    //        //计算传送门的旋转向量
    //        Quaternion _rotation = Quaternion.LookRotation(-_normal, _hitUp);
    //        ////创建传送门
    //        //GameObject newPortal = Instantiate(portals[0], _hitPoint, Quaternion.identity);
    //        //移动并旋转传送门
    //        orangeProtal.transform.position = _hitPoint;
    //        orangeProtal.transform.rotation = _rotation;
    //        //将创建的传送门的位置加上一个微小的向着平面法向上的值（在没有加的时候会出现z-Fighting）
    //        orangeProtal.transform.position += _normal * 0.01f;
    //        //橙色传送门是否存在设为真
    //        orangeProtal.GetComponent<Portal>().isPlaced = true;
    //        //将传送门依附的物体的碰撞体传递
    //        orangeProtal.GetComponent<Portal>().SetAttachCollider(hit.collider);
    //    }
    //}
    ///// <summary>
    ///// 偏移击中点
    ///// </summary>
    ///// <param name="_hit">击中点的hit信息</param>
    ///// <param name="_hitPoint">击中点坐标</param>
    ///// <returns></returns>
    //private Vector3 OffsetPoint(RaycastHit _hit, Vector3 _hitPoint)
    //{
    //    //击中点与击中平面中心的距离矢量
    //    Vector3 _distance = _hit.transform.position - _hitPoint;
    //    //距离矢量在击中平面自身的x、y轴的投影
    //    float _xProjection = Vector3.Dot(_distance, _hit.transform.right);
    //    float _yProjection = Vector3.Dot(_distance, _hit.transform.up);
    //    //x,y上的离边界的差值
    //    float _xOffset = _hit.transform.lossyScale.x / 2 - Mathf.Abs(_xProjection);
    //    float _yOffset = _hit.transform.lossyScale.y / 2 - Mathf.Abs(_yProjection);
    //    float _newAddX = 0;
    //    float _newAddy = 0;
    //    if (_xOffset <= 0.7f)
    //        _newAddX = _xProjection < 0 ? _xOffset - 0.8f : 0.8f - _xOffset;
    //    if (_yOffset <= 1.4)
    //        _newAddy = _yProjection < 0 ? _yOffset - 1.4f : 1.4f - _yOffset;
    //    //新的中心点的坐标
    //    Vector3 _newHitPoint = _hitPoint + _hit.transform.right * _newAddX + _hit.transform.up * _newAddy;
    //    return _newHitPoint;
    //}
    ///// <summary>
    ///// 偏移击中点
    ///// </summary>
    ///// <param name="_hit">击中点hit信息</param>
    ///// <param name="_hitPoint">击中点坐标</param>
    ///// <param name="_projectVec">射线投影</param>
    ///// <returns></returns>
    //private Vector3 OffsetPoint(RaycastHit _hit, Vector3 _hitPoint, Vector3 _projectVec)
    //{
    //    //投影向量和right、up轴的夹角
    //    float _angleRight = Vector3.Angle(_projectVec, _hit.transform.right);
    //    float _angleUp = Vector3.Angle(_projectVec, _hit.transform.up);
    //    //恰好能契合时的中心点距离边界轴的距离
    //    float _rightDistance = 1.0f * Mathf.Abs(Mathf.Cos(_angleRight)) + 0.5f * Mathf.Abs(Mathf.Sin(_angleRight));
    //    float _upDistance = 1.0f * Mathf.Abs(Mathf.Cos(_angleUp)) + 0.5f * Mathf.Abs(Mathf.Sin(_angleUp));
    //    //击中点与击中平面中心的距离矢量
    //    Vector3 _distance = _hit.transform.position - _hitPoint;
    //    //距离矢量在击中平面自身的x、y轴的投影
    //    float _xProjection = Vector3.Dot(_distance, _hit.transform.right);
    //    float _yProjection = Vector3.Dot(_distance, _hit.transform.up);
    //    //x,y上的离边界的差值
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
    /// 第一步，先将传送门移到击中位置
    /// </summary>
    /// <param name="_id">传送门id</param>
    /// <param name="_hitNormal">击中法向</param>
    /// <param name="_hitPoint">击中点</param>
    private void FirstStepForGenerate(int _id, Vector3 _hitNormal, Vector3 _hitPoint)
    {
        //算出平面的Right轴
        Vector3 _hitRight = Vector3.Cross(Vector3.up, _hitNormal);
        //进而计算出平面的Up轴
        Vector3 _hitUp = Vector3.Cross(_hitNormal, _hitRight);
        //计算出传送门的朝向
        Quaternion _portalRot = Quaternion.LookRotation(-_hitNormal, _hitUp);
        //判断是哪个传送门
        var _portal = _id == 0 ? bluePortal : orangeProtal;
        //记录原先的位置和旋转，方便无法产生时回溯
        originalPos = _portal.transform.position;
        orignalRot = _portal.transform.rotation;
        //先将传送门移到相应位置并旋转，之后进行后续处理
        _portal.transform.position = _hitPoint;
        _portal.transform.rotation = _portalRot;
    }

    /// <summary>
    /// 第一步，先将传送门移到击中位置
    /// </summary>
    /// <param name="_id">传送门id</param>
    /// <param name="_hitNormal">击中法向</param>
    /// <param name="_hitPoint">击中点</param>
    /// <param name="_hitUp">击中Up轴</param>
    private void FirstStepForGenerate(int _id, Vector3 _hitNormal, Vector3 _hitPoint, Vector3 _hitUp)
    {
        //计算出传送门的朝向
        Quaternion _portalRot = Quaternion.LookRotation(-_hitNormal, _hitUp);
        //判断是哪个传送门
        var _portal = _id == 0 ? bluePortal : orangeProtal;
        //记录原先的位置和旋转，方便无法产生时回溯
        originalPos = _portal.transform.position;
        orignalRot = _portal.transform.rotation;
        //先将传送门移到相应位置并旋转，之后进行后续处理
        _portal.transform.position = _hitPoint;
        _portal.transform.rotation = _portalRot;
    }

    /// <summary>
    /// 检查传送门的边界是否超出墙体的边界，如果是，则偏移
    /// </summary>
    /// <param name="_id">传送门id</param>
    private bool CheckForAround(int _id)
    {
        //得到生成传送门的id
        var _portalTransform = _id == 0 ? bluePortal.transform : orangeProtal.transform;
        //发射偏移检测射线的起点
        Vector3 _raycastPos;
        //发射偏移检测射线的方向
        Vector3 _raycastDir;
        for(int i = 0; i < 4; i++)
        {
            //将传送门的四个边界点转换到世界坐标系
            _raycastPos = _portalTransform.TransformPoint(portalBounds[i]);
            //将传送门面的四个方向转换到世界坐标系
            _raycastDir = _portalTransform.TransformDirection(portalDirection[i]);
            //在四个边界点上检测是否有碰撞体
            int _colliderCount = Physics.OverlapSphereNonAlloc(_raycastPos, 0.03f, hitBoundCollider, enablePortalLayer);
            //当检测到边界点在碰撞体内时，直接Continue
            bool isSkip = false;
            if (_colliderCount > 0)
            {
                for(int j = 0; j < _colliderCount; j++)
                {
                    //只有碰撞体是传送门所附着的碰撞体才行
                    if (hitBoundCollider[j].GetInstanceID() == hitCollider)
                        isSkip = true;
                }
                if (isSkip)
                {
                    continue;
                }
            }

            //从边界点发射射线
            hitBound = Physics.RaycastAll(_raycastPos, _raycastDir, 1.5f, enablePortalLayer);
            if (hitBound.Length > 0)
            {
                for(int j = 0; j < hitBound.Length; j++)
                {
                    //当射线检测到传送门附着边界点时
                    if (hitBound[j].colliderInstanceID == hitCollider)
                    {
                        //单个边界点的偏移量
                        Vector3 _offset = hitBound[j].point - _raycastPos;
                        _portalTransform.Translate(_offset, Space.World);
                        break;
                    }
                }
            }
            //如果边界点即不在碰撞体内又不能偏移，则返回false
            else
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckForPorjectAround(int _id)
    {
        //得到生成传送门的id
        var _portalTransform = _id == 0 ? bluePortal.transform : orangeProtal.transform;
        //发射偏移检测射线的起点
        Vector3 _raycastPos;
        //发射偏移检测射线的方向
        Vector3 _raycastDir;
        //最终的偏移量
        Vector3 _offset = Vector3.zero;
        for(int i = 0; i < 4; i++)
        {
            _raycastPos = _portalTransform.TransformPoint(portalProjectBounds[i]);
            _raycastDir = _portalTransform.TransformDirection(portalProjectDirection[i]);
            //在四个角点上检测是否有碰撞体
            int _colliderCount = Physics.OverlapSphereNonAlloc(_raycastPos, 0.05f, hitBoundCollider, enablePortalLayer);
            //当检测到角点在碰撞体内时，直接Continue
            bool isSkip = false;
            if (_colliderCount > 0)
            {
                for (int j = 0; j < _colliderCount; j++)
                {
                    //只有碰撞体是传送门所附着的碰撞体才行
                    if (hitBoundCollider[j].GetInstanceID() == hitCollider)
                        isSkip = true;
                }
                if (isSkip)
                    continue;
            }
            //从边界点发射射线
            hitBound = Physics.RaycastAll(_raycastPos, _raycastDir, 1.5f, enablePortalLayer);
            if (hitBound.Length > 0)
            {
                for (int j = 0; j < hitBound.Length; j++)
                {
                    //当射线检测到传送门附着边界点时
                    if (hitBound[j].colliderInstanceID == hitCollider)
                    {
                        //单个边界点的偏移量
                        //if(_offset.sqrMagnitude < (hitBound[j].point - _raycastPos).sqrMagnitude)
                        _offset = hitBound[j].point - _raycastPos;
                        _portalTransform.Translate(_offset, Space.World);
                        break;
                    }
                }
            }
            //如果边界点即不在碰撞体内又不能偏移，则返回false
            else
            {
                return false;
            }
        }
        //Debug.Log($"offset:{_offset}");
        return true;
    }

    /// <summary>
    /// 产生传送门
    /// </summary>
    /// <param name="_id">传送门id</param>
    /// <param name="_hitPoint">撞击点</param>
    /// <param name="_normal">撞击平面法线</param>
    private void GeneratePortal(int _id, Vector3 _normal)
    {
        var _portal = _id == 0 ? bluePortal : orangeProtal;
        _portal.transform.position += _normal * 0.001f;
        _portal.GetComponent<Portal>().isPlaced = true;
        _portal.GetComponent<Portal>().SetAttachCollider(hit.collider);
        //if (_id == 0)
        //{
        //    //将创建的传送门的位置加上一个微小的向着平面法向上的值（在没有加的时候会出现z-Fighting）
        //    bluePortal.transform.position += _normal * 0.01f;
        //    //蓝色传送门是否存在设为真
        //    bluePortal.GetComponent<Portal>().isPlaced = true;
        //    //将传送门依附的物体的碰撞体传递
        //    bluePortal.GetComponent<Portal>().SetAttachCollider(hit.collider);
        //}
        //else if (_id == 1)
        //{
        //    //将创建的传送门的位置加上一个微小的向着平面法向上的值（在没有加的时候会出现z-Fighting）
        //    orangeProtal.transform.position += _normal * 0.01f;
        //    //橙色传送门是否存在设为真
        //    orangeProtal.GetComponent<Portal>().isPlaced = true;
        //    //将传送门依附的物体的碰撞体传递
        //    orangeProtal.GetComponent<Portal>().SetAttachCollider(hit.collider);
        //}
    }

    /// <summary>
    /// 检查击中点附近是否有另一个传送门
    /// </summary>
    /// <param name="_id"></param>
    /// <param name="_hitPoint"></param>
    /// <param name="_hitNormal"></param>
    /// <returns></returns>
    private bool CheckForAnother(int _id, Vector3 _hitPoint, Vector3 _hitNormal)
    {
        //另外一个传送门的id
        int _otherId = _id == 0 ? 1 : 0;
        var _portalTransform = _id == 0 ? bluePortal.transform : orangeProtal.transform;
        string _anotherTag;
        Vector3 _distance;
        //判断传送门是否已“产生”
        if (_otherId == 0 && bluePortal.GetComponent<Portal>().isPlaced)
        {
            //另外一个门与击中点的距离向量
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
        //如果距离向量垂直于击中平面的法向，可以认为击中点和另一个传送门是在同一个平面上的
        if (Vector3.Dot(_distance, _hitNormal) < 0.01f)
        {
            ////计算距离向量在击中平面自身的x，y轴上的投影
            //float _rightProjection = Vector3.Dot(_distance, hit.transform.right);
            //float _upProjection = Vector3.Dot(_distance, hit.transform.up);
            ////不在矩形范围内，返回假，
            //if (Mathf.Abs(_rightProjection) > 1.25f || Mathf.Abs(_upProjection) > 2.5f)
            //    return false;
            //else
            //    return true;

            //通过射线检测判断击中点的附近是否有另一个传送门
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
