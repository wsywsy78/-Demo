using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Runtime.InteropServices;

/// <summary>
/// 第一人称转动视角类
/// </summary>
public class MouseLook : MonoBehaviour
{
    /// <summary>
    /// 鼠标X轴灵敏度
    /// </summary>
    [SerializeField] private float xSensitivity;
    /// <summary>
    /// 鼠标y轴灵敏度
    /// </summary>
    [SerializeField] private float ySensitivity;
    /// <summary>
    /// 玩家的Transform
    /// </summary>
    [SerializeField] private Transform playerBody;
    /// <summary>
    /// 传送枪的Transform
    /// </summary>
    [SerializeField] private Transform portalGun;
    /// <summary>
    /// 输入设备的鼠标X轴变化量
    /// </summary>
    private float mouseX => InputManager.Instance.mouseInput.x;
    /// <summary>
    /// 输入设备的鼠标y轴变化量
    /// </summary>
    private float mouseY => InputManager.Instance.mouseInput.y;
    /// <summary>
    /// x轴旋转角度
    /// </summary>
    private float xRotation;
    /// <summary>
    /// 传送枪x轴旋转角度
    /// </summary>
    private float gunXRotation;
    /// <summary>
    /// 玩家是否刚刚经历传送
    /// </summary>
    private bool isTeleport = false;
    /// <summary>
    /// 视角是否在回正中
    /// </summary>
    private bool isRecover = false;

    #region 传送枪震动相关
    private float duration = 10f;
    private float strength = 0.02f;
    private Tween twe;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //将鼠标中心对准屏幕中心
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
        //在传送的瞬间，得到相机的旋转角度
        else
        {
            xRotation = transform.localRotation.eulerAngles.x;
            float _zRotation = transform.localRotation.eulerAngles.z;

            xRotation -= _mouseY;
            //xRotation = Mathf.Clamp(xRotation, -90, 90);
            playerBody.Rotate(Vector3.up * _mouseX);
            //由于xRotation = Mathf.Clamp(xRotation, -90, 90)的存在，当xRotation绝对值大于90时要转换成-90到90以内的值
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
