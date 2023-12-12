using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour
{
    /// <summary>
    /// 粒子的刚体
    /// </summary>
    private Rigidbody rb;
    /// <summary>
    /// 粒子反弹次数
    /// </summary>
    private int bounceCount;
    /// <summary>
    /// 最大反弹次数
    /// </summary>
    [SerializeField] private int maxBounceCount = 10;
    /// <summary>
    /// 该粒子对应的id值,id值不能为-1（-1是检测有没有赋值的标准）
    /// </summary>
    private int pelletId;
    /// <summary>
    /// 物体的材质
    /// </summary>
    private Material material;
    /// <summary>
    /// 材质的颜色
    /// </summary>
    private Color materialColor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        material = GetComponent<Renderer>().material;
        materialColor = material.color;
        bounceCount = 0;
        pelletId = gameObject.GetInstanceID();
    }

    private void Start()
    {
        EventManager.Instance.AddListener(EventName.DestroyPellet, DestroyPellet);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.DestroyPellet, DestroyPellet);
    }

    private void OnEnable()
    {
        material.color = materialColor;
    }

    /// <summary>
    /// 设置粒子的id值
    /// </summary>
    /// <param name="_id">传入的id值</param>
    public void SetPelletId(int _id)
    {
        pelletId = _id;
    }

    /// <summary>
    /// 检查反弹次数
    /// </summary>
    private void CheckForBounce()
    {
        bounceCount++;
        //如果反弹次数超过了上限，由于后续还要使用粒子，将其设为不活跃
        if(bounceCount > maxBounceCount)
        {
            this.TriggerEvent(EventName.ResetPellet, new EventParameter { eventInt = pelletId });
            //将子弹设为false，避免销毁时的GC
            gameObject.SetActive(false);
            bounceCount = 0;
            //在延迟一秒后重发粒子
        }
    }

    /// <summary>
    /// 对于接收器已经接受到粒子的情况，直接销毁物体（后续不会使用）
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DestroyPellet(object sender, EventArgs e)
    {
        var _info = e as EventParameter;
        if (_info.eventInt != pelletId)
            return;
        Destroy(gameObject);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("BluePortal") || collision.collider.CompareTag("OrangePortal"))
            return;
        bounceCount++;
        CheckForBounce();
        ReduceEnergy();
    }

    private void ReduceEnergy()
    {
        float _opacity = (maxBounceCount - (float)bounceCount / 2) / maxBounceCount;        
        Color _color = material.color;
        _color.a = _opacity;
        material.color = _color;
    }
}
