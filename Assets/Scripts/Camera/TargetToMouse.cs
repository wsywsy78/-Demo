using UnityEngine;

/// <summary>
/// ������һ��target�����ϵĽű������ڽ���ҵĴ���ǹ��׼��Ļ����
/// </summary>
public class TargetToMouse : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 m_MousePos = new Vector3(InputManager.Instance.mousePosition.x, InputManager.Instance.mousePosition.y, 10);
        transform.position = Camera.main.ScreenToWorldPoint(m_MousePos);
    }
}
