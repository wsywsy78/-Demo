using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialDestroyController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickUpObject"))
        {
            this.TriggerEvent(EventName.ReGenerate, new EventParameter { eventInt = other.gameObject.GetInstanceID() });
            other.gameObject.SetActive(false);
        }
    }
}
