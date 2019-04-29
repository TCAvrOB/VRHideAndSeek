using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Folloｗ : MonoBehaviour
{
    [SerializeField] GameObject obj;

    private void Update()
    {
        transform.position = obj.transform.position;
        transform.rotation = obj.transform.rotation;
    }
}
