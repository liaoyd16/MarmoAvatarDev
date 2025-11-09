using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class WorldPatchManager : MonoBehaviour
{
    float patch_width = 4; // by default
    
    [Tooltip("how many adjacent prefabs to pad around avatar stepping-on patch")]
    [SerializeField] int padding = 1;

    // wherever agent goes, render around agent
    // if agent is too far away from origin, reset origin
    [Tooltip("how far the avatar moves away from (0,0,0) should we re-origin everything")]
    [SerializeField] float re_origin_thres = 100;

    // topologically player is always on a torus
    public Torus<GameObject> active_patches;
    [SerializeField] Vector3 active_patch_pos; // for debug

    [Tooltip("place avatar game object here")]
    [SerializeField] GameObject agent;

    //public string prefab_name;
    [SerializeField] GameObject patch4x4_prefab;
    public Vector3 offset_in_map { get; private set; }

    // object pool
    ObjectPool<GameObject> patch4x4_pool;

    void Start()
    {
        TerrainInfo terrain_info = patch4x4_prefab.GetComponent<TerrainInfo>();
        patch_width = terrain_info.patch_wid;
        MapManager.startMapManager(patch_width);
        offset_in_map = Vector3.zero;

        // initialize pool
        patch4x4_pool = new ObjectPool<GameObject>(createFunc, actionOnRelease: actionOnRelease);

        // populate active_patches
        active_patches = new Torus<GameObject>(padding);
        populateAll();
    }

    void Update() {
        active_patch_pos = active_patches.get(0,0).transform.position;
        if (agent.transform.position.magnitude > re_origin_thres)
            reOrigin();
    }
    
    void LateUpdate()
    {
        // need to load new patches?
        Vector3 agent_leave = (agent.transform.position
             - active_patches.get(0, 0).transform.position) / patch_width;

        if (agent_leave.x > 1f) //move right
        {
            patchRight();
        }
        else if (agent_leave.x < 0f)
        {
            patchLeft();
        }
        if (agent_leave.z > 1f)
        {
            patchForward();
        }
        else if (agent_leave.z < 0f)
        {
            patchBack();
        }
    }

    GameObject createFunc()
    {
        GameObject p = Instantiate(patch4x4_prefab, transform.position + Vector3.down * 100, Quaternion.identity, this.transform);
        return p;
    }

    void actionOnRelease(GameObject p)
    {
        p.SetActive(false);
        p.transform.SetLocalPositionAndRotation(
            transform.position + Vector3.down * 100, Quaternion.identity);
    }

    void reOrigin()
    {
        offset_in_map += Vector3.right * agent.transform.position.x +
                         Vector3.forward * agent.transform.position.z; // no up-down allowed

        // minus agent transform
        for (int i = -active_patches.padding; i < active_patches.padding; i++)
        {
            for (int j = -active_patches.padding; j < active_patches.padding; j++)
            {
                active_patches.get(i, j).transform.position
                    -= agent.transform.position;
            }
        }

        agent.transform.position = Vector3.zero;
        transform.position = Vector3.zero;
    }

    void fromMapPopulateAt(int ixmap, int izmap, int ix, int iz)
    {
        // get a patch here
        GameObject patch_obj = patch4x4_pool.Get();

        // access map[ixmap][izmap]
        MapManager.getPatch(ixmap, izmap);
        foreach (GameObject patchitem in MapManager.singleton.patch_info)
        {
            // TODO: add patchitem
        }

        patch_obj.transform.localPosition =
            Vector3.right * ixmap * patch_width +
            Vector3.forward * izmap * patch_width -
            offset_in_map;

        // now, add patch_floor to active patch
        // but first do release job?
        if (active_patches.get(ix, iz))
            patch4x4_pool.Release(active_patches.get(ix, iz));

        // now can attach patch to active_patches
        patch_obj.SetActive(true);
        active_patches.set(ix, iz, patch_obj);
    }

    void populateAll()
    {
        int ixmap, izmap;
        MapManager.patchIndexOf(
            agent.transform.position.z + offset_in_map.z, 
            agent.transform.position.x + offset_in_map.x, out izmap, out ixmap);

        for (int ix = -active_patches.padding; ix < active_patches.padding; ix ++) {
            for (int iz = -active_patches.padding; iz < active_patches.padding; iz++)
            {
                fromMapPopulateAt(ixmap+ix, izmap+iz, ix, iz);
            }
        }
    }

    void patchRight()
    {
        // get patch info of rightmost colomn
        GameObject rightmost_patch = active_patches.get(active_patches.padding - 1, 0);
        Vector3 rightmost_mappos = rightmost_patch.transform.position + offset_in_map;
        int ixmap, izmap;
        MapManager.patchIndexOf(rightmost_mappos.x, rightmost_mappos.z, out ixmap, out izmap);

        // patches roll right
        active_patches.rollRight();

        // instantiate the new rightmost column
        for (int j = -active_patches.padding; j < active_patches.padding; j++)
        {
            fromMapPopulateAt(ixmap+1, izmap+j, active_patches.padding-1, j);
        }
    }

    void patchLeft()
    {
        // get patch info of leftmost colomn
        GameObject leftmost_patch = active_patches.get(-active_patches.padding, 0);
        Vector3 leftmost_mappos = leftmost_patch.transform.position + offset_in_map;
        int ixmap, izmap;
        MapManager.patchIndexOf(leftmost_mappos.x, leftmost_mappos.z, out ixmap, out izmap);

        // patches roll left
        active_patches.rollLeft();

        // instantiate the new leftmost column
        for (int j = -active_patches.padding; j < active_patches.padding; j++)
        {
            fromMapPopulateAt(ixmap - 1, izmap + j, -active_patches.padding, j);
        }
    }

    void patchForward()
    {
        // get patch info of forwardmost colomn
        GameObject fwdmost_patch = active_patches.get(0, active_patches.padding-1);
        Vector3 fwdmost_mappos = fwdmost_patch.transform.position + offset_in_map;
        int ixmap, izmap;
        MapManager.patchIndexOf(fwdmost_mappos.x, fwdmost_mappos.z, out ixmap, out izmap);

        // patches roll forward
        active_patches.rollForward();

        // instantiate the new fwdmost column
        for (int i = -active_patches.padding; i < active_patches.padding; i++)
        {
            fromMapPopulateAt(ixmap + i, izmap + 1, i, active_patches.padding-1);
        }
    }

    void patchBack()
    {
        // get patch info of backmost colomn
        GameObject backmost_patch = active_patches.get(0, -active_patches.padding);
        Vector3 backmost_mappos = backmost_patch.transform.position + offset_in_map;
        int ixmap, izmap;
        MapManager.patchIndexOf(backmost_mappos.x, backmost_mappos.z, out ixmap, out izmap);

        // patches roll back
        active_patches.rollBack();

        // instantiate the new backmost column
        for (int i = -active_patches.padding; i < active_patches.padding; i++)
        {
            fromMapPopulateAt(ixmap + i, izmap - 1, i, -active_patches.padding);
        }
    }
}