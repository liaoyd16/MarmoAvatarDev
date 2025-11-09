using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderPhysics : MonoBehaviour
{
    public Vector3 velocity;
    public float timeremain;
    ColliClick parent;

    void Start()
    {
        parent = transform.parent.GetComponent<ColliClick>();
        velocity = Vector3.zero;
        timeremain = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timeremain > 0f)
        {
            transform.position += velocity * Time.deltaTime;
            timeremain -= Time.deltaTime;
            if (timeremain <= 0f)
            {
                parent.judgeDestination();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // collect hit objects
        if (!other.gameObject.TryGetComponent<StageObjID>(out StageObjID objID) ||
            !(objID.m_type == StageObjID.StageObjType.Grid))
            return;

        Debug.Log(objID.soid);
        parent.addCollideOther(other);
    }
}
