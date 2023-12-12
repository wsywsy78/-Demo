using System;

/// <summary>
/// 扩展方法
/// </summary>
public static class EventTriggerExt
{
    /// <summary>
    /// 触发事件(无参)
    /// </summary>
    /// <param name="sender">事件源对象</param>
    /// <param name="eventName">事件名称</param>
    public static void TriggerEvent(this object sender, string eventName)
    {
        EventManager.Instance.TriggerEvent(eventName, sender);
    }
    /// <summary>
    /// 触发事件(有参)
    /// </summary>
    /// <param name="sender">事件源对象</param>
    /// <param name="eventName">事件名称</param>
    public static void TriggerEvent(this object sender, string eventName, EventArgs args)
    {
        EventManager.Instance.TriggerEvent(eventName, sender, args);
    }
}
