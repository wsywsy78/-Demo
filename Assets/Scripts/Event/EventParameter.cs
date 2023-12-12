using System;
using UnityEngine;

/// <summary>
/// �¼������ĸ��ֲ���
/// </summary>
public class EventParameter : EventArgs
{
    public int eventInt;
    public InPortal eventGetInPortal;
    public int eventGetOutPortal;
    public Transform[] eventTransforms;
    public Vector3 maintainVec;
    public bool isOpenDoor;
    public struct InPortal
    {
        public Collider objectCollider;
        public Collider attachCollider;
        public Transform inportal;
        public Transform outportal;
    }
}
