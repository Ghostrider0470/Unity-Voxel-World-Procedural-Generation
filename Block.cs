using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Block
{
    
    enum CubeSide { BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK };
    public enum BlockType { GRASS, DIRT, STONE, DIAMOND, AIR };

    private delegate void CreateQuadDelegate(CubeSide side);
    private Chunk Owner;
    private GameObject Parent;
    private Vector3 Position;
    private Material CubeMaterial;
    private BlockType blockType;
    public bool IsSolid;
    //all possible vertices 
    static Vector3 p0 = new Vector3(-0.5f, -0.5f, 0.5f);
    static Vector3 p1 = new Vector3(0.5f, -0.5f, 0.5f);
    static Vector3 p2 = new Vector3(0.5f, -0.5f, -0.5f);
    static Vector3 p3 = new Vector3(-0.5f, -0.5f, -0.5f);
    static Vector3 p4 = new Vector3(-0.5f, 0.5f, 0.5f);
    static Vector3 p5 = new Vector3(0.5f, 0.5f, 0.5f);
    static Vector3 p6 = new Vector3(0.5f, 0.5f, -0.5f);
    static Vector3 p7 = new Vector3(-0.5f, 0.5f, -0.5f);
    public Block(GameObject parent, Vector3 position, Material material, BlockType block_Type, Chunk chunk)
    {
        Parent = parent;
        Owner = chunk;
        Position = position;
        CubeMaterial = material;
        blockType = block_Type;
        IsSolid = true;
        if (blockType == BlockType.AIR)
            IsSolid = false;

    }

    private Vector2[,] blockUVs =
    {
        /*GRASS TOP*/
        {
            new Vector2(0.125f, 0.375f), new Vector2(0.1875f, 0.375f),
            new Vector2(0.125f, 0.4375f), new Vector2(0.1875f, 0.4375f)
        },
        /*GRASS SIDE*/
        {
            new Vector2(0.1875f, 0.9375f), new Vector2(0.25f, 0.9375f),
            new Vector2(0.1875f, 1.0f), new Vector2(0.25f, 1.0f)
        },
        /*DIRT*/
        {
            new Vector2(0.125f, 0.9375f), new Vector2(0.1875f, 0.9375f),
            new Vector2(0.125f, 1.0f), new Vector2(0.1875f, 1.0f)
        },
        /*STONE*/
        {
            new Vector2(0, 0.9375f), new Vector2(0.0625f, 0.9375f),
            new Vector2(0, 1f), new Vector2(0.0625f, 1f)
        },
        /*DIAMOND*/
        {
            new Vector2(0.125f,0.75f), new Vector2(0.1875f,0.75f),
            new Vector2(0.125f,0.8125f), new Vector2(0.1875f,0.8125f)
        }
    };

    void CreateQuad(CubeSide side)
    {
        Mesh quadMesh = new Mesh();
        quadMesh.name = "ScriptedMesh" + side.ToString();

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] triangles = new int[6];

        //all possible UVs
        Vector2 uv00;
        Vector2 uv10;
        Vector2 uv01;
        Vector2 uv11;

        if (blockType == BlockType.GRASS && side == CubeSide.TOP)
        {
            uv00 = blockUVs[0, 0];
            uv10 = blockUVs[0, 1];
            uv01 = blockUVs[0, 2];
            uv11 = blockUVs[0, 3];
        }
        else if (blockType == BlockType.GRASS && side == CubeSide.BOTTOM)
        {
            uv00 = blockUVs[(int)(BlockType.DIRT + 1), 0];
            uv10 = blockUVs[(int)(BlockType.DIRT + 1), 1];
            uv01 = blockUVs[(int)(BlockType.DIRT + 1), 2];
            uv11 = blockUVs[(int)(BlockType.DIRT + 1), 3];
        }
        else
        {
            uv00 = blockUVs[(int)(blockType + 1), 0];
            uv10 = blockUVs[(int)(blockType + 1), 1];
            uv01 = blockUVs[(int)(blockType + 1), 2];
            uv11 = blockUVs[(int)(blockType + 1), 3];
        }

      

        switch (side)
        {
            case CubeSide.BOTTOM:
                vertices = new Vector3[] { p0, p1, p2, p3 };
                normals = new Vector3[] {Vector3.down, Vector3.down,
                                            Vector3.down, Vector3.down};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case CubeSide.TOP:
                vertices = new Vector3[] { p7, p6, p5, p4 };
                normals = new Vector3[] {Vector3.up, Vector3.up,
                                            Vector3.up, Vector3.up};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case CubeSide.LEFT:
                vertices = new Vector3[] { p7, p4, p0, p3 };
                normals = new Vector3[] {Vector3.left, Vector3.left,
                                            Vector3.left, Vector3.left};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case CubeSide.RIGHT:
                vertices = new Vector3[] { p5, p6, p2, p1 };
                normals = new Vector3[] {Vector3.right, Vector3.right,
                                            Vector3.right, Vector3.right};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case CubeSide.FRONT:
                vertices = new Vector3[] { p4, p5, p1, p0 };
                normals = new Vector3[] {Vector3.forward, Vector3.forward,
                                            Vector3.forward, Vector3.forward};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case CubeSide.BACK:
                vertices = new Vector3[] { p6, p7, p3, p2 };
                normals = new Vector3[] {Vector3.back, Vector3.back,
                                            Vector3.back, Vector3.back};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
        } //SIDE CONSTRUCTION VECTOR MATHEMATICS

        quadMesh.vertices = vertices;
        quadMesh.normals = normals;
        quadMesh.uv = uvs;
        quadMesh.triangles = triangles;

        quadMesh.RecalculateBounds();

        GameObject quad = new GameObject("Quad");
        quad.transform.position = Position;
        quad.transform.parent = Parent.transform;
        MeshFilter meshFilter = (MeshFilter)quad.AddComponent(typeof(MeshFilter));
        meshFilter.mesh = quadMesh;
        //MeshRenderer renderer = quad.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        //renderer.material = CubeMaterial;
    }

    int ConvertBlockIndexToLocal(int i)
    {
        if (i == -1)
            i = WorldEngine.ChunkSize-1;
        else if (i == WorldEngine.ChunkSize)
            i = 0;
        return i;
    }
    bool HasSolidNeighbour(int x, int y, int z)
    {
        Block[,,] chunks;
        // Checks if the the coordinate
        // is smaller than 0 or larger than chunksize
        // it means the block we are checking is in the neigboring chunk  
        if (x < 0 || x >= WorldEngine.ChunkSize || 
            y < 0 || y >= WorldEngine.ChunkSize || 
            z < 0 || z >= WorldEngine.ChunkSize)
        {
            // Trying to calculate the exact position of the exact neigbouring chunk
            Vector3 chunkPosition = this.Parent.transform.position +
                                    new Vector3(
                                        (x - (int) Position.x) * WorldEngine.ChunkSize,
                                        (y - (int) Position.y) * WorldEngine.ChunkSize,
                                        (z - (int) Position.z) * WorldEngine.ChunkSize);
            var neighbourChunkName = World.BuildChunkName(chunkPosition);

            x = ConvertBlockIndexToLocal(x);
            y = ConvertBlockIndexToLocal(y);
            z = ConvertBlockIndexToLocal(z);
            Chunk tempData;
            if (WorldEngine.ChunkData.TryGetValue(neighbourChunkName, out tempData))
            {
                chunks = tempData.chunkData;
            }
            else
                return false;
        }
        else
            chunks = Owner.chunkData;

        try
        {
            return chunks[x, y, z].IsSolid;
        }
        catch (IndexOutOfRangeException ex)
        {
        }
        return false;
    }

    public void DrawCube()
    {
       CreateQuadDelegate createQuad = new CreateQuadDelegate((side =>CreateQuad(side)));
        if(blockType == BlockType.AIR)
            return;
        if (!HasSolidNeighbour((int) Position.x, (int) Position.y, (int) Position.z + 1))
            createQuad.Invoke(CubeSide.FRONT);
        //CreateQuad(CubeSide.FRONT);
        if (!HasSolidNeighbour((int)Position.x, (int)Position.y, (int)Position.z - 1))
            createQuad.Invoke(CubeSide.BACK);
        //CreateQuad(CubeSide.BACK);
        if (!HasSolidNeighbour((int)Position.x, (int)Position.y + 1, (int)Position.z))
            createQuad.Invoke(CubeSide.TOP);
        //CreateQuad(CubeSide.TOP);
        if (!HasSolidNeighbour((int)Position.x, (int)Position.y - 1, (int)Position.z))
            createQuad.Invoke(CubeSide.BOTTOM);
        //CreateQuad(CubeSide.BOTTOM);
        if (!HasSolidNeighbour((int)Position.x - 1, (int)Position.y, (int)Position.z))
            createQuad.Invoke(CubeSide.LEFT);
        //CreateQuad(CubeSide.LEFT);
        if (!HasSolidNeighbour((int)Position.x + 1, (int)Position.y, (int)Position.z))
            createQuad.Invoke(CubeSide.RIGHT);
        //CreateQuad(CubeSide.RIGHT);
    }
} 
