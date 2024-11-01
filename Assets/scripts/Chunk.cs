using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{

    private static readonly Vector3[] points = {new Vector3(0,0,0), new Vector3(1,0,0), new Vector3(0,1,0), new Vector3(1,1,0), new Vector3(0,0,1), new Vector3(1,0,1), new Vector3(0,1,1), new Vector3(1,1,1)};
    private static readonly int[,] points2 = {{2, 6, 7, 3}, {4, 0, 1, 5}, {0, 2, 3, 1}, {5, 7, 6, 4}, {1, 3, 7, 5}, {4, 6, 2, 0}};
    private static readonly Vector3[] offsets = {new Vector3(0, 1, 0), new Vector3(0, -1, 0), new Vector3(0, 0, -1), new Vector3(0, 0, 1), new Vector3(1, 0, 0), new Vector3(-1, 0, 0)};
    private static readonly int[,] textureNumber = {{-1, -1, -1, -1, -1, -1}, {3, 3, 3, 3, 3, 3}, {6, 6, 6, 6, 6, 6}, {7, 7, 7, 7, 7, 7}, {0, 3, 2, 2, 2, 2}, {8, 8, 8, 8, 8, 8}, {4, 4, 5, 5, 5, 5}, {1, 1, 1, 1, 1, 1}};
    //                                         air, dirt, stone, coblestone, grass, planks, logs leaves
    private GameObject meshObject;
    private Mesh mesh;
    int[,,] blocks;
    private Material material;
    Vector3 chunkPos;

    public Chunk(int[,,] theBlocks, int X, int Y, int Z, Material mat)
    {
        chunkPos = new Vector3(X*16, Y*16, Z*16);
        material = mat;
        blocks =  theBlocks;
        generateMesh();
    }

    public void generateMesh()
    {
        mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> triangles = new List<int>();
        Vector3 pos;
        List<Vector2> UVs = new List<Vector2>();
        Vector3 offset;
        int block;
        for (int x = 0; x<16; x++)
        {
            for (int z = 0; z<16; z++)
            {
                for (int y = 0; y<16; y++)
                {
                    if (blocks[x,y,z] != 0)
                    {
                        block = blocks[x,y,z];
                        for (int index = 0; index<6; index++)
                        {
                            offset = offsets[index];
                            if ((int)offset.x+x>=0 && (int)offset.x+x < 16 && (int)offset.y+y>=0 && (int)offset.y+y < 16 && (int)offset.z+z>=0 && (int)offset.z+z < 16)
                            {
                                if (blocks[(int)offset.x+x,(int)offset.y+y,(int)offset.z+z] == 0)
                                {
                                    for (int i = 0;i<4;i++)
                                    {
                                        pos = points[points2[index, i]]+new Vector3(x, y, z)+chunkPos;
                                        verts.Add(pos);
                                        UVs.Add(new Vector2(textureNumber[block, index]+Mathf.Floor(i/2),Mathf.Floor((i+1)%4/2)+15)/16f);
                                    }
                                    triangles.Add(verts.Count-4);
                                    triangles.Add(verts.Count-3);
                                    triangles.Add(verts.Count-2);
                                    triangles.Add(verts.Count-1);
                                    triangles.Add(verts.Count-4);
                                    triangles.Add(verts.Count-2);
                                }
                            }
                            else{
                                for (int i = 0;i<4;i++)
                                {
                                    pos = points[points2[index, i]]+new Vector3(x, y, z)+chunkPos;
                                    verts.Add(pos);
                                    UVs.Add(new Vector2(Mathf.Floor(i/2)+textureNumber[block, index],Mathf.Floor((i+1)%4/2)+15)/16f);
                                    
                                }
                                triangles.Add(verts.Count-4);
                                triangles.Add(verts.Count-3);
                                triangles.Add(verts.Count-2);
                                triangles.Add(verts.Count-1);
                                triangles.Add(verts.Count-4);
                                triangles.Add(verts.Count-2);
                            }
                        }

                    }
                }
            }
        }
        meshObject = new GameObject();
        if (verts.Count>0)
        {
            mesh.SetVertices(verts);
            mesh.SetTriangles(triangles, 0);
            mesh.SetUVs(0, UVs);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            meshObject.AddComponent(typeof(MeshFilter));
            meshObject.GetComponents<MeshFilter>()[0].mesh = mesh;
            meshObject.AddComponent(typeof(MeshRenderer));
            meshObject.GetComponents<MeshRenderer>()[0].material = material;
            meshObject.AddComponent(typeof(MeshCollider));
            meshObject.GetComponents<MeshCollider>()[0].sharedMesh = mesh;
        }
    }

    public int getBlock(int x, int y, int z)
    {
        return blocks[x, y, z];
    }
    
    public void setBlock(int x, int y, int z, int block)
    {
        blocks[x, y, z] = block;
        Object.Destroy(meshObject);
        generateMesh();
    }


    public void dissable()
    {
        meshObject.SetActive(false);

    }
    
    public void enable()
    {
        meshObject.SetActive(true);
    }
    
}
