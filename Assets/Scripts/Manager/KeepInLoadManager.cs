using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepInLoadManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> keepInLoad;

    private void Awake()
    {
        foreach(var obj in keepInLoad)
        {
            DontDestroyOnLoad(obj);
        }
    }
}
