using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Laser : MonoBehaviour
{
    [SerializeField] SteamVR_Input_Sources HandType;
    [SerializeField] SteamVR_Action_Boolean Trigger;
    TriggerObject hitWall;

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 1000, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out var hit))
        {
            var wall = hit.transform.gameObject.GetComponent<TriggerObject>();
            Debug.Log(hit.transform.gameObject);
            if (wall != null)
            {
                if (wall != hitWall)
                {
                    wall.OnLaserOver();
                    if (hitWall != null)
                    {
                        hitWall.OnLaserOut();
                    }
                    hitWall = wall;
                }
            }
            else if (hitWall != null)
            {
                hitWall.OnLaserOut();
                hitWall = null;
            }
        }
        else if (hitWall != null)
        {
            hitWall.OnLaserOut();
            hitWall = null;
        }

        if (Trigger.GetStateDown(HandType) && hitWall != null)
        {
            hitWall.OnTrigger();
        }
    }
}
