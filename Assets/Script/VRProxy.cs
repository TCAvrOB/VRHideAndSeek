using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRProxy : MonoBehaviour {

    [SerializeField] GameObject TrackBase;
    [SerializeField] GameObject TrackRight;
    [SerializeField] GameObject TrackLeft;
    [SerializeField] Camera TrackCamera;
    [SerializeField] SteamVR_Action_Boolean VRTrigger;

    [SerializeField] GameObject wand;

    public GameObject Base { get { return TrackBase; } }
    public GameObject Right { get { return TrackRight; } }
    public GameObject Left { get { return TrackLeft; } }
    public Camera Camera { get { return TrackCamera; } }
    public SteamVR_Action_Boolean Trigger { get { return VRTrigger; } }

    public delegate void TeleportDelegate();
    public static TeleportDelegate OnVRTeleport;

    private void LateUpdate()
    {
        wand.transform.position = Right.transform.position;
        wand.transform.rotation = Right.transform.rotation;
    }

    public void OnTeleport()
    {
        if (OnVRTeleport != null)
        {
            OnVRTeleport();
        }
    }
}
