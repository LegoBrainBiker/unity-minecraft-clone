using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.UIElements;

public class generateTerrain : MonoBehaviour
{
    private readonly Vector3Int[] offsets = {new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0), new Vector3Int(0, 0, -1), new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0)};
    public Transform playerTransform;
    public Material material;
    public Camera mainCamera;
    public Transform cursor;
    public Dictionary<Vector3,Chunk> chunks = new Dictionary<Vector3,Chunk>();
    public List<Vector3Int> activeChunks = new List<Vector3Int>();
    // public Vector3[] treeBlockPositions = {new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 2, 0), new Vector3(0, 3, 0), new Vector3(0, 4, 0), new Vector3(0, 3, 1), new Vector3(0, 3, 2), new Vector3(0, 3, -1), new Vector3(0, 3, -2), new Vector3(1, 3, 0), new Vector3(2, 3, 0), new Vector3(-1, 3, 0), new Vector3(-2, 3, 0)};
    // public int[] treeBlockTypes = {6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7};
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
                    activeChunks.Add(new Vector3Int(x, y, z));
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
                height = Mathf.PerlinNoise(x/16f+chunkX+999999, z/16f+chunkZ+999999)*5+4;
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

        chunks.Add(new Vector3Int(chunkX, chunkY, chunkZ),new Chunk(blocks, chunkX, chunkY, chunkZ, material, getNeighborChunks(new Vector3Int(chunkX, chunkY, chunkZ))));
        updateSideChunks(new Vector3Int(chunkX, chunkY, chunkZ));
        foreach (Vector3Int offset in offsets) {
            if (chunks.ContainsKey(new Vector3Int(chunkX, chunkY, chunkZ)+offset))
                chunks[new Vector3Int(chunkX, chunkY, chunkZ)+offset].generateSideMesh(getNeighborChunks(new Vector3Int(chunkX, chunkY, chunkZ)+offset));
        }
        // update side meshes of neighboring chunks
    }

    private void updateSideChunks(Vector3Int chunkPos) {
        foreach (Vector3Int offset in offsets) {
            if (chunks.ContainsKey(chunkPos+offset))
                chunks[chunkPos+offset].generateSideMesh(getNeighborChunks(chunkPos+offset));
        }
    }

    private Chunk[] getNeighborChunks(Vector3Int chunkPos) {
        
        Chunk[] sideChunks = new Chunk[6];
        Vector3Int pos;
        for (int i = 0; i<6; i++) {
            pos = chunkPos+offsets[i];
            if (chunks.ContainsKey(pos))
                sideChunks[i] = chunks[pos];
        }
        return sideChunks;
    }

    private void setBlock(Vector3Int pos, int block) {
        Vector3Int chunkPos = new Vector3Int((int)Math.Floor(pos.x/16.0),(int)Math.Floor(pos.y/16.0),(int)Math.Floor(pos.z/16.0));
        chunks[chunkPos].setBlock(mod(pos.x,16), mod(pos.y,16), mod(pos.z,16), block, getNeighborChunks(new Vector3Int((int)Mathf.Floor(pos.x/16), (int)Mathf.Floor(pos.y/16),(int)Mathf.Floor(pos.z/16))));
        if (mod(pos.x,16) == 0) {
            chunks[chunkPos+new Vector3Int(-1, 0, 0)].generateSideMesh(getNeighborChunks(chunkPos+new Vector3Int(-1, 0, 0)));
        } else if (mod(pos.x,16) == 15) {
            chunks[chunkPos+new Vector3Int(1, 0, 0)].generateSideMesh(getNeighborChunks(chunkPos+new Vector3Int(1, 0, 0)));
        }
        if (mod(pos.y,16) == 0) {
            chunks[chunkPos+new Vector3Int(0, -1, 0)].generateSideMesh(getNeighborChunks(chunkPos+new Vector3Int(0, -1, 0)));
        } else if (mod(pos.y,16) == 15) {
            chunks[chunkPos+new Vector3Int(0, 1, 0)].generateSideMesh(getNeighborChunks(chunkPos+new Vector3Int(0, 1, 0)));
        }
        if (mod(pos.z,16) == 0) {
            chunks[chunkPos+new Vector3Int(0, 0, -1)].generateSideMesh(getNeighborChunks(chunkPos+new Vector3Int(0, 0, -1)));
        } else if (mod(pos.z,16) == 15) {
            chunks[chunkPos+new Vector3Int(0, 0, 1)].generateSideMesh(getNeighborChunks(chunkPos+new Vector3Int(0, 0, 1)));
        }
    }


    int mod(int x, int y) {
        return x-(int)Math.Floor(((double) x)/y)*y;
    }

    // Update is called once per frame
    void Update()
    {
        bool meshHasBeenEditedThisFrame = false;
        Ray raycast;
        RaycastHit hit;
        if (Input.GetButtonDown("mouse 0"))
        {
            raycast = mainCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

            if (Physics.Raycast(raycast, out hit))
            {
                if (hit.distance<4.5f)
                {
                    Vector3Int pos = Vector3Int.FloorToInt(hit.point+hit.normal*-0.5f);
                    setBlock(pos, 0);
                    meshHasBeenEditedThisFrame = true;
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
                    Vector3Int pos = Vector3Int.FloorToInt(hit.point+hit.normal*0.5f);
                    setBlock(pos, 3);
                    meshHasBeenEditedThisFrame = true;
                }
            }
        }
        if (!meshHasBeenEditedThisFrame)
        {
            List<Vector3Int> nearbyChunks = new List<Vector3Int>();

            for (int x = -4; x<4; x++)
            {
                for (int z = -4; z<4; z++)
                {
                    for (int y = -4; y<4; y++)
                    {
                        nearbyChunks.Add(new Vector3Int((int)Mathf.Floor(playerTransform.position.x/16)+x, (int)Mathf.Floor(playerTransform.position.y/16)+y, (int)Mathf.Floor(playerTransform.position.z/16)+z));
                    }
                }
            }
            for (int i=0; i<activeChunks.Count; i++)
            {
                if(!nearbyChunks.Contains(activeChunks[i]))
                {
                    chunks[activeChunks[i]].dissable();
                    activeChunks.Remove(activeChunks[i]);
                    i--;
                }
            }
            foreach (Vector3Int Pos in nearbyChunks)
            {
                if (!(meshHasBeenEditedThisFrame ||activeChunks.Contains(Pos)))
                {
                    if (chunks.ContainsKey(Pos))
                    {
                        chunks[Pos].enable();
                    }
                    else
                    {
                        meshHasBeenEditedThisFrame = true;
                        generateBlocks((int)Pos.x, (int)Pos.y, (int)Pos.z);
                    }
                    activeChunks.Add(Pos);

                }
            }
        }
    }
}
