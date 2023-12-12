using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour, IPlatform
{
    /// <summary>
    /// Ŀ�����λ��
    /// </summary>
    [SerializeField] private Vector3 targetPos;
    /// <summary>
    /// �����Ƿ�����
    /// </summary>
    private bool isOver;
    /// <summary>
    /// ���ݵ��ƶ��ٶ�
    /// </summary>
    private Vector3 platformVec;
    /// <summary>
    /// ����
    /// </summary>
    [SerializeField] private Transform glass;
    /// <summary>
    /// �����ƶ������λ��
    /// </summary>
    [SerializeField] private Vector3 glassTargetPos;
    /// <summary>
    /// ������Ĭ��λ��
    /// </summary>
    private Vector3 glassDefaultPos;

    private void Start()
    {
        glassDefaultPos = glass.transform.localPosition;
    }

    public Vector3 GetPlatformVelocity()
    {
        return platformVec;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isOver)
            return;
        if (other.CompareTag("Player"))
        {
            isOver = true;
            Vector3 _glassVec = glassDefaultPos + glassTargetPos;
            glass.DOLocalMove(_glassVec, 2.5f).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                StartMove();
            });
            
        }
    }

    private void StartMove()
    {        
        targetPos = transform.position + targetPos;
        Vector3 _position = transform.position;
        transform.DOMove(targetPos, 10f).SetEase(Ease.InOutQuad).OnUpdate(() =>
        {
            platformVec = (transform.position - _position) / Time.deltaTime;
            _position = transform.position;
        }).
        OnComplete(() =>
        {
            glass.DOLocalMove(glassDefaultPos, 2.5f).SetEase(Ease.InOutQuad);
        });
    }
}
