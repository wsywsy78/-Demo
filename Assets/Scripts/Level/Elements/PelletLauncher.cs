using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PelletLauncher : MonoBehaviour
{
    /// <summary>
    /// �ͷŵ����ӵ��ٶ�
    /// </summary>
    [SerializeField] private Vector3 pelletVelocity;
    /// <summary>
    /// ����Ԥ����
    /// </summary>
    [SerializeField] private GameObject pelletPrefab;
    /// <summary>
    /// ʵ�ʲ�������
    /// </summary>
    private GameObject pellet;
    /// <summary>
    /// ���ӵĸ���
    /// </summary>
    private Rigidbody pelletRb;
    /// <summary>
    /// �÷�������������ӵ�idֵ
    /// </summary>
    private int pelletId;
    /// <summary>
    /// ���ӵĳ�ʼRotation
    /// </summary>
    private Quaternion pelletRotation;
    /// <summary>
    /// ���ӳ�ʼλ��
    /// </summary>
    private Vector3 pelletPosition;
    /// <summary>
    /// ���ӵ���Է���λ��
    /// </summary>
    [SerializeField] private Vector3 launchPosition;
    private void Awake()
    {
        //��ǰ��������Ԥ����
        pellet = Instantiate(pelletPrefab, transform.position + launchPosition, Quaternion.identity);
        pelletRotation = pellet.transform.rotation;
        pelletPosition = pellet.transform.position;
        //idֵ�������ӵ�idֵ
        pelletId = pellet.gameObject.GetInstanceID();
        pelletRb = pellet.GetComponent<Rigidbody>();
        //�ı�Բ�����ɫ
        pellet.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
    }

    private void Start()
    {
        EventManager.Instance.AddListener(EventName.ResetPellet, ResetPellet);
        //��������һ�����ٶ�����
        //pelletRb.AddForce(pelletVelocity, ForceMode.VelocityChange);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.ResetPellet, ResetPellet);
    }

    public void StartLaunch()
    {
        pelletRb.AddForce(pelletVelocity, ForceMode.VelocityChange);
    }

    /// <summary>
    /// ���·�������
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ResetPellet(object sender, EventArgs e)
    {
        var _info = e as EventParameter;
        //��������ֻ���ɶ�Ӧ�����Ӵ���
        if (_info.eventInt != pelletId)
            return;
        StartCoroutine(StartReset());
    }

    private IEnumerator StartReset()
    {
        yield return new WaitForSeconds(1);
        pellet.SetActive(true);
        pellet.transform.position = pelletPosition;
        pellet.transform.rotation = pelletRotation;
        pelletRb.velocity = Vector3.zero;
        //�����ʩ�������������÷�ʽΪVelocityChange����֤������һ�����ٶ��˶�
        pelletRb.AddForce(pelletVelocity, ForceMode.VelocityChange);
    }
}
