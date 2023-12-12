using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cude : MonoBehaviour
{
    /// <summary>
    /// 方块碰撞时的接触点
    /// </summary>
    private List<ContactPoint> contactPoint = new List<ContactPoint>();
    /// <summary>
    /// 方块的移动
    /// </summary>
    /// <param name="_targetPosition">移动的目标地点</param>
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
