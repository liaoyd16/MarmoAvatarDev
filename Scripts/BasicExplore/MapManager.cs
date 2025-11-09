
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;


public class MapManager
{
    public static MapManager singleton { get; private set; }
    // how big is a patch
    public float patch_wid { get;  private set; }

    // a buffer to write in: 
    // what objects are to be instantiated here?
    public List<GameObject> patch_info { get; private set; }
    
    // static initiation
    public static void startMapManager(float wid)
    {
        if (singleton != null) return;
        singleton = new MapManager(wid);
    }

    private MapManager(float wid)
    {
        patch_wid = wid;
        patch_info = new List<GameObject>();
    }

    public static void getPatch(int iz, int ix)
    {
        singleton.patch_info = new List<GameObject>(); // debug: simple case, no object here
    }

    public static void patchIndexOf(float z, float x, out int iz, out int ix)
    {
        iz = Mathf.FloorToInt(z / singleton.patch_wid);
        ix = Mathf.FloorToInt(x / singleton.patch_wid);
    }
}