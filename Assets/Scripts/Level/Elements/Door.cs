using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Door : MonoBehaviour
{
    /// <summary>
    /// ����
    /// </summary>
    [SerializeField] private Transform leftDoor;
    /// <summary>
    /// ����
    /// </summary>
    [SerializeField] private Transform rightDoor;
    /// <summary>
    /// ���ŵ�Ĭ��localXλ��
    /// </summary>
    private float leftDefault;
    /// <summary>
    /// ���ŵ�Ĭ��localXλ��
    /// </summary>
    private float rightDefault;

    private void Start()
    {
        leftDefault = leftDoor.localPosition.x;
        rightDefault = rightDoor.localPosition.x;

        EventManager.Instance.AddListener(EventName.Unlock, OpenDoor);
        EventManager.Instance.AddListener(EventName.Lock, CloseDoor);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.Unlock, OpenDoor);
        EventManager.Instance.RemoveListener(EventName.Lock, CloseDoor);
    }

    /// <summary>
    /// ����
    /// </summary>
    private void OpenDoor(object sender, EventArgs e)
    {
        var _info = e as EventParameter;
        if (_info.eventInt != gameObject.GetInstanceID())
            return;
        float _leftPos = leftDefault + 1.4f;
        float _rightPos = rightDefault - 1.4f;
        leftDoor.DOLocalMoveX(_leftPos, 0.3f).SetEase(Ease.Linear);
        rightDoor.DOLocalMoveX(_rightPos, 0.3f).SetEase(Ease.Linear);
    }

    /// <summary>
    /// �ر���
    /// </summary>
    private void CloseDoor(object sender, EventArgs e)
    {
        var _info = e as EventParameter;
        if (_info.eventInt != gameObject.GetInstanceID())
            return;
        leftDoor.DOLocalMoveX(leftDefault, 0.3f).SetEase(Ease.Linear);
        rightDoor.DOLocalMoveX(rightDefault, 0.3f).SetEase(Ease.Linear);
    }
}
