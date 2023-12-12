using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour, IPlatform
{
    /// <summary>
    /// ƽ̨����������ߵľ���
    /// </summary>
    [SerializeField] private Vector3 moveRoundDistance;
    /// <summary>
    /// ����������õ�ʱ��
    /// </summary>
    [Range(0.5f, 20f)]
    [SerializeField] private float moveRoundTimer;
    /// <summary>
    /// platform��id���������д��ڶ��platformʱ��Ҫ����id������Ӧƽ̨���ƶ�
    /// </summary>
    [SerializeField] private int platformId;
    /// <summary>
    /// ƽ̨�ĳ�ʼλ��
    /// </summary>
    private Vector3 defaultPosition;
    /// <summary>
    /// ƽ̨�ƶ��ٶ�
    /// </summary>
    public Vector3 moveVelocity { get; private set; }
    /// <summary>
    /// �������������ƶ���ż���������ƶ�
    /// </summary>
    private int roundBack;

    private void Start()
    {
        EventManager.Instance.AddListener(EventName.Unlock, StartMoveRound);

        defaultPosition = transform.position;
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.Unlock, StartMoveRound);
    }

    /// <summary>
    /// ƽ̨��ʼ�����ƶ�
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StartMoveRound(object sender, EventArgs e)
    {
        var _info = e as EventParameter;
        //ֻ�е������intֵ����platform��idֵʱ�����ܴ��������ƶ��Ĺ���
        if (_info.eventInt != gameObject.GetInstanceID())
            return;
        MoveTo();
    }

    /// <summary>
    /// ƽ̨�ƶ�
    /// </summary>
    /// <param name="_moveDirection">�ƶ��ľ���ʸ��</param>
    private void MoveTo() 
    {
        Vector3 _position = transform.position;
        //ÿ�ε���MoveTo���ͽ�roundBack��ֵ��һ
        roundBack++;
        Vector3 _moveDirection = moveRoundDistance * (roundBack % 2f);
        transform.DOMove(_moveDirection + defaultPosition, moveRoundTimer).SetEase(Ease.Linear).OnComplete(() =>
        {
            //���෴��λ�ƴ��룬���ϵݹ�
            MoveTo();
        }).
        OnUpdate(() =>
        {
            //����ǰһ֡��λ�ú���һ֡��λ����ʱ��Ĳ�ֵ�����ٶ�
            moveVelocity = (transform.position - _position) / Time.deltaTime;
            _position = transform.position;
        });
    }

    public Vector3 GetPlatformVelocity()
    {
        return moveVelocity;
    }
}
