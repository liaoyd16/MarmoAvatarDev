using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarmoActions : MonoBehaviour
{
    // misc
    static float DIST_EPS = 5e-2f;
    [SerializeField]
    AnimationClip turnleft_clip,
                                   turnright_clip,
                                   leapl_clip,
                                   leapr_clip;
    float turn_duration, leap_duration;

    // animator stuff 
    Animator ac;
    Dictionary<string, int> layer_of = new Dictionary<string, int>() { { "Main", 0 } };

    // variable for turning
    float angular_speed, angle_remaining;

    // variable for leaping
    [SerializeField] float raw_leap_dist = 6;
    float raw_leap_velo;
    Vector3 spd_leap;
    Vector3 terrain_offset;
    Transform terrain_transform;
    [SerializeField] bool endingOrientHintEnable = false;
    Vector3 endingOrientHint;

    public AnimatorStateInfo avatarState { get; private set; }

    void Start()
    {
        ac = GetComponent<Animator>();
        turn_duration = Mathf.Min(turnleft_clip.length, turnright_clip.length);
        leap_duration = Mathf.Min(leapl_clip.length, leapr_clip.length);

        angle_remaining = 0f;
        endingOrientHint = transform.rotation * Vector3.forward;

        spd_leap = Vector3.zero;
        terrain_offset = Vector3.zero;
        terrain_transform = transform;

        raw_leap_velo = raw_leap_dist / leap_duration;
    }

    public void onNewDestination(Transform terrain, Vector3 offset)
    {
        // todo: yield until returned to state "idle"
        AnimatorStateInfo info = ac.GetCurrentAnimatorStateInfo(layer_of["Main"]);
        if (!info.IsName("sitting")) return;

        terrain_transform = terrain;
        terrain_offset = offset;
        Vector3 dest = terrain.position + offset; // might not be true if not go in straight line

        // calculate angle, number of leaps
        Vector3 displace = new Vector3(dest.x, transform.position.y, dest.z) - transform.position;

        // angle: consider both displacement and self orientation
        float orient_angle = transform.eulerAngles.y;
        Vector3 displace_rotate = Quaternion.AngleAxis(-orient_angle, Vector3.up) * displace;
        angle_remaining = (float)Math.Atan2(displace_rotate.x, displace_rotate.z) * Mathf.Rad2Deg;

        // calculate ending orientation hint
        Vector3 myfront = transform.rotation * Vector3.forward,
                myright = transform.rotation * Vector3.right;
        float projection_onto_front = Vector3.Dot(displace, myfront);
        Vector3 deflect = displace - projection_onto_front * myfront;
        if (Vector3.Dot(deflect, myright) > 0 && Mathf.Abs(angle_remaining) > 5) {
            endingOrientHint = myright;
        }
        else if (Vector3.Dot(deflect, myright) < 0 && Mathf.Abs(angle_remaining) > 5) {
            endingOrientHint = -myright;
        }
        else if (projection_onto_front < 0) {
            endingOrientHint = -myfront;
        }
        else {
            endingOrientHint = myfront;
        }

        ac.SetFloat("turn_angle", angle_remaining);

        // rotating speed, time
        angular_speed = angle_remaining / turn_duration;

        // set number of leaps & speed vector for fixed update
        spd_leap = getNextLeapSpeed();
        if (spd_leap.magnitude > 0)
        {
            ac.SetInteger("num_leaps", 1);
        }
        else
        {
            ac.SetInteger("num_leaps", 0);
        }
    }

    bool tempflag; // remember previous animator state info
    void FixedUpdate()
    {
        AnimatorStateInfo info = ac.GetCurrentAnimatorStateInfo(layer_of["Main"]);
        if (info.IsName("turnleft") || info.IsName("turnright"))
        {
            float angle_delta = angular_speed * Time.deltaTime;
            if (angle_remaining * (angle_remaining - angle_delta) > 0) // angle_remaining would not change sign
            {
                transform.Rotate(Vector3.up, angle_delta);
                angle_remaining -= angle_delta;
            }
        }
        else if (info.IsName("leapl") || info.IsName("leapr"))
        {
            Vector3 pos_delta = spd_leap * Time.deltaTime;
            Vector3 dest = terrain_offset + terrain_transform.position;
            if ((transform.position - dest).magnitude > (transform.position + pos_delta - dest).magnitude)
            {
                transform.position += pos_delta;
            }
            else
            {
                ac.SetInteger("num_leaps", 0);
            }
        }
        else if (info.IsName("temporary"))
        {
            tempflag = true;
        }
        else if (info.IsName("sitting"))
        {
            if (tempflag)
            {
                tempflag = false;

                // todo
                // calculate angle difference
                Quaternion rotation = Quaternion.FromToRotation(
                    transform.rotation * Vector3.forward, endingOrientHint);
                // start coroutine
                if (endingOrientHintEnable)
                    StartCoroutine(SmoothRotate(rotation, turn_duration));
            }
        }
    }

    IEnumerator SmoothRotate(Quaternion rotation, float turn_duration)
    {
        Quaternion start_rot = transform.rotation,
                   end_rot = transform.rotation * rotation;

        float time_elapsed = 0f;
        while (time_elapsed < turn_duration + Time.deltaTime) // enough time for rotation
        {
            float proportion = Mathf.Min(1, time_elapsed / turn_duration); // protortion ceiling is 1
            transform.rotation = Quaternion.Slerp(start_rot, end_rot, time_elapsed / turn_duration);

            time_elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void Update()
    {
        avatarState = ac.GetCurrentAnimatorStateInfo(layer_of["Main"]);
        if (avatarState.IsName("temporary"))
        {
            spd_leap = getNextLeapSpeed();
        }
    }

    Vector3 getNextLeapSpeed()
    {
        Vector3 dest = terrain_offset + terrain_transform.position;
        Vector3 displace = new Vector3(dest.x, transform.position.y, dest.z) - transform.position;

        if (displace.magnitude > DIST_EPS)
        {
            return raw_leap_velo * displace.normalized;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
