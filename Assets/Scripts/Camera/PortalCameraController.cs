using UnityEngine;
/// <summary>
/// 控制绘制两个传送门的相机脚本
/// </summary>
public class PortalCameraController : MonoBehaviour
{
    /// <summary>
    /// 两个传送门
    /// </summary>
    private Portal[] portals = new Portal[2];
    /// <summary>
    /// 绘制传送门反射的相机
    /// </summary>
    [SerializeField] private Camera[] cameras = new Camera[2];
    /// <summary>
    /// 主摄像机
    /// </summary>
    private Camera mainCamera;
    /// <summary>
    /// 蓝色传送门的临时纹理
    /// </summary>
    private RenderTexture blueTexture;
    /// <summary>
    /// 橙色传送门的临时纹理
    /// </summary>
    private RenderTexture orangeTexture;
    /// <summary>
    /// 传送门最大递归次数
    /// </summary>
    [SerializeField] private int iterations = 5;

    
    private void Awake()
    {
        blueTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        orangeTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
    }

    private void Start()
    {
        portals[0] = GameObject.FindWithTag("BluePortal")?.GetComponent<Portal>();
        portals[1] = GameObject.FindWithTag("OrangePortal")?.GetComponent<Portal>();
        portals[0]?.SetTexture(blueTexture);
        portals[1]?.SetTexture(orangeTexture);

        //获取场景中主摄像机
        mainCamera = Camera.main;
        //替换传送门相机渲染时的所有物体的shader
        //cameras[0].SetReplacementShader(_portalShader, "RenderType");
        //cameras[1].SetReplacementShader(_portalShader, "RenderType");
    }

    private void Update()
    {
        ////获取主摄像机距传送门的距离向量，主摄像机的旋转信息
        //Vector3 _overturnBlue = portals[0].position - _mainCamera.position;
        //Vector3 _overturnOrange = portals[1].position - _mainCamera.position;
        //Vector3 _rotationAxis = Vector3.Cross(_overturnBlue, _overturnOrange).normalized;
        //float _angle = Vector3.Angle(_overturnBlue, _overturnOrange);
        //Quaternion _bluerotation = Quaternion.AngleAxis(_angle, _rotationAxis);
        //Quaternion _orangerotation = Quaternion.AngleAxis(-_angle, _rotationAxis);
        //Vector3 _bluePosition = _bluerotation * _overturnBlue;
        //Vector3 _orangePosition = _orangerotation * _overturnOrange;
        //cameras[0].transform.position = portals[1].transform.position + _orangePosition;
        //cameras[1].transform.position = portals[0].transform.position + _bluePosition;

        //更新摄像机的位置信息
        //SetAndRenderCamera(portals[0].transform, portals[1].transform, cameras[0], 1);
        //SetAndRenderCamera(portals[1].transform, portals[0].transform, cameras[1], 1);
    }

    private void LateUpdate()
    {
        ////获取相机视野包围盒
        //GeometryUtility.CalculateFrustumPlanes(mainCamera, cameraBounds);

        //Debug.Log($"{CheckIsInCamera(0)}");
    }

    /// <summary>
    /// 预渲染处理，反向递归将摄像机移动到相应的位置，实现门中门效果
    /// </summary>
    private void OnPreRender()
    {
        //如果传送门可见并且已经放置，设置相机并渲染传送门
        if (portals[0].IsRenderVisible() && portals[0].isPlaced && portals[1].isPlaced)
        {
            //将摄像机的targetTexture设为传送门的mainTex
            cameras[0].targetTexture = blueTexture;
            //反向递归，从递归次数最大的地方递归，先绘制最深层的画面，再逐层退出
            for (int i = iterations; i > 0; i--)
            {
                SetAndRenderCamera(portals[0].transform, portals[1].transform, cameras[0], i);
            }
        }
        if (portals[1].IsRenderVisible() && portals[1].isPlaced && portals[0].isPlaced)
        {
            cameras[1].targetTexture = orangeTexture;
            for (int i = iterations; i > 0; i--)
            {
                SetAndRenderCamera(portals[1].transform, portals[0].transform, cameras[1], i);
            }
        }

    }

    /// <summary>
    /// 设置摄像机的位置和旋转，蓝色摄像机观察橙色传送门，橙色摄像机观察蓝色传送门
    /// </summary>
    /// <param name="_inPortal">摄像机将渲染画面展示的传送门</param>
    /// <param name="_outPortal">摄像机跟随的传送门</param>
    /// <param name="_camera">摄像机的Transform</param>
    private void SetAndRenderCamera(Transform _inPortal, Transform _outPortal, Camera _camera, int _iterations)
    {
        //先将传送门摄像机的位置移到主摄像机的位置
        _camera.transform.position = mainCamera.transform.position;
        _camera.transform.rotation = mainCamera.transform.rotation;

        //迭代，将传送门相机移动到相应递归层次的位置
        for (int i = 0; i < _iterations; i++)
        {
            //将摄像机的位置相对进入传送门的位置翻转
            Vector3 _relativePos = _inPortal.InverseTransformPoint(_camera.transform.position);
            _relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * _relativePos;
            //将摄像机的位置移到出传送门的位置
            _camera.transform.position = _outPortal.TransformPoint(_relativePos);

            //将摄像机的旋转信息也相对进入传送门的位置翻转
            Quaternion _relativeRot = Quaternion.Inverse(_inPortal.rotation) * _camera.transform.rotation;
            _relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * _relativeRot;
            _camera.transform.rotation = _outPortal.rotation * _relativeRot;

        }

        //Debug.DrawLine(mainCamera.transform.position, _inPortal.position, Color.green);
        //Debug.DrawLine(_camera.transform.position, _outPortal.position, Color.green);

        //得到传送门平面的信息，将之作为摄像机的斜投影矩阵，以保证摄像机的只能观察到传送门后的物体
        Plane p = new Plane(-_outPortal.forward, _outPortal.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(_camera.worldToCameraMatrix)) * clipPlane;
        var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        _camera.projectionMatrix = newMatrix;

        _camera.Render();
    }

    //private void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    if (portals[0].IsRenderVisible() && portals[0].isPlaced && portals[1].isPlaced)
    //    {
    //        cameras[0].targetTexture = blueTexture;
    //        SetAndRenderCamera(portals[0].transform, portals[1].transform, cameras[0], 1);
    //        testMat.SetInt("_RefValue", 1);
    //        Graphics.Blit(blueTexture, source, testMat);
    //    }
    //    if (portals[1].IsRenderVisible() && portals[0].isPlaced && portals[1].isPlaced)
    //    {
    //        cameras[1].targetTexture = orangeTexture;
    //        SetAndRenderCamera(portals[1].transform, portals[0].transform, cameras[1], 1);
    //        testMat.SetInt("_RefValue", 1);
    //        Graphics.Blit(orangeTexture, source, testMat);
    //    }

    //    Graphics.Blit(source, destination);
    //}

    //private bool CheckIsInCamera(int _id)
    //{
    //    return GeometryUtility.TestPlanesAABB(cameraBounds, portals[_id].GetComponent<Collider>().bounds);
    //}
}
