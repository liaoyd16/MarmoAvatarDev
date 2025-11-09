using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TaskEvent
{
    public abstract string Repr();
}

public class ClickPosEvent : TaskEvent
{
    Vector3 clickpos;
    public ClickPosEvent(Vector3 clickpos_)
    {
        this.clickpos = clickpos_;
    }
    public override string Repr()
    {
        return string.Format("marmoset click pos at {0}", clickpos);
    }
}

public class AvatarMoveEvent : TaskEvent
{
    Vector3 clickpos;
    public AvatarMoveEvent(Vector3 clickpos_)
    {
        this.clickpos = clickpos_;
    }
    public override string Repr()
    {
        return string.Format("marmoset moving to {0}", clickpos);
    }
}

public class Click2dEvent : TaskEvent
{
    Vector2 clickpos;
    public Click2dEvent(Vector2 clickpos_)
    {
        clickpos = clickpos_;
    }
    public override string Repr()
    {
        return string.Format("marmoset click 2d at {0}", clickpos);
    }
}

public class EventCollector : MonoBehaviour
{
    SocketController socketController;

    void Start()
    {
        socketController = GetComponent<SocketController>();
    }

    public void onTaskEvent(TaskEvent e)
    {
        socketController.SendWorkstation(e.Repr());
    }

    public void onRewardEvent()
    {
        socketController.SendRasp4("succ");
    }
}
