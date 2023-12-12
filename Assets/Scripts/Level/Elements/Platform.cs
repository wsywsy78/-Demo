using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour, IPlatform
{
    /// <summary>
    /// 平台半个来回所走的距离
    /// </summary>
    [SerializeField] private Vector3 moveRoundDistance;
    /// <summary>
    /// 半个来回所用的时间
    /// </summary>
    [Range(0.5f, 20f)]
    [SerializeField] private float moveRoundTimer;
    /// <summary>
    /// platform的id，当场景中存在多个platform时，要根据id触发相应平台的移动
    /// </summary>
    [SerializeField] private int platformId;
    /// <summary>
    /// 平台的初始位置
    /// </summary>
    private Vector3 defaultPosition;
    /// <summary>
    /// 平台移动速度
    /// </summary>
    public Vector3 moveVelocity { get; private set; }
    /// <summary>
    /// 奇数代表正向移动，偶数代表反向移动
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
    /// 平台开始来回移动
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StartMoveRound(object sender, EventArgs e)
    {
        var _info = e as EventParameter;
        //只有当传入的int值等于platform的id值时，才能触发来回移动的功能
        if (_info.eventInt != gameObject.GetInstanceID())
            return;
        MoveTo();
    }

    /// <summary>
    /// 平台移动
    /// </summary>
    /// <param name="_moveDirection">移动的距离矢量</param>
    private void MoveTo() 
    {
        Vector3 _position = transform.position;
        //每次调用MoveTo，就将roundBack的值加一
        roundBack++;
        Vector3 _moveDirection = moveRoundDistance * (roundBack % 2f);
        transform.DOMove(_moveDirection + defaultPosition, moveRoundTimer).SetEase(Ease.Linear).OnComplete(() =>
        {
            //将相反的位移传入，不断递归
            MoveTo();
        }).
        OnUpdate(() =>
        {
            //根据前一帧的位置和这一帧的位置与时间的差值计算速度
            moveVelocity = (transform.position - _position) / Time.deltaTime;
            _position = transform.position;
        });
    }

    public Vector3 GetPlatformVelocity()
    {
        return moveVelocity;
    }
}
