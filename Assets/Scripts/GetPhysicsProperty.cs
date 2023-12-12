using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GetPhysicsProperty
{
    public static Vector3 prevFrameApplyVec = Vector3.zero;
    public static Vector3 prevFrameMotionVec = Vector3.zero;
    public static Vector3 prevFrameMaintainVec = Vector3.zero; 
    public static bool isJump = false;

    public static void SetPrevFrameVec(Vector3 _motionVec, Vector3 _applyVec, Vector3 _maintainVec)
    {
        prevFrameMotionVec = _motionVec;
        prevFrameMaintainVec = _maintainVec;
        prevFrameApplyVec = _applyVec;
    }
}
