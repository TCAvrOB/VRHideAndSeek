using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] Vector3 VRPostion;
    [SerializeField] Vector3 VRScale;

    public void SetVR(bool apply)
    {
        if (apply)
        {
            transform.position = VRPostion;
            transform.localScale = VRScale;
        }
        else
        {
            transform.position = Vector3.zero;
            transform.localScale = Vector3.one;
        }
    }
}
