using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Debugger : MonoBehaviour
{
    // Start is called before the first frame update
    static float EPS = 1e-2f;
    [SerializeField] UnityEvent<Transform, Vector3> triggerNewDestination;
    [SerializeField] bool dbg_active;
    Vector3 dbg_prev_position;

    void Update()
    {
        if (dbg_active && (transform.position - dbg_prev_position).magnitude > EPS)
        {
            triggerNewDestination.Invoke(transform, Vector3.zero);
            dbg_prev_position = transform.position;
        }
    }
}
