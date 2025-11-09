using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TaskLogic : MonoBehaviour
{
    // find ui objects: lower, fpvcamera, tpvcamera
    [SerializeField] SocketController socket;
    [SerializeField] EventCollector eventCollector;

    // delay times
    [SerializeField]
    float show_stim_delay,
        hide_stim_delay,
        fpv2tpv_delay,
        max_touch_wait,
        avatar_move_time;

    public UnityEvent On_Enter_Initial_FPV, On_Leave_Initial_FPV,
                      On_Enter_ShowStim_FPV, On_Leave_ShowStim_FPV,
                      On_Enter_HideStim_FPV, On_Leave_HideStim_FPV,
                      On_Enter_WaitTouched_TPV, On_Leave_WaitTouched_TPV,
                      On_Enter_Touched_TPV, On_Leave_Touched_TPV,
                      On_Enter_Rest_TPV, On_Leave_Rest_TPV,
                      On_Enter_NoTouch_TPV, On_Leave_NoTouch_TPV;
    public UnityEvent<Vector3> On_Enter_Touched_TPV_V3;



    public enum TaskState
    {
        Initial_FPV, ShowStim_FPV, HideStim_FPV,
        WaitTouch_TPV,
        Touched_TPV, Rest_TPV,
        NoTouch_TPV
    }
    public TaskState task_state;// { get; private set; }
    bool state_protect;

    void Start()
    {
        state_protect = false;
        EnterState(TaskState.Initial_FPV);
    }

    void EnterState(TaskState new_taskstate)
    {
        task_state = new_taskstate;

        // invoke
        doEnterInvoke(task_state);

        // start coroutine: LeaveForState(...)
        switch (new_taskstate)
        {
            case TaskState.Initial_FPV:
                StartCoroutine(LeaveForState(task_state, TaskState.ShowStim_FPV, show_stim_delay));
                break;
            case TaskState.ShowStim_FPV:
                StartCoroutine(LeaveForState(task_state, TaskState.HideStim_FPV, hide_stim_delay));
                break;
            case TaskState.HideStim_FPV:
                StartCoroutine(LeaveForState(task_state, TaskState.WaitTouch_TPV, fpv2tpv_delay));
                break;
            case TaskState.WaitTouch_TPV:
                StartCoroutine(LeaveForState(task_state, TaskState.NoTouch_TPV, max_touch_wait));
                break;
            case TaskState.Touched_TPV:
                StartCoroutine(LeaveForState(task_state, TaskState.Rest_TPV, avatar_move_time));
                break;
            case TaskState.Rest_TPV:
                StartCoroutine(LeaveForState(task_state, TaskState.Initial_FPV, fpv2tpv_delay));
                break;
            case TaskState.NoTouch_TPV:
                StartCoroutine(LeaveForState(task_state, TaskState.ShowStim_FPV, fpv2tpv_delay));
                break;
        }
    }

    IEnumerator LeaveForState(TaskState old_state, TaskState new_state, float delay)
    {
        // yield wait
        yield return new WaitForSeconds(delay);

        // check whether current state is still old_state?
        if (state_protect == false && task_state == old_state)
        { //invoke, enterstate
            state_protect = true;
            doLeaveInvoke(old_state);
            EnterState(new_state);
            state_protect = false;
        }
        else { }  //  abort
    }

    public void OnTouched(bool correct)
    {
        if (!state_protect)
        {
            // lock task_state
            // then invoke, enterstate
            if (task_state == TaskState.WaitTouch_TPV)
            {
                state_protect = true;
                if (correct) {
                    socket.SendRasp4("succ");
                }
                Debug.LogFormat("click success? {0}", correct);
                doLeaveInvoke(task_state);
                EnterState(TaskState.Touched_TPV);
                state_protect = false;
            }
        }
    }

    void doEnterInvoke(TaskState entered)
    {
        switch (entered)
        {
            case TaskState.Initial_FPV:
                On_Enter_Initial_FPV.Invoke();
                break;
            case TaskState.ShowStim_FPV:
                On_Enter_ShowStim_FPV.Invoke();
                break;
            case TaskState.HideStim_FPV:
                On_Enter_HideStim_FPV.Invoke();
                break;
            case TaskState.WaitTouch_TPV:
                On_Enter_WaitTouched_TPV.Invoke();
                break;
            case TaskState.Touched_TPV:
                On_Enter_Touched_TPV.Invoke();
                break;
            case TaskState.Rest_TPV:
                On_Enter_Rest_TPV.Invoke();
                break;
            case TaskState.NoTouch_TPV:
                On_Enter_NoTouch_TPV.Invoke();
                break;
        }
    }

    void doLeaveInvoke(TaskState leaving)
    {
        switch (leaving)
        {
            case TaskState.Initial_FPV:
                On_Leave_Initial_FPV.Invoke();
                break;
            case TaskState.ShowStim_FPV:
                On_Leave_ShowStim_FPV.Invoke();
                break;
            case TaskState.HideStim_FPV:
                On_Leave_HideStim_FPV.Invoke();
                break;
            case TaskState.WaitTouch_TPV:
                On_Leave_WaitTouched_TPV.Invoke();
                break;
            case TaskState.Touched_TPV:
                On_Leave_Touched_TPV.Invoke();
                break;
            case TaskState.Rest_TPV:
                On_Leave_Rest_TPV.Invoke();
                break;
            case TaskState.NoTouch_TPV:
                On_Leave_NoTouch_TPV.Invoke();
                break;
        }
    }
}
