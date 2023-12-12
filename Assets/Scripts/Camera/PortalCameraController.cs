using UnityEngine;
/// <summary>
/// ���ƻ������������ŵ�����ű�
/// </summary>
public class PortalCameraController : MonoBehaviour
{
    /// <summary>
    /// ����������
    /// </summary>
    private Portal[] portals = new Portal[2];
    /// <summary>
    /// ���ƴ����ŷ�������
    /// </summary>
    [SerializeField] private Camera[] cameras = new Camera[2];
    /// <summary>
    /// �������
    /// </summary>
    private Camera mainCamera;
    /// <summary>
    /// ��ɫ�����ŵ���ʱ����
    /// </summary>
    private RenderTexture blueTexture;
    /// <summary>
    /// ��ɫ�����ŵ���ʱ����
    /// </summary>
    private RenderTexture orangeTexture;
    /// <summary>
    /// ���������ݹ����
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

        //��ȡ�������������
        mainCamera = Camera.main;
        //�滻�����������Ⱦʱ�����������shader
        //cameras[0].SetReplacementShader(_portalShader, "RenderType");
        //cameras[1].SetReplacementShader(_portalShader, "RenderType");
    }

    private void Update()
    {
        ////��ȡ��������ഫ���ŵľ��������������������ת��Ϣ
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

        //�����������λ����Ϣ
        //SetAndRenderCamera(portals[0].transform, portals[1].transform, cameras[0], 1);
        //SetAndRenderCamera(portals[1].transform, portals[0].transform, cameras[1], 1);
    }

    private void LateUpdate()
    {
        ////��ȡ�����Ұ��Χ��
        //GeometryUtility.CalculateFrustumPlanes(mainCamera, cameraBounds);

        //Debug.Log($"{CheckIsInCamera(0)}");
    }

    /// <summary>
    /// Ԥ��Ⱦ��������ݹ齫������ƶ�����Ӧ��λ�ã�ʵ��������Ч��
    /// </summary>
    private void OnPreRender()
    {
        //��������ſɼ������Ѿ����ã������������Ⱦ������
        if (portals[0].IsRenderVisible() && portals[0].isPlaced && portals[1].isPlaced)
        {
            //���������targetTexture��Ϊ�����ŵ�mainTex
            cameras[0].targetTexture = blueTexture;
            //����ݹ飬�ӵݹ�������ĵط��ݹ飬�Ȼ��������Ļ��棬������˳�
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
    /// �����������λ�ú���ת����ɫ������۲��ɫ�����ţ���ɫ������۲���ɫ������
    /// </summary>
    /// <param name="_inPortal">���������Ⱦ����չʾ�Ĵ�����</param>
    /// <param name="_outPortal">���������Ĵ�����</param>
    /// <param name="_camera">�������Transform</param>
    private void SetAndRenderCamera(Transform _inPortal, Transform _outPortal, Camera _camera, int _iterations)
    {
        //�Ƚ��������������λ���Ƶ����������λ��
        _camera.transform.position = mainCamera.transform.position;
        _camera.transform.rotation = mainCamera.transform.rotation;

        //������������������ƶ�����Ӧ�ݹ��ε�λ��
        for (int i = 0; i < _iterations; i++)
        {
            //���������λ����Խ��봫���ŵ�λ�÷�ת
            Vector3 _relativePos = _inPortal.InverseTransformPoint(_camera.transform.position);
            _relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * _relativePos;
            //���������λ���Ƶ��������ŵ�λ��
            _camera.transform.position = _outPortal.TransformPoint(_relativePos);

            //�����������ת��ϢҲ��Խ��봫���ŵ�λ�÷�ת
            Quaternion _relativeRot = Quaternion.Inverse(_inPortal.rotation) * _camera.transform.rotation;
            _relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * _relativeRot;
            _camera.transform.rotation = _outPortal.rotation * _relativeRot;

        }

        //Debug.DrawLine(mainCamera.transform.position, _inPortal.position, Color.green);
        //Debug.DrawLine(_camera.transform.position, _outPortal.position, Color.green);

        //�õ�������ƽ�����Ϣ����֮��Ϊ�������бͶӰ�����Ա�֤�������ֻ�ܹ۲쵽�����ź������
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
