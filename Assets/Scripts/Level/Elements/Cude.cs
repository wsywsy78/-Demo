using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cude : MonoBehaviour
{
    /// <summary>
    /// ������ײʱ�ĽӴ���
    /// </summary>
    private List<ContactPoint> contactPoint = new List<ContactPoint>();
    /// <summary>
    /// ������ƶ�
    /// </summary>
    /// <param name="_targetPosition">�ƶ���Ŀ��ص�</param>
    private void CubeOffset(Vector3 _targetPosition)
    {
        if (contactPoint.Count == 0)
            return;

    }

    private void OnCollisionEnter(Collision collision)
    {
        collision.GetContacts(contactPoint);
    }
}
