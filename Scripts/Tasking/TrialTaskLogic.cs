using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class TrialTaskLogic : MonoBehaviour
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
        avatar_move_time,
        infinity_time;

    public UnityEvent On_Enter_Initial_FPV, On_Leave_Initial_FPV,
                      On_Enter_ShowStim_FPV, On_Leave_ShowStim_FPV,
                      On_Enter_HideStim_FPV, On_Leave_HideStim_FPV,
                      On_Enter_WaitTouched_TPV, On_Leave_WaitTouched_TPV,
                      On_Enter_Touched_TPV, On_Leave_Touched_TPV,
                      On_Enter_Rest_TPV, On_Leave_Rest_TPV,
                      On_Enter_NoTouch_TPV, On_Leave_NoTouch_TPV,
                      On_Enter_BlackCam, On_Leave_BlackCam;

    public enum TaskState
    {
        Initial_FPV, ShowStim_FPV, HideStim_FPV,
        WaitTouch_TPV,
        Touched_TPV, Rest_TPV,
        NoTouch_TPV,
        BlackCam
    }
    public TaskState task_state;// { get; private set; }
    public bool state_protect { get; private set; }

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
                StartCoroutine(LeaveForState(task_state, TaskState.BlackCam, avatar_move_time));
                break;
            case TaskState.BlackCam:
                // wait infinitely long
                StartCoroutine(LeaveForState(task_state, TaskState.Initial_FPV, infinity_time));
                break;
        }
    }

    IEnumerator LeaveForState(TaskState old_state, TaskState new_state, float delay)
    {
        // yield wait
        yield return new WaitForSeconds(delay);

        // check whether current state is still old_state?
        if (state_protect == false && task_state == old_state)
        {
            state_protect = true;
            // leave todos
            doLeaveInvoke(old_state);
            // invoke, enterstate
            EnterState(new_state);
            state_protect = false;
        }
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
                if (correct)
                {
                    socket.SendRasp4("succ");
                }
                Debug.LogFormat("click success? {0}", correct);
                doLeaveInvoke(TaskState.WaitTouch_TPV);
                EnterState(TaskState.Touched_TPV);
                state_protect = false;
            }
        }
    }

    public void OnTouchOffTargets()
    {
        Debug.Log(task_state);
        if (!state_protect)
        {
            if (task_state == TaskState.WaitTouch_TPV)
            {
                state_protect = true;
                doLeaveInvoke(TaskState.WaitTouch_TPV);
                EnterState(TaskState.NoTouch_TPV);
                state_protect = false;
            }
        }
    }

    void OnGUI()
    {
        if (task_state != TaskState.BlackCam) return;

        Event curr_event = Event.current;
        if (curr_event.type != EventType.MouseDown && curr_event.type != EventType.TouchDown)
            return;

        if (!state_protect)
        {
            state_protect = true;
            doLeaveInvoke(TaskState.BlackCam);
            EnterState(TaskState.Initial_FPV);
            state_protect = false;
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
            case TaskState.BlackCam:
                On_Enter_BlackCam.Invoke();
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
            case TaskState.BlackCam:
                On_Leave_BlackCam.Invoke();
                break;
        }
    }

}
