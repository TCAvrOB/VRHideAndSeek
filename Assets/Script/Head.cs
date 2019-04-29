using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonobitEngine.MonoBehaviour
{
    [SerializeField] GameObject cam;
    [SerializeField] GameObject laser;
    [SerializeField] float laserRange = 0.1f;

    public float LaserScale
    {
        get { return laserRange; }
        set { laserRange = value; }
    }

    Player lastPlayer = null;
    Player player = null;

    float hitTime = 0;
    int hitCount = 0;

    private void LateUpdate()
    {
        if (Network.Mode == Network.PlayMode.Gaint)
        {
            transform.position = cam.transform.position;
            transform.rotation = cam.transform.rotation;
        }

        Vector3 tmpScale = laser.transform.localScale;
        tmpScale.x = laserRange;
        tmpScale.z = laserRange;
        laser.transform.localScale = tmpScale;

        if (Physics.SphereCast(transform.position, laser.transform.lossyScale.x / 2f, transform.forward, out var hit))
        {
            player = hit.transform.gameObject.GetComponent<Player>();

            //Debug.Log("hit: " + hit.transform.gameObject.name + " player: " + player);
        }
        else
        {
            player = null;

            //Debug.Log("hit none");
        }

        if (lastPlayer == null && player != null)
        {
            hitTime = Time.realtimeSinceStartup;
            player.SetColor(Color.red);
        }

        if (lastPlayer != null && player == null)
        {
            Debug.LogError("Hit: " + (++hitCount) + " Time: " + (Time.realtimeSinceStartup - hitTime));
            lastPlayer.SetColor(Color.green);
        }

        lastPlayer = player;
        //Debug.Log("lastPlayer: " + lastPlayer);
    }
}
