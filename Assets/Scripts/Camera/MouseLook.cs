using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Runtime.InteropServices;

/// <summary>
/// ��һ�˳�ת���ӽ���
/// </summary>
public class MouseLook : MonoBehaviour
{
    /// <summary>
    /// ���X��������
    /// </summary>
    [SerializeField] private float xSensitivity;
    /// <summary>
    /// ���y��������
    /// </summary>
    [SerializeField] private float ySensitivity;
    /// <summary>
    /// ��ҵ�Transform
    /// </summary>
    [SerializeField] private Transform playerBody;
    /// <summary>
    /// ����ǹ��Transform
    /// </summary>
    [SerializeField] private Transform portalGun;
    /// <summary>
    /// �����豸�����X��仯��
    /// </summary>
    private float mouseX => InputManager.Instance.mouseInput.x;
    /// <summary>
    /// �����豸�����y��仯��
    /// </summary>
    private float mouseY => InputManager.Instance.mouseInput.y;
    /// <summary>
    /// x����ת�Ƕ�
    /// </summary>
    private float xRotation;
    /// <summary>
    /// ����ǹx����ת�Ƕ�
    /// </summary>
    private float gunXRotation;
    /// <summary>
    /// ����Ƿ�ով�������
    /// </summary>
    private bool isTeleport = false;
    /// <summary>
    /// �ӽ��Ƿ��ڻ�����
    /// </summary>
    private bool isRecover = false;

    #region ����ǹ�����
    private float duration = 10f;
    private float strength = 0.02f;
    private Tween twe;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //��������Ķ�׼��Ļ����
        Cursor.lockState = CursorLockMode.Locked;
        EventManager.Instance.AddListener(EventName.ChangeCameraMode, SetIsTeleport);


    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.ChangeCameraMode, SetIsTeleport);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float _mouseX = mouseX * xSensitivity * Time.deltaTime;
        float _mouseY = mouseY * ySensitivity * Time.deltaTime;
        if (!isTeleport)
        {

            xRotation -= _mouseY;
            xRotation = Mathf.Clamp(xRotation, -90, 90);
            

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * _mouseX);
        }
        //�ڴ��͵�˲�䣬�õ��������ת�Ƕ�
        else
        {
            xRotation = transform.localRotation.eulerAngles.x;
            float _zRotation = transform.localRotation.eulerAngles.z;

            xRotation -= _mouseY;
            //xRotation = Mathf.Clamp(xRotation, -90, 90);
            playerBody.Rotate(Vector3.up * _mouseX);
            //����xRotation = Mathf.Clamp(xRotation, -90, 90)�Ĵ��ڣ���xRotation����ֵ����90ʱҪת����-90��90���ڵ�ֵ
            if (xRotation < -90 || xRotation > 90)
                xRotation = xRotation - Mathf.RoundToInt(xRotation / 360) * 360;
            transform.localRotation = Quaternion.Euler(xRotation, 0, _zRotation);
            if(_zRotation != 0)
            {
                twe = transform.DORotateQuaternion(Quaternion.Euler(xRotation, 0, 0), 1);
                if (!isRecover)
                    twe.OnComplete(
                    () => {
                        isRecover = false;
                        isTeleport = false;
                    });
                isRecover = true;
            }
            else
                isTeleport = false;
        }
    }

    private void SetIsTeleport(object sender, EventArgs e)
    {
        isTeleport = true;
    }
}
