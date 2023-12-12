using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PelletLauncher : MonoBehaviour
{
    /// <summary>
    /// 释放的粒子的速度
    /// </summary>
    [SerializeField] private Vector3 pelletVelocity;
    /// <summary>
    /// 粒子预制体
    /// </summary>
    [SerializeField] private GameObject pelletPrefab;
    /// <summary>
    /// 实际产生粒子
    /// </summary>
    private GameObject pellet;
    /// <summary>
    /// 粒子的刚体
    /// </summary>
    private Rigidbody pelletRb;
    /// <summary>
    /// 该发射器发射的粒子的id值
    /// </summary>
    private int pelletId;
    /// <summary>
    /// 粒子的初始Rotation
    /// </summary>
    private Quaternion pelletRotation;
    /// <summary>
    /// 粒子初始位置
    /// </summary>
    private Vector3 pelletPosition;
    /// <summary>
    /// 粒子的相对发射位置
    /// </summary>
    [SerializeField] private Vector3 launchPosition;
    private void Awake()
    {
        //提前生成粒子预制体
        pellet = Instantiate(pelletPrefab, transform.position + launchPosition, Quaternion.identity);
        pelletRotation = pellet.transform.rotation;
        pelletPosition = pellet.transform.position;
        //id值等于粒子的id值
        pelletId = pellet.gameObject.GetInstanceID();
        pelletRb = pellet.GetComponent<Rigidbody>();
        //改变圆球的颜色
        pellet.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
    }

    private void Start()
    {
        EventManager.Instance.AddListener(EventName.ResetPellet, ResetPellet);
        //让粒子以一定的速度运行
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
    /// 重新发射粒子
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ResetPellet(object sender, EventArgs e)
    {
        var _info = e as EventParameter;
        //重射粒子只能由对应的粒子触发
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
        //向刚体施加力，力的作用方式为VelocityChange，保证粒子以一定的速度运动
        pelletRb.AddForce(pelletVelocity, ForceMode.VelocityChange);
    }
}
