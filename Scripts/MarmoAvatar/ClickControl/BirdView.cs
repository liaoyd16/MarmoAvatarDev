
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BirdView : MonoBehaviour
{
    [SerializeField] Vector3 cam_rotation;
    GameObject m_camera_obj;
    Camera m_camera;
    [SerializeField] bool isFirstPersonView;
    [SerializeField] GameObject agent;
    [SerializeField] Vector3 cam_backoff;

    void Start()
    {
        m_camera_obj = transform.Find("BirdViewCam").gameObject;
        m_camera = m_camera_obj.GetComponent<Camera>();
    }

    void Update()
    {
        transform.position = agent.transform.position;
        if (isFirstPersonView)
        {
            transform.rotation = agent.transform.rotation;
        }

        m_camera_obj.transform.localRotation = Quaternion.Euler(cam_rotation);
        m_camera_obj.transform.localPosition = cam_backoff;
    }
}
