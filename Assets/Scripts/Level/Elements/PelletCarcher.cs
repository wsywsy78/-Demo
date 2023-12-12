using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PelletCarcher : MonoBehaviour
{
    /// <summary>
    /// ��Ӧ�Ĵ����������idֵ
    /// </summary>
    [SerializeField] private int relativeObjectId;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pellet"))
        {
            this.TriggerEvent(EventName.DestroyPellet, new EventParameter { eventInt = other.gameObject.GetInstanceID() });
            this.TriggerEvent(EventName.UnlockOneKey, new EventParameter { eventInt = gameObject.GetInstanceID() });
            //this.TriggerEvent(EventName.StartMovePlatform, new EventParameter { eventInt = relativeObjectId});
        }
    }
}
