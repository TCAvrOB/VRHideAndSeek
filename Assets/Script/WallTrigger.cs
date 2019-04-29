using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class WallTrigger : TriggerObject
{
    [SerializeField] Material triggerMat;

    static List<WallTrigger> objs = new List<WallTrigger>();

    void Start()
    {
        objs.Add(this);

        OnTriggerActive.Subscribe(_ => 
        {
            objs.ForEach(w => w.OnTriggerEnd());
            Collider[] colls = GetComponentsInChildren<Collider>();
            colls.ToList().ForEach(c => c.enabled = false);
            ChangeMaterial(triggerMat);
        }).AddTo(this);
    }

    public void OnTriggerEnd()
    {
        Collider[] colls = GetComponentsInChildren<Collider>();
        colls.ToList().ForEach(c => c.enabled = true);
        ChangeMaterial(oriMat);
    }

    public static void AllOnTriggerEnd()
    {
        objs.ForEach(w => w.OnLaserOut());
    }

    private void OnDestroy()
    {
        objs.Remove(this);
    }
}
