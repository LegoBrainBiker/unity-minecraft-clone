using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Chunk
{
    // all the vertexes of a block
    private static readonly Vector3Int[] points = {new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(0,1,0), new Vector3Int(1,1,0), new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(0,1,1), new Vector3Int(1,1,1)};
    // all the vertexes for each side
    private static readonly int[,] points2 = {{2, 6, 7, 3}, {4, 0, 1, 5}, {0, 2, 3, 1}, {5, 7, 6, 4}, {1, 3, 7, 5}, {4, 6, 2, 0}};
    private static readonly Vector3Int[] offsets = {new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0), new Vector3Int(0, 0, -1), new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0)};
    private static readonly Vector3Int[] offsets2 = {new Vector3Int(0, 15, 0), new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 15), new Vector3Int(15, 0, 0), new Vector3Int(0, 0, 0)};
    private static readonly Vector3Int[] offsets3 = {new Vector3Int(0, 0, 0), new Vector3Int(0, 15, 0), new Vector3Int(0, 0, 15), new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 0), new Vector3Int(15, 0, 0)};
    private static readonly Vector3Int[] offsets2x = {new Vector3Int(1, 0, 0), new Vector3Int(1, 0, 0), new Vector3Int(1, 0, 0), new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, 1, 0)};
    private static readonly Vector3Int[] offsets2y = {new Vector3Int(0, 0, 1), new Vector3Int(0, 0, 1), new Vector3Int(0, 1, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, 0, 1), new Vector3Int(0, 0, 1)};
    private static readonly int[,] textureNumber = {{-1, -1, -1, -1, -1, -1}, {3, 3, 3, 3, 3, 3}, {6, 6, 6, 6, 6, 6}, {7, 7, 7, 7, 7, 7}, {0, 3, 2, 2, 2, 2}, {8, 8, 8, 8, 8, 8}, {4, 4, 5, 5, 5, 5}, {1, 1, 1, 1, 1, 1}};
    //                                         air, dirt, stone, coblestone, grass, planks, logs, leaves
    private GameObject meshObject;
    private Mesh mesh;
    private GameObject sideMeshObject;
    private Mesh sideMesh;

    Block[,,] blocks;
    private Material material;
    Vector3Int chunkPos;

    public Chunk(Block[,,] theBlocks, int X, int Y, int Z, Material mat, Chunk[] sideChunks)
    {
        chunkPos = new Vector3Int(X*16, Y*16, Z*16);
        material = mat;
        blocks =  theBlocks;
        generateMesh();
        generateSideMesh(sideChunks);
    }

    private void generateMesh()
    {
        Object.Destroy(meshObject);
        mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> triangles = new List<int>();
        Vector3 pos;
        List<Vector2> UVs = new List<Vector2>();
        Vector3 offset;
        Block block;
        for (int x = 0; x<16; x++)
        {
            for (int z = 0; z<16; z++)
            {
                for (int y = 0; y<16; y++)
                {
                    if (blocks[x,y,z] != Block.air)
                    {
                        block = blocks[x,y,z];
                        for (int index = 0; index<6; index++)
                        {
                            offset = offsets[index];
                            if ((int)offset.x+x>=0 && (int)offset.x+x < 16 && (int)offset.y+y>=0 && (int)offset.y+y < 16 && (int)offset.z+z>=0 && (int)offset.z+z < 16)
                            {
                                if (blocks[(int)offset.x+x,(int)offset.y+y,(int)offset.z+z] == Block.air)
                                {
                                    for (int i = 0;i<4;i++)
                                    {
                                        pos = points[points2[index, i]]+new Vector3(x, y, z)+chunkPos;
                                        verts.Add(pos);
                                        UVs.Add(new Vector2(block.textureMapings[index]+Mathf.Floor(i/2),Mathf.Floor((i+1)%4/2)+15)/16f);
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
                                /*
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
                                */
                            }
                        }

                    }
                }
            }
        }
        if (verts.Count>0)
        {
            meshObject = new GameObject();
            meshObject.name = "main mesh of "+chunkPos;
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
    public void generateSideMesh(Chunk[] sideChunks) {
        Object.Destroy(sideMeshObject);
        sideMesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> triangles = new List<int>();
        Vector3Int pos;
        Vector3Int pos2;
        List<Vector2> UVs = new List<Vector2>();
        Block block;
        Block block2;
        for (int i = 0; i<6; i++) { // block faces
            if (sideChunks[i] != null) {
                for (int x = 0; x < 16; x++) { // Position on face
                    for (int y = 0; y < 16; y++) {
                        pos = offsets2[i]+offsets2x[i]*x+offsets2y[i]*y;
                        block = blocks[pos.x,pos.y,pos.z];
                        pos2 = offsets3[i]+offsets2x[i]*x+offsets2y[i]*y;
                        block2 = sideChunks[i].blocks[pos2.x,pos2.y,pos2.z];
                        if (block != Block.air && block2 == Block.air) {
                                for (int j = 0;j<4;j++) // vertexes
                                {
                                    pos2 = points[points2[i, j]]+pos+chunkPos;
                                    verts.Add(pos2);
                                    UVs.Add(new Vector2(Mathf.Floor(j/2)+block.textureMapings[i],Mathf.Floor((j+1)%4/2)+15)/16f);
                                    
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
        if (verts.Count>0)
        {
            sideMeshObject = new GameObject();
            sideMeshObject.name = "sides mesh of "+chunkPos;
            sideMesh.SetVertices(verts);
            sideMesh.SetTriangles(triangles, 0);
            sideMesh.SetUVs(0, UVs);
            sideMesh.RecalculateNormals();
            sideMesh.RecalculateBounds();
            sideMeshObject.AddComponent(typeof(MeshFilter));
            sideMeshObject.GetComponents<MeshFilter>()[0].mesh = sideMesh;
            sideMeshObject.AddComponent(typeof(MeshRenderer));
            sideMeshObject.GetComponents<MeshRenderer>()[0].material = material;
            sideMeshObject.AddComponent(typeof(MeshCollider));
            sideMeshObject.GetComponents<MeshCollider>()[0].sharedMesh = sideMesh;
        }
    }
    public Block getBlock(int x, int y, int z)
    {
        return blocks[x, y, z];
    }
    
    public void setBlock(int x, int y, int z, Block block, Chunk[] sideChunks)
    {
        blocks[x, y, z] = block;
        generateMesh();
        generateSideMesh(sideChunks);
    }


    public void dissable()
    {
        if (meshObject != null)
            meshObject.SetActive(false);
        if (sideMeshObject != null)
            sideMeshObject.SetActive(false);

    }
    
    public void enable()
    {
        if (meshObject != null)
            meshObject.SetActive(true);
        if (sideMeshObject != null)
            sideMeshObject.SetActive(true);
        
    }
}
