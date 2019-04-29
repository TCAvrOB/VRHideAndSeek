using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class WallTracker : MonoBehaviour
{
    [SerializeField] List<WallTrigger> walls;
    [SerializeField] GameObject timeline;

    // Start is called before the first frame update
    void Start()
    {
        walls.ForEach(w => w.OnTriggerActive.Subscribe(_ => 
        {
            if (timeline.activeSelf == false)
            {
                timeline.SetActive(true);
            }
        }).AddTo(this));
    }
}
