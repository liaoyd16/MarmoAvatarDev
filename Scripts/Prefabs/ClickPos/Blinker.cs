using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinker : MonoBehaviour
{
    public Vector3 initial_scale;
    public Vector3 minimal_scale = Vector3.zero;
    float timestamp;
    [SerializeField] float omega = Mathf.PI;

    [Tooltip("=-1:'blink forever', >=0:'externally controlled'")]
    public float blinkFlag;

    void OnEnable()
    {
        StartCoroutine(blinkForever());
    }

    IEnumerator blinkForever()
    {
        while (true)
        {
            if (blinkFlag > 0 || blinkFlag == -1)
            {
                transform.localScale = (initial_scale - minimal_scale) * (1 - Mathf.Cos(omega * timestamp)) / 2 + minimal_scale;
                timestamp += Time.deltaTime;
                if (timestamp > 2 * Mathf.PI / omega) timestamp = 0;

                if (blinkFlag > 0) blinkFlag = blinkFlag > Time.deltaTime? blinkFlag - Time.deltaTime: 0;
            }
            else
            {
                transform.localScale = Vector3.zero;
                timestamp = 0;
            }
            yield return null;
        }
    }
}
