using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    private void Start()
    {
        EventManager.Instance.AddListener(EventName.OnLoadNext, OnLoadNextLevel);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.OnLoadNext, OnLoadNextLevel);
    }

    private void OnLoadNextLevel(object sender, EventArgs e)
    {
        int _index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(++_index);
    }
}
