using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{

    private static readonly Vector3[] points = {new Vector3(0,0,0), new Vector3(1,0,0), new Vector3(0,1,0), new Vector3(1,1,0), new Vector3(0,0,1), new Vector3(1,0,1), new Vector3(0,1,1), new Vector3(1,1,1)};
    private static readonly int[,] points2 = {{2, 6, 7, 3}, {4, 0, 1, 5}, {1, 0, 2, 3}, {4, 5, 7, 6}, {1, 3, 7, 5}, {4, 6, 2, 0}};
    private static readonly Vector3[] offsets = {new Vector3(0, 1, 0), new Vector3(0, -1, 0), new Vector3(0, 0, -1), new Vector3(0, 0, 1), new Vector3(1, 0, 0), new Vector3(-1, 0, 0)};
    private GameObject meshObject = new GameObject();
    private Mesh mesh;
    bool[,,] blocks;
    private Material material;

    public Chunk(bool[,,] theBlocks, int chunkX, int chunkY, int chunkZ, Material mat)
    {
        material = mat;
        blocks =  theBlocks;
        generateMesh(chunkX, chunkY, chunkZ);
    }

    public void generateMesh(int chunkX, int chunkY, int chunkZ)
    {
        mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> triangles = new List<int>();
        Vector3 pos;
        List<Vector2> UVs = new List<Vector2>();
        Vector3 offset;
        for (int x = 0; x<16; x++)
        {
            for (int z = 0; z<16; z++)
            {
                for (int y = 0; y<16; y++)
                {
                    if (blocks[x,y,z])
                    {
                        for (int index = 0; index<6; index++)
                        {
                            offset = offsets[index];
                            if ((int)offset.x+x>=0 && (int)offset.x+x < 16 && (int)offset.y+y>=0 && (int)offset.y+y < 16 && (int)offset.z+z>=0 && (int)offset.z+z < 16)
                            {
                                if (!blocks[(int)offset.x+x,(int)offset.y+y,(int)offset.z+z])
                                {
                                    for (int i = 0;i<4;i++)
                                    {
                                        pos = (points[points2[index, i]])+(new Vector3(x+chunkX, y+chunkY, z+chunkZ));
                                        verts.Add(pos);
                                        UVs.Add(new Vector2(Mathf.Floor(i/2),Mathf.Floor((i+1)%4/2)));
                                        
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
                                    pos = (points[points2[index, i]])+(new Vector3(x+chunkX, y+chunkY, z+chunkZ));
                                    verts.Add(pos);
                                    UVs.Add(new Vector2(Mathf.Floor(i/2),Mathf.Floor((i+1)%4/2)));
                                    
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
        mesh.SetVertices(verts);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, UVs);
        meshObject.AddComponent(typeof(MeshFilter));
        meshObject.GetComponents<MeshFilter>()[0].mesh = mesh;
        meshObject.AddComponent(typeof(MeshRenderer));
        meshObject.GetComponents<MeshRenderer>()[0].material = material;
        meshObject.AddComponent(typeof(MeshCollider));
        meshObject.GetComponents<MeshCollider>()[0].sharedMesh = mesh;
    }

    public bool getBlock(int x, int y, int z)
    {
        return blocks[x, y, z];
    }
    /*
    public void setBlock(int chunkX, int chunkY, int chunkZ, int x, int y, int z, bool block)
    {
        blocks[x, y, z] = block;
        generateMesh();
    }
    */
}
