using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockManager : MonoBehaviour
{
    public static LockManager Instance;
    /// <summary>
    /// Ëø×Öµä
    /// </summary>
    public List<Lock> Locks;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
    }

    private void Start()
    {
        foreach(var _lock in Locks)
        {
            _lock.InitializationId();
        }

        EventManager.Instance.AddListener(EventName.UnlockOneKey, UnlockOneCondition);
        EventManager.Instance.AddListener(EventName.LockOneKey, LockOneCondition);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.UnlockOneKey, UnlockOneCondition);
        EventManager.Instance.RemoveListener(EventName.LockOneKey, LockOneCondition);
    }

    private void UnlockOneCondition(object sender, EventArgs e)
    {
        var _info = e as EventParameter;
        if (_info.eventInt == 0)
            return;
        foreach(var _lock in Locks)
        {
            if (!_lock.isInKeysId(_info.eventInt))
                continue;
            _lock.UnlockOneKey(_info.eventInt);
            if (!_lock.isUnlock())
                return;
            this.TriggerEvent(EventName.Unlock, new EventParameter { eventInt = _lock.GetLockId() });
            return;
        }
    }

    private void LockOneCondition(object sender, EventArgs e)
    {
        var _info = e as EventParameter;
        if(_info.eventInt == 0)
            return;
        foreach(var _lock in Locks)
        {
            if (!_lock.isInKeysId(_info.eventInt))
                continue;
            _lock.LockOneKey(_info.eventInt);
            this.TriggerEvent(EventName.Lock, new EventParameter { eventInt = _lock.GetLockId() });
            return;
        }
    }
}
