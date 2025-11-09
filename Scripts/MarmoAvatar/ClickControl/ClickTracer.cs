using UnityEngine;
using UnityEngine.Events;

public class ClickTracer : MonoBehaviour
{
    [SerializeField] GameObject m_camera_obj;
    Camera m_camera;
    [SerializeField] GameObject m_plane_obj;
    public Vector3 click_pt { get; private set; }
    public UnityEvent<Vector3> onClickListeners;
    public UnityEvent<TaskEvent> onEventCollect;

    [SerializeField] MarmoActions avatar;

    void Start()
    {
        m_camera = m_camera_obj.GetComponent<Camera>();
    }

    Vector3 doRayCast(Vector3 from_pt, Vector3 to_pt, Plane plane)
    {
        Ray ray = new Ray(from_pt, to_pt - from_pt);
        float dist;
        if (plane.Raycast(ray, out dist))
        {
            return from_pt + dist * ray.direction;
        }
        else
        {
            Debug.LogWarning("error in ray cast");
            return from_pt;
        }
    }


    Vector2 calcMousePos(Vector2 event_pos, out bool click_succ)
    {
        click_succ = true;
        return new Vector2(
            event_pos.x,
            m_camera.pixelHeight - event_pos.y);
    }

    void OnGUI()
    {
        if (!avatar.avatarState.IsName("sitting")) return;

        Event curr_event = Event.current;
        if (curr_event.type != EventType.MouseDown && curr_event.type != EventType.TouchDown)
            return;
        bool click_succ;
        Vector2 mouse_pos = calcMousePos(curr_event.mousePosition, out click_succ);
        onEventCollect.Invoke(new Click2dEvent(mouse_pos));

        if (!click_succ) return;
        
        Vector3 pt = m_camera.ScreenToWorldPoint(
            new Vector3(mouse_pos.x, mouse_pos.y, 1));

        if (m_camera.orthographic)
        {
            click_pt = doRayCast(
                pt, pt + m_camera_obj.transform.rotation * Vector3.forward,
                new Plane(m_plane_obj.transform.rotation * Vector3.up,
                        m_plane_obj.transform.position));
        }
        else
        {
            click_pt = doRayCast(
                m_camera_obj.transform.position, pt,
                new Plane(m_plane_obj.transform.rotation * Vector3.up,
                        m_plane_obj.transform.position)
            );
        }

        onClickListeners.Invoke(click_pt);
    }
}
