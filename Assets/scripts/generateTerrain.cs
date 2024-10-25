using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generateTerrain : MonoBehaviour
{
    public GameObject world;
    public Material material;
    public Dictionary<Vector3,Chunk> chunks = new Dictionary<Vector3,Chunk>();
    // Start is called before the first frame update
    void Start()
    {
        generateBlocks(0, 0, 0);
    }

    private void generateBlocks(int chunkX, int chunkY, int chunkZ)
    {
        float height;
        bool[,,] blocks = new bool[16, 16, 16];
        for (int x = 0; x<16; x++)
        {
            for (int z = 0; z<16; z++)
            {
                height = Mathf.PerlinNoise(x/16f, z/16f)*5+4;
                for (int y = 0; y<16; y++)
                {
                    blocks[x, y, z] = y<height;
                }
            }
        }
        chunks.Add(new Vector3(chunkX, chunkY, chunkZ),new Chunk(blocks, chunkX, chunkY, chunkZ, material));

    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
