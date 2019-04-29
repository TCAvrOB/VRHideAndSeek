using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 1f;

    public float Speed
    {
        get { return speed; }
        set { speed = value; } 
    }

    private float rate = 0.01f;

    private Material m;

    private void Start()
    {
        m = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal") * speed * rate;
        float z = Input.GetAxis("Vertical") * speed * rate;

        Vector3 tmp = transform.localPosition;
        Vector3 dir = transform.forward * z + transform.right * x;
        tmp.x += dir.x;
        tmp.y += dir.y;
        tmp.z += dir.z;
        transform.localPosition = tmp;

        float y = Input.GetAxis("Horizontal2");
        //float y = Input.GetAxis("Vertical2");
        transform.rotation = transform.rotation * Quaternion.AngleAxis(y, Vector3.up);
    }

    public void SetColor(Color color)
    {
        m.SetColor("_Color", color);
    }
}
