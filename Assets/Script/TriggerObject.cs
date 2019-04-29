using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class TriggerObject : MonoBehaviour
{
    [SerializeField] Material overMat;

    private Subject<Unit> triggerSubject = new Subject<Unit>();
    public IObservable<Unit> OnTriggerActive { get { return triggerSubject; } }

    protected Material oriMat;
    protected Collider coll;

    void Awake()
    {
        MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
        oriMat = renderer.material;

        coll = GetComponent<Collider>();
    }

    protected void ChangeMaterial(Material m)
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        Array.ForEach(renderers, r =>
        {
            r.material = m;
        });
    }

    public void OnLaserOver()
    {
        //Debug.Log("OnLaserOver");
        if (coll.enabled == false)
        {
            return;
        }

        ChangeMaterial(overMat);
    }

    public void OnLaserOut()
    {
        //Debug.Log("OnLaserOut");
        if (coll.enabled == false)
        {
            return;
        }

        ChangeMaterial(oriMat);
    }

    public void OnTrigger()
    {
        triggerSubject.OnNext(Unit.Default);
    }
}
