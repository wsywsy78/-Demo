using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���࣬��¼��Կ�׺���
/// </summary>
[Serializable]
public class Lock
{
    /// <summary>
    /// һ����������Կ��
    /// </summary>
    public List<GameObject> keys;
    /// <summary>
    /// ������
    /// </summary>
    public GameObject value;
    /// <summary>
    /// Կ�׵�idֵ
    /// </summary>
    private List<int> keysId;
    /// <summary>
    /// ����idֵ
    /// </summary>
    private int valuesId;
    /// <summary>
    /// ��¼Կ�׵�idֵ
    /// </summary>
    private List<int> recordKeysId;

    /// <summary>
    /// ��idֵ���г�ʼ��
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
    /// ��ȥһ��key��idֵ
    /// </summary>
    /// <param name="_id">idֵ</param>
    public void UnlockOneKey(int _id)
    {
        recordKeysId.Remove(_id);
    }

    /// <summary>
    /// ����һ��key��idֵ
    /// </summary>
    /// <param name="_id"></param>
    public void LockOneKey(int _id)
    {
        recordKeysId.Add(_id);
    }

    /// <summary>
    /// idֵ�Ƿ��ڼ�¼��ֵ��
    /// </summary>
    /// <param name="_id">idֵ</param>
    /// <returns></returns>
    public bool isInKeysId(int _id)
    {
        if(keysId.Contains(_id))
            return true;
        return false;
    }

    /// <summary>
    /// �Ƿ��������е�Կ��
    /// </summary>
    /// <returns></returns>
    public bool isUnlock()
    {
        if (recordKeysId.Count == 0)
            return true;
        return false;
    }

    /// <summary>
    /// ��ȡ����idֵ
    /// </summary>
    /// <returns></returns>
    public int GetLockId()
    {
        return valuesId;
    }
}
