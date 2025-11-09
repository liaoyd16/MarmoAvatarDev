using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TaskDebugger : MonoBehaviour
{
    public UnityEvent boolSetEvent, boolResetEvent;

    [SerializeField] bool boolFlag;
    bool prev_boolFlag = false;

    void Start()
    {
        prev_boolFlag = boolFlag;
    }


    void Update()
    {
        if (boolFlag == true && prev_boolFlag == false)
        {
            boolSetEvent.Invoke();
            prev_boolFlag = true;
        }
        else if (boolFlag == false && prev_boolFlag == true)
        {
            boolResetEvent.Invoke();
            prev_boolFlag = false;
        }
    }
}
