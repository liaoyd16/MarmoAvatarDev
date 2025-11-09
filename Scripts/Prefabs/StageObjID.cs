using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StageObjID : MonoBehaviour
{
    static int id_counter = 0;
    public enum StageObjType
    {
        Grid, Hint, ClickBall, Misc
    }
    [SerializeField] public StageObjType m_type;
    public int soid { get; private set; }

    void OnEnable()
    {
        assignID();
    }

    void assignID()
    {
        id_counter += 1;
        soid = id_counter;
    }
}
