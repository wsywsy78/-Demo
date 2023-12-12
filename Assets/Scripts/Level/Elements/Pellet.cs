using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour
{
    /// <summary>
    /// ���ӵĸ���
    /// </summary>
    private Rigidbody rb;
    /// <summary>
    /// ���ӷ�������
    /// </summary>
    private int bounceCount;
    /// <summary>
    /// ��󷴵�����
    /// </summary>
    [SerializeField] private int maxBounceCount = 10;
    /// <summary>
    /// �����Ӷ�Ӧ��idֵ,idֵ����Ϊ-1��-1�Ǽ����û�и�ֵ�ı�׼��
    /// </summary>
    private int pelletId;
    /// <summary>
    /// ����Ĳ���
    /// </summary>
    private Material material;
    /// <summary>
    /// ���ʵ���ɫ
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
    /// �������ӵ�idֵ
    /// </summary>
    /// <param name="_id">�����idֵ</param>
    public void SetPelletId(int _id)
    {
        pelletId = _id;
    }

    /// <summary>
    /// ��鷴������
    /// </summary>
    private void CheckForBounce()
    {
        bounceCount++;
        //��������������������ޣ����ں�����Ҫʹ�����ӣ�������Ϊ����Ծ
        if(bounceCount > maxBounceCount)
        {
            this.TriggerEvent(EventName.ResetPellet, new EventParameter { eventInt = pelletId });
            //���ӵ���Ϊfalse����������ʱ��GC
            gameObject.SetActive(false);
            bounceCount = 0;
            //���ӳ�һ����ط�����
        }
    }

    /// <summary>
    /// ���ڽ������Ѿ����ܵ����ӵ������ֱ���������壨��������ʹ�ã�
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
