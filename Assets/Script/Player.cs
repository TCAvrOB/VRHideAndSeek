using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField] GameObject Head;
    [SerializeField] VRPlayer vr;
    [SerializeField] GameObject Model;
    [SerializeField] SkinnedMeshRenderer mesh;
    [SerializeField] GameObject CameraBase;

    public bool VRMode { get; set; }

    public float Speed
    {
        get { return speed; }
        set { speed = value; } 
    }

    public bool DisplayModel
    {
        get { return Model.activeSelf; }
        set { Model.SetActive(value); }
    }

    private float rate = 0.01f;

    private Material m;

    private void Start()
    {
        m = mesh.material;
    }

    private void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal") * speed * rate;
        float z = Input.GetAxis("Vertical") * speed * rate;

        Transform t = VRMode ? vr.CameraTransform : transform;
        Vector3 f = t.forward;
        f.y = 0;
        Vector3 r = t.right;
        r.y = 0;

        Vector3 tmp = transform.localPosition;
        Vector3 dir = f * z + r * x;
        tmp.x += dir.x;
        tmp.y += dir.y;
        tmp.z += dir.z;
        transform.localPosition = tmp;

        if (VRMode)
        {
            bool noInput = x == 0 && z == 0;
            if (!noInput)
            {
                transform.rotation = Quaternion.AngleAxis(Ultility.AbsolutelyAngle(dir), Vector3.up);
            }

            if (Input.GetButtonDown("Teleport"))
            {
                vr.Teleport(transform.position, transform.forward);
            }

            if (Input.GetButtonDown("Resume"))
            {
                Vector3 pos = vr.Camera.transform.position;
                pos.y = vr.transform.position.y + vr.Height;
                transform.position = pos;

                Vector3 rot = vr.Camera.transform.rotation.eulerAngles;
                rot.x = rot.z = 0f;
                transform.eulerAngles = rot;
            }
        }
        else
        {
            float y = Input.GetAxis("Horizontal2");
            transform.rotation = transform.rotation * Quaternion.AngleAxis(y, Vector3.up);

            float head = Input.GetAxis("Vertical2");
            CameraBase.transform.localRotation = CameraBase.transform.localRotation * Quaternion.Euler(head, 0, 0);
        }
    }

    public void SetColor(Color color)
    {
        m.SetColor("_Color", color);
    }
}
