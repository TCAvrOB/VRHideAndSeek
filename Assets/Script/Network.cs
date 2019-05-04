using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MonobitEngine;
using UniRx;

public class Network : MonobitEngine.MunMonoBehaviour
{
    [SerializeField] GameObject normalCam;
    [SerializeField] GameObject vrCam;

    [SerializeField] GameObject Head;
    [SerializeField] GameObject Player;
    [SerializeField] GameObject FPSPos;
    [SerializeField] GameObject Sun;

    [SerializeField] World world;
    [SerializeField] VRPlayer vrPlayer;

    static PlayMode mode = PlayMode.None;

    public static PlayMode Mode { get { return mode; } }

    public enum PlayMode
    {
        None,
        Gaint,
        Mini,
        MiniVR,
    }

    NetworkTransform playerTransform;
    NetworkTransform headTransform;

    Head head;
    Player player;

    string laserScale;
    string moveSpeed;
    float sunAngle = 50f;

    void Start()
    {
        Application.targetFrameRate = 100;

        UnityEngine.XR.XRSettings.enabled = false;

        head = Head.GetComponent<Head>();
        player = Player.GetComponent<Player>();

        laserScale = head.LaserScale.ToString();
        moveSpeed = player.Speed.ToString();

        Debug.Log("connect...");
        MonobitNetwork.sendRate = 100;
        MonobitNetwork.updateStreamRate = 100;
        MonobitNetwork.autoJoinLobby = true;
        MonobitNetwork.ConnectServer("0.0.1");

        Observable.Interval(System.TimeSpan.FromMilliseconds(10)).Subscribe(l =>
        {
            if (mode == PlayMode.Gaint)
            {
                monobitView.RpcSecure("GaintTransform", MonobitTargets.Others, false, false, Head.transform.localPosition, Head.transform.localRotation.eulerAngles);
                //monobitView.RPC("GaintTransform", MonobitTargets.Others, Head.transform.localPosition, Head.transform.localRotation.eulerAngles);
            }

            if (mode == PlayMode.Mini)
            {
                monobitView.RpcSecure("MiniTransform", MonobitTargets.Others, false, false, Player.transform.localPosition);
                //monobitView.RPC("MiniTransform", MonobitTargets.Others, Player.transform.localPosition);
            }
        }).AddTo(this);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("On left room.");

        Observable.Timer(System.TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            Debug.Log("reconnect...");
            MonobitNetwork.ConnectServer("0.0.1");
        }).AddTo(this);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("On join lobby.");
        MonobitNetwork.JoinOrCreateRoom("TCAvrOB", new RoomSettings(), LobbyInfo.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("On join room.");
    }

    [MunRPC]
    void MiniTransform(Vector3 position)
    {
        //Debug.Log("MiniTransform: " + position + " time: " + Time.realtimeSinceStartup);
        if (playerTransform == null)
        {
            Debug.Log("MiniTransform Error: " + playerTransform);
            return;
        }
        playerTransform.Position = position;
    }

    [MunRPC]
    void GaintTransform(Vector3 position, Vector3 rotation)
    {
        //Debug.Log("GaintTransform: " + position + " " + rotation + " time: " + Time.realtimeSinceStartup);
        if (headTransform == null)
        {
            Debug.Log("GaintTransform Error: " + headTransform);
            return;
        }
        headTransform.Position = position;
        headTransform.Rotation = rotation;
    }

    [MunRPC]
    void SetLaser(float v)
    {
        Debug.Log("SetLaser: " + v);
        head.LaserScale = v;
    }

    [MunRPC]
    void SetSpeed(float v)
    {
        Debug.Log("SetSpeed: " + v);
        player.Speed = v;
    }

    private void OnGUI()
    {
        if (MonobitNetwork.inRoom == false)
        {
            GUILayout.Label("Connecting Server...");
            return;
        }

        GUILayout.Label("State online.");

        if (GUILayout.Button("Gaint"))
        {
            normalCam.SetActive(false);
            vrCam.SetActive(true);
            UnityEngine.XR.XRSettings.enabled = true;

            mode = PlayMode.Gaint;
            if (playerTransform == null)
            {
                playerTransform = Player.AddComponent<NetworkTransform>();
            }

            world.SetVR(false);
            player.VRMode = false;
        }

        if (GUILayout.Button("Mini"))
        {
            normalCam.SetActive(true);
            vrCam.SetActive(false);
            UnityEngine.XR.XRSettings.enabled = false;

            mode = PlayMode.Mini;
            if (headTransform == null)
            {
                headTransform = Head.AddComponent<NetworkTransform>();
            }
            normalCam.transform.parent = FPSPos.transform;
            normalCam.transform.localPosition = Vector3.zero;
            normalCam.transform.localRotation = Quaternion.identity;

            world.SetVR(false);
            player.VRMode = false;
        }

        if (GUILayout.Button("MiniVR"))
        {
            normalCam.SetActive(false);
            vrCam.SetActive(true);
            UnityEngine.XR.XRSettings.enabled = true;

            mode = PlayMode.MiniVR;
            if (headTransform == null)
            {
                headTransform = Head.AddComponent<NetworkTransform>();
            }

            world.SetVR(true);
            player.VRMode = true;
            vrPlayer.Init();
        }

        GUILayout.Label("Mode: " + mode);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Laser: ");
        laserScale = GUILayout.TextField(laserScale, GUILayout.MinWidth(200));
        if (GUILayout.Button("Set"))
        {
            if (float.TryParse(laserScale, out float v))
            {
                monobitView.RPC("SetLaser", MonobitTargets.All, v);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Speed: ");
        moveSpeed = GUILayout.TextField(moveSpeed, GUILayout.MinWidth(200));
        if (GUILayout.Button("Set"))
        {
            if (float.TryParse(moveSpeed, out float v))
            {
                monobitView.RPC("SetSpeed", MonobitTargets.All, v);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("Sun: ");
        sunAngle = GUILayout.HorizontalSlider(sunAngle, 0f, 360f);
        Sun.transform.rotation = Quaternion.Euler(sunAngle, -30f, 0f);
    }
}
