using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherTrigger : MonoBehaviour
{
    private bool isTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (isTrigger)
            return;
        if (other.CompareTag("Player"))
        {
            Debug.Log("isTrigger");
            GetComponentInParent<PelletLauncher>()?.StartLaunch();
            isTrigger = true;
        }
    }
}
