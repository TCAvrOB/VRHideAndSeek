using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPlayer : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] GameObject VRCamera;
    [SerializeField] float PlayerHeight = 0.08f;

    public Transform CameraTransform { get { return VRCamera.transform; } }

    private void Start()
    {
        VRProxy.OnVRTeleport += () =>
        {
            Vector3 pos = VRCamera.transform.position;
            pos.y = transform.position.y + PlayerHeight;
            Player.transform.position = pos;

            float cameraAngle = Ultility.AbsolutelyAngle(VRCamera.transform.forward);
            Player.transform.rotation = Quaternion.AngleAxis(cameraAngle, Vector3.up);
        };
    }

    public void Init()
    {
        Vector3 pos = Player.transform.position;
        pos.x = VRCamera.transform.position.x;
        pos.z = VRCamera.transform.position.z;
        Player.transform.position = pos;
    }

    public void Teleport(Vector3 pos, Vector3 forward)
    {
        float playerAngle = Ultility.AbsolutelyAngle(forward);
        float cameraAngle = Ultility.AbsolutelyAngle(VRCamera.transform.forward);
        float baseAngle = Ultility.AbsolutelyAngle(transform.forward);

        transform.rotation = Quaternion.AngleAxis(baseAngle + playerAngle - cameraAngle, Vector3.up);

        Vector3 diff = pos - VRCamera.transform.position;
        diff.y = 0f;
        transform.position += diff;
    }
}
