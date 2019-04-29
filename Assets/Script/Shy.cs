using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shy : MonoBehaviour
{
    [SerializeField] float range = 1f;
    [SerializeField] float period = 5f;
    [SerializeField] GameObject center;
    Camera cam;
    Animator anim;
    bool alert = false;
    float alertTime;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        cam = GameObject.FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.up * range);
        if (Vector3.Distance(transform.position, cam.transform.position) < range)
        {
            if (alert == false)
            {
                alert = true;
            }
            
            if (alert)
            {
                Vector3 dir = new Vector3(center.transform.position.x - cam.transform.position.x, 0, center.transform.position.z - cam.transform.position.z);
                float angle = Vector3.Angle(dir, Vector3.right);
                center.transform.rotation = Quaternion.Euler(0, angle, 0);
            }

            alertTime = Time.realtimeSinceStartup;
        }
        else
        {
            if (alert == true && Time.realtimeSinceStartup - alertTime > period)
            {
                alert = false;
            }
        }
    }
}
