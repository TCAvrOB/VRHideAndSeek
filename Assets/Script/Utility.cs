using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ultility
{
    public static float AbsolutelyAngle(Vector3 forward)
    {
        forward.y = 0f;
        float a = Vector3.Angle(Vector3.forward, forward);
        if (forward.x < 0)
        {
            a = 360f - a;
        }

        return a;
    }
}
