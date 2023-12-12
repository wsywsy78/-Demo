using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlatform
{
    /// <summary>
    /// 得到平台的速度
    /// </summary>
    /// <returns></returns>
    Vector3 GetPlatformVelocity();
}
