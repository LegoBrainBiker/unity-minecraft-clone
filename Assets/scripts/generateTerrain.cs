using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generateTerrain : MonoBehaviour
{
    public Transform playerTransform;
    public GameObject world;
    public Material material;
    public Camera mainCamera;
    public Transform cursor;
    public Dictionary<Vector3,Chunk> chunks = new Dictionary<Vector3,Chunk>();
    // Start is called before the first frame update
    void Start()
    {
        for (int x = -2; x<2; x++)
        {
            for (int z = -2; z<2; z++)
            {
                for (int y = -2; y<2; y++)
                {
                    generateBlocks(x, y, z);
                }
            }
        }
    }

    private void generateBlocks(int chunkX, int chunkY, int chunkZ)
    {
        float height;
        int[,,] blocks = new int[16, 16, 16];
        for (int x = 0; x<16; x++)
        {
            for (int z = 0; z<16; z++)
            {
                height = Mathf.PerlinNoise(x/16f, z/16f)*5+4;
                for (int y = 0; y<16; y++)
                {
                    if (y+chunkY*16<height)
                        blocks[x, y, z] = 1;
                    else
                        blocks[x, y, z] = 0;
                }
            }
        }
        chunks.Add(new Vector3(chunkX, chunkY, chunkZ),new Chunk(blocks, chunkX, chunkY, chunkZ, material));
    }



    // Update is called once per frame
    void Update()
    {
        Ray raycast;
        RaycastHit hit;
        if (Input.GetButtonDown("mouse 0"))
        {
            raycast = mainCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

            if (Physics.Raycast(raycast, out hit))
            {
                if (hit.distance<4.5f)
                {
                    Vector3 pos = hit.point+hit.normal*-0.5f;
                    
                    chunks[new Vector3(Mathf.Floor(pos.x/16), Mathf.Floor(pos.y/16), Mathf.Floor(pos.z/16))].setBlock(-(int)Mathf.Floor(pos.x/16)*16+(int)Mathf.Floor(pos.x), -(int)Mathf.Floor(pos.y/16)*16+(int)Mathf.Floor(pos.y), (int)Mathf.Floor(pos.z)-(int)Mathf.Floor(pos.z/16)*16, 0);
                }
            }
        }
        raycast = mainCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        if (Physics.Raycast(raycast, out hit))
        {
            if (hit.distance<4.5f)
            {
                Vector3 pos = hit.point+hit.normal*-0.5f;
                cursor.position = new Vector3(Mathf.Floor(pos.x), Mathf.Floor(pos.y), Mathf.Floor(pos.z));
            }
            else
                cursor.position = playerTransform.position-(Vector3.down*100000000000);

        }
        else
            cursor.position = playerTransform.position-(Vector3.down*100000000000);

        if (Input.GetButtonDown("mouse 1"))
        {
            raycast = mainCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

            if (Physics.Raycast(raycast, out hit))
            {
                if (hit.distance<4.5f)
                {
                    Vector3 pos = hit.point+hit.normal*0.5f;
                    
                    chunks[new Vector3(Mathf.Floor(pos.x/16), Mathf.Floor(pos.y/16), Mathf.Floor(pos.z/16))].setBlock(-(int)Mathf.Floor(pos.x/16)*16+(int)Mathf.Floor(pos.x), -(int)Mathf.Floor(pos.y/16)*16+(int)Mathf.Floor(pos.y), (int)Mathf.Floor(pos.z)-(int)Mathf.Floor(pos.z/16)*16, 3);
                }
            }
        }
    }
}
