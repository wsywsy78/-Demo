using UnityEngine;

public class RenderPortal : MonoBehaviour
{
    [SerializeField] private Camera portalCamera;

    [SerializeField] private Material portalMaterial;

    private Camera mainCamera;

    [SerializeField] private GameObject[] portals = new GameObject[2];
    [SerializeField] private Shader replaceShader;

    private RenderTexture temTexture;

    private void Awake()
    {
        //�����ŵ�Ŀ������
        temTexture = new RenderTexture(Screen.width, Screen.height, 24);
        portals[0].GetComponent<Renderer>().material.mainTexture = temTexture;
        portals[1].GetComponent<Renderer>().material.mainTexture = temTexture;
    }

    private void Start()
    {
        //if (replaceShader == null)
        //    replaceShader = Shader.Find("Opaque");

        //�滻�������Ⱦʱ�����ж����Shader
        //portalCamera.SetReplacementShader(replaceShader, "RenderType");
        //Debug.Log($" {replaceShader.GetPassCountInSubshader(0)}");
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        
    }
}
