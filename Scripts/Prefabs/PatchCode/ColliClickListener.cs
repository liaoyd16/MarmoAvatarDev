using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliClickListener : MonoBehaviour
{
    TerrainManager parent;
    void Start()
    {
        parent = transform.parent.parent.parent.GetComponent<TerrainManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        // check whether other has stageobj property?
        // or is not clickball
        if (!other.gameObject.TryGetComponent<StageObjID>(out StageObjID soid) ||
            soid.m_type != StageObjID.StageObjType.ClickBall)
            return;

        // tell parent i am hit
        parent.doConsume();
    }
}
