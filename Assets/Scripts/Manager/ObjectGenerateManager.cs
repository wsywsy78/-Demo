using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateProperty
{
    public int m_objectId;
    public Vector3 m_objectPosition;

    public GenerateProperty(int _objectId, Vector3 _objectPosition)
    {
        m_objectId = _objectId;
        m_objectPosition = _objectPosition;
    }
}

public class ObjectGenerateManager : MonoBehaviour
{
    public static ObjectGenerateManager Instance;


    [SerializeField] private List<GameObject> generateObject = new List<GameObject>();
    private List<GenerateProperty> generateProperty = new List<GenerateProperty>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
    }

    private void Start()
    {
        foreach(var obj  in generateObject)
        {
            generateProperty.Add(new GenerateProperty(obj.GetInstanceID(), obj.transform.position));
        }

        EventManager.Instance.AddListener(EventName.ReGenerate, ReGenerate);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.ReGenerate, ReGenerate);
    }


    /// <summary>
    /// 重新生成物品
    /// </summary>
    /// <param name="_id">物体的GetInstance值</param>
    private void ReGenerate(object sender, EventArgs e)
    {
        var _info = e as EventParameter;
        if (_info.eventInt == 0)
            return;
        StartCoroutine(StartReGenerate(_info.eventInt));
    }

    private IEnumerator StartReGenerate(int _id)
    {
        yield return new WaitForSeconds(2f);
        Vector3 _generatePos = Vector3.zero;
        bool _isContain = false;
        int _index = 0;
        foreach (var info in generateProperty)
        {
            if (info.m_objectId == _id)
            {
                _isContain = true;
                _generatePos = info.m_objectPosition;
                break;
            }
            _index++;
        }
        if (_isContain)
        {
            generateObject[_index].SetActive(true);
            generateObject[_index].transform.position = _generatePos;
        }
    }
}
