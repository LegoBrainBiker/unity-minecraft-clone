using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.UIElements;

public class generateTerrain : MonoBehaviour
{
    public Transform playerTransform;
    public GameObject world;
    public Material material;
    public Camera mainCamera;
    public Transform cursor;
    public Dictionary<Vector3,Chunk> chunks = new Dictionary<Vector3,Chunk>();
    public List<Vector3> activeChunks = new List<Vector3>();
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
                    activeChunks.Add(new Vector3(x, y, z));
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
                    {
                        if (y+chunkY*16>height-1)
                            blocks[x, y, z] = 4;
                        else
                        {
                            if (y+chunkY*16>height-6)
                                blocks[x, y, z] = 1;
                            else
                                blocks[x, y, z] = 2;
                        }
                    }
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
            List<Vector3> nearbyChunks = new List<Vector3>();

        for (int x = -2; x<2; x++)
        {
            for (int z = -2; z<2; z++)
            {
                for (int y = -2; y<2; y++)
                {
                    nearbyChunks.Add(new Vector3(Mathf.Floor(playerTransform.position.x/16)+x, Mathf.Floor(playerTransform.position.y/16)+y, Mathf.Floor(playerTransform.position.z/16)+z));
                }
            }
        }
        for (int i = 0; i<activeChunks.Count; i++)
        foreach (Vector3 Pos in activeChunks)
        {
            if(!nearbyChunks.Contains(Pos))
            {
                chunks[Pos].dissable();
                activeChunks.Remove(Pos);
                i--;
            }
        }
        foreach (Vector3 Pos in nearbyChunks)
        {
            if (!activeChunks.Contains(Pos))
            {
                Debug.Log("enabling "+Pos);
                if (chunks.ContainsKey(Pos))
                {
                    chunks[Pos].enable();
                    Debug.Log("reloading");
                }
                else
                {
                    generateBlocks((int)Pos.x, (int)Pos.y, (int)Pos.z);
                    Debug.Log("generating");
                }
                activeChunks.Add(Pos);

            }
        }


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
