using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class HideTrigger : TriggerObject
{
    [SerializeField] GameObject timeline;
    [SerializeField] Animator anim;

    private void Start()
    {
        OnTriggerActive.Subscribe(_ =>
        {
            timeline.SetActive(true);
            anim.runtimeAnimatorController = null;
        }).AddTo(this);
    }
}
