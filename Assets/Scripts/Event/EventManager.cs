using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件管理系统
/// </summary>
public class EventManager : MonoBehaviour
{
    /// <summary>
    /// 单例
    /// </summary>
    public static EventManager Instance;
    /// <summary>
    /// 委托字典
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
    /// 添加事件监听者
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="handler">事件委托</param>
    public void AddListener(string eventName, EventHandler handler)
    {
        if (handlerDic.ContainsKey(eventName))
            handlerDic[eventName] += handler;
        else
            handlerDic.Add(eventName, handler);
    }
    /// <summary>
    /// 删除事件监听者
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="handler"></param>
    public void RemoveListener(string eventName, EventHandler handler)
    {
        if (handlerDic.ContainsKey(eventName))
            handlerDic[eventName] -= handler;
    }
    /// <summary>
    /// 事件触发(无参数)
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="sender">事件委托</param>
    public void TriggerEvent(string eventName, object sender)
    {
        if (handlerDic.ContainsKey(eventName))
            handlerDic[eventName]?.Invoke(sender, EventArgs.Empty);
    }
    /// <summary>
    /// 事件触发(有参数)
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="sender">事件委托</param>
    public void TriggerEvent(string eventName, object sender, EventArgs args)
    {
        if (handlerDic.ContainsKey(eventName))
            handlerDic[eventName]?.Invoke(sender, args);
    }
    /// <summary>
    /// 清空所有事件
    /// </summary>
    public void Clear()
    {
        handlerDic.Clear();
    }
}
