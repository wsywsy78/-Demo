using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 锁类，记录着钥匙和锁
/// </summary>
[Serializable]
public class Lock
{
    /// <summary>
    /// 一个锁的所有钥匙
    /// </summary>
    public List<GameObject> keys;
    /// <summary>
    /// 锁本身
    /// </summary>
    public GameObject value;
    /// <summary>
    /// 钥匙的id值
    /// </summary>
    private List<int> keysId;
    /// <summary>
    /// 锁的id值
    /// </summary>
    private int valuesId;
    /// <summary>
    /// 记录钥匙的id值
    /// </summary>
    private List<int> recordKeysId;

    /// <summary>
    /// 对id值进行初始化
    /// </summary>
    public void InitializationId()
    {
        keysId = new List<int>();
        foreach(var key in keys)
        {
            keysId.Add(key.GetInstanceID());
        }
        recordKeysId = new List<int>(keysId);
        valuesId = value.GetInstanceID();
    }

    /// <summary>
    /// 移去一个key的id值
    /// </summary>
    /// <param name="_id">id值</param>
    public void UnlockOneKey(int _id)
    {
        recordKeysId.Remove(_id);
    }

    /// <summary>
    /// 增加一个key的id值
    /// </summary>
    /// <param name="_id"></param>
    public void LockOneKey(int _id)
    {
        recordKeysId.Add(_id);
    }

    /// <summary>
    /// id值是否在记录键值中
    /// </summary>
    /// <param name="_id">id值</param>
    /// <returns></returns>
    public bool isInKeysId(int _id)
    {
        if(keysId.Contains(_id))
            return true;
        return false;
    }

    /// <summary>
    /// 是否集齐了所有的钥匙
    /// </summary>
    /// <returns></returns>
    public bool isUnlock()
    {
        if (recordKeysId.Count == 0)
            return true;
        return false;
    }

    /// <summary>
    /// 获取锁的id值
    /// </summary>
    /// <returns></returns>
    public int GetLockId()
    {
        return valuesId;
    }
}
