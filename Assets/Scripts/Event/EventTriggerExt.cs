using System;

/// <summary>
/// ��չ����
/// </summary>
public static class EventTriggerExt
{
    /// <summary>
    /// �����¼�(�޲�)
    /// </summary>
    /// <param name="sender">�¼�Դ����</param>
    /// <param name="eventName">�¼�����</param>
    public static void TriggerEvent(this object sender, string eventName)
    {
        EventManager.Instance.TriggerEvent(eventName, sender);
    }
    /// <summary>
    /// �����¼�(�в�)
    /// </summary>
    /// <param name="sender">�¼�Դ����</param>
    /// <param name="eventName">�¼�����</param>
    public static void TriggerEvent(this object sender, string eventName, EventArgs args)
    {
        EventManager.Instance.TriggerEvent(eventName, sender, args);
    }
}
