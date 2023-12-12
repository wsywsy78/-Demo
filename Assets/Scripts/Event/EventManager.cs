using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �¼�����ϵͳ
/// </summary>
public class EventManager : MonoBehaviour
{
    /// <summary>
    /// ����
    /// </summary>
    public static EventManager Instance;
    /// <summary>
    /// ί���ֵ�
    /// </summary>
    private Dictionary<string, EventHandler> handlerDic = new Dictionary<string, EventHandler>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
    }

    /// <summary>
    /// ����¼�������
    /// </summary>
    /// <param name="eventName">�¼�����</param>
    /// <param name="handler">�¼�ί��</param>
    public void AddListener(string eventName, EventHandler handler)
    {
        if (handlerDic.ContainsKey(eventName))
            handlerDic[eventName] += handler;
        else
            handlerDic.Add(eventName, handler);
    }
    /// <summary>
    /// ɾ���¼�������
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="handler"></param>
    public void RemoveListener(string eventName, EventHandler handler)
    {
        if (handlerDic.ContainsKey(eventName))
            handlerDic[eventName] -= handler;
    }
    /// <summary>
    /// �¼�����(�޲���)
    /// </summary>
    /// <param name="eventName">�¼�����</param>
    /// <param name="sender">�¼�ί��</param>
    public void TriggerEvent(string eventName, object sender)
    {
        if (handlerDic.ContainsKey(eventName))
            handlerDic[eventName]?.Invoke(sender, EventArgs.Empty);
    }
    /// <summary>
    /// �¼�����(�в���)
    /// </summary>
    /// <param name="eventName">�¼�����</param>
    /// <param name="sender">�¼�ί��</param>
    public void TriggerEvent(string eventName, object sender, EventArgs args)
    {
        if (handlerDic.ContainsKey(eventName))
            handlerDic[eventName]?.Invoke(sender, args);
    }
    /// <summary>
    /// ��������¼�
    /// </summary>
    public void Clear()
    {
        handlerDic.Clear();
    }
}
