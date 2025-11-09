using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class ColliClick : MonoBehaviour
{
    GameObject m_collider;
    ColliderPhysics m_collider_phy;
    Blinker m_click_ball;
    [SerializeField][Range(0f, 10f)] float collider_travel_dist;
    [SerializeField][Range(.1f, 1f)] float collider_travel_time = .5f;
    [SerializeField][Range(1f, 10f)] float blinker_time = 2f;
    
    public UnityEvent<Transform, Vector3> ClickTrigger;
    [SerializeField] bool clickpos_quantize = false;

    void Start()
    {
        m_collider = transform.Find("Collider").gameObject;
        m_collider_phy = m_collider.GetComponent<ColliderPhysics>();
        repositCollider();

        // click ball
        m_click_ball = transform.Find("StimBall").gameObject.GetComponent<Blinker>();
        m_click_ball.blinkFlag = 0;
    }
    
    /* Use collider component to detect stepping on posts */
    /* let collision happen during kinematic process */
    /* rather than instantly moving click ball */

    public void onNewPosition(Vector3 clickpos)
    {
        resetCollisionList();
        
        // begin detection
        transform.position = clickpos;
        repositCollider();
        dropCollider(clickpos);

        // blink begin
        m_click_ball.blinkFlag = blinker_time;
    }
    
    void repositCollider()
    {
        m_collider.transform.position = transform.position + Vector3.up * collider_travel_dist / 2f;
    }

    void dropCollider(Vector3 clickpos)
    {
        // move from y = collider_travel_dist / 2f 
        // to y = -collider_travel_dist / 2f
        // in 0.5s?
        m_collider_phy.velocity = Vector3.down * collider_travel_dist / collider_travel_time;
        m_collider_phy.timeremain = collider_travel_time;
    }

    List<Collider> collision_list;
    void resetCollisionList()
    {
        collision_list = new List<Collider>();
    }
    public void addCollideOther(Collider other)
    {
        collision_list.Add(other);
    }
    public void judgeDestination()
    {
        Debug.Log("judge destination");
        if (!clickpos_quantize) 
            ClickTrigger.Invoke(transform, Vector3.zero);
        else
        {
            // TODO: which one is closest to click pos (transform.position)?
            float distmin = Mathf.Infinity;
            Collider collider_min = null;
            foreach(Collider collider in collision_list)
            {
                float dist = (collider.transform.position - transform.position).magnitude;
                if (dist < distmin)
                {
                    collider_min = collider;
                    distmin = dist;
                }
            }
            if (collider_min != null) 
                ClickTrigger.Invoke(collider_min.transform, Vector3.zero);
        }
    }
}
