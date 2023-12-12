using UnityEngine;

/// <summary>
/// 挂载在一个target物体上的脚本，用于将玩家的传送枪对准屏幕中心
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
