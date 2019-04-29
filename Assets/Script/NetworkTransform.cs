using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTransform : MonoBehaviour
{
    public Vector3 Position
    {
        set
        {
            lastPosition = transform.localPosition;
            float time = Time.realtimeSinceStartup;
            speed = (value - lastPosition) / (time - lastPositionTime);
            transform.localPosition = value;
            lastPositionTime = time;
        }
    }
    public Vector3 Rotation
    {
        set
        {
            lastRotation = transform.localRotation.eulerAngles;
            float time = Time.realtimeSinceStartup;
            angleSpeed = (value - lastRotation) / (time - lastRotationTime);
            transform.localRotation = Quaternion.Euler(value);
            lastRotationTime = time;
        }
    }

    Vector3 lastPosition;
    Vector3 speed;
    float lastPositionTime = 0;

    Vector3 lastRotation;
    Vector3 angleSpeed;
    float lastRotationTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.localPosition;
        lastRotation = transform.localRotation.eulerAngles;
    }

    //private void Update()
    //{
    //    float time = Time.realtimeSinceStartup;

    //    if (lastPositionTime > 0)
    //    {
    //        float diff = time - lastPositionTime;
    //        diff = (diff > Time.deltaTime) ? Time.deltaTime : diff ;

    //        transform.localPosition = lastPosition + diff * speed;
    //    }

    //    if (lastRotationTime > 0)
    //    {
    //        float diff = time - lastRotationTime;
    //        diff = (diff > Time.deltaTime) ? Time.deltaTime : diff;

    //        Vector3 ro = lastRotation + diff * angleSpeed;
    //        transform.localRotation = Quaternion.Euler(ro);
    //    }
    //}
}
