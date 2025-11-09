using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TerrainInfo))]
public class TerrainManager : MonoBehaviour
{
    float sidelen;
    Transform[][] brickpos4x4;
    
    bool[][] occupied;
    int consumption;

    Transform terrains;
    Transform[] terrain_blocks;
    [SerializeField] Material clicked_material;
    public UnityEvent onRewardSucc;

    void Start()
    {
        TerrainInfo terrain_info = GetComponent<TerrainInfo>();
        sidelen = terrain_info.patch_wid;

        brickpos4x4 = new Transform[4][];
        for (int i = 0; i < 4; i++)
        {
            brickpos4x4[i] = new Transform[4];
            for (int j = 0; j < 4; j++)
            {
                brickpos4x4[i][j] = transform.Find(
                    string.Format("Floor4x4/Brick{0}{1}", i, j));
                brickpos4x4[i][j].localPosition.Set((i + .5f) * sidelen / 4, 0, (j + .5f) * sidelen / 4);
                brickpos4x4[i][j].localScale.Set(sidelen / 4, .1f, sidelen / 4);
            }
        }

        consumption = 1;

        Transform ground = transform.Find("Floor4x4/Ground");
        ground.localPosition.Set(sidelen/2, 0, sidelen/2);
        ground.localScale.Set(sidelen, .1f, sidelen);

        // special terrain
        occupied = new bool[4][];
        for (int i = 0; i < 4; i++)
        {
            occupied[i] = new bool[4];
        }
        terrains = transform.Find("TerrainManager");
        terrain_blocks = new Transform[terrains.childCount];
        for (int i = 0; i < terrains.childCount; i++)
            terrain_blocks[i] = terrains.GetChild(i);

        genTerrain();
    }

    void genTerrain()
    {
        // pick place
        int pick_x = UnityEngine.Random.Range(0, 4);
        int pick_z = UnityEngine.Random.Range(0, 4);

        // occupy 1 in 4 direction
        occupied[pick_x][pick_z] = true;

        switch (UnityEngine.Random.Range(0, 4))
        {
            case 0:
                occupied[pick_x + (pick_x==3?-1:1)][pick_z] = true;
                break;
            case 1:
                occupied[pick_x - (pick_x==0?-1:1)][pick_z] = true;
                break;
            case 2:
                occupied[pick_x][pick_z + (pick_z==3?-1:1)] = true;
                break;
            case 3:
                occupied[pick_x][pick_z - (pick_z==0?-1:1)] = true;
                break;
        }

        int _count = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (occupied[i][j])
                {
                    terrain_blocks[_count].localPosition = new Vector3(i, 0, j) * sidelen / 4;
                    _count++;
                }
            }
        }
    }

    public void doConsume()
    {
        if (consumption == 0) 
            return;
        
        consumption -= 1;
        if (consumption == 0)
        {
            foreach (Transform terr in terrain_blocks)
            {
                terr.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = clicked_material;
                // emit signal
                onRewardSucc.Invoke();
            }
        }
    }
}