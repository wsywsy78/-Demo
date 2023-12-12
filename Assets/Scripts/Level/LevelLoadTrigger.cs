using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoadTrigger : MonoBehaviour
{
    private bool isLoad;
    
    private void OnTriggerEnter(Collider other)
    {
        if (isLoad)
            return;
        if (other.CompareTag("Player"))
        {
            this.TriggerEvent(EventName.OnLoadNext);
            isLoad = true;
        }
    }
}
