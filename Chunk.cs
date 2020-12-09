using UnityEngine;
using Random = UnityEngine.Random;

public class Chunk
{
    public Material CubeMaterial;
    public Block[,,] chunkData;
    public GameObject chunk;
    public enum ChunkStatus { Draw, Done, Keep}
    public ChunkStatus Status;
    public Chunk(Vector3 position, Material material)
    {
        chunk = new GameObject(WorldEngine.BuildChunkName(position));
        chunk.transform.position = position;
        CubeMaterial = material;
        BuildChunk();
        //DrawChunk();
    }

    

    /// <summary>
    /// Generates a Chunk of blocks x,y,z in size
    /// </summary>
    /// <param name="sizeX">Size parameter which sets the width of a chunk</param>
    /// <param name="sizeY">Size parameter which sets the height of a chunk</param>
    /// <param name="sizeZ">Size parameter which sets the length of a chunk</param>
    /// <returns></returns>
    void BuildChunk()
    {
        var randomiser = Random.Range(-30, 30);
        chunkData = new Block[WorldEngine.ChunkSize,WorldEngine.ChunkSize,WorldEngine.ChunkSize];
        var diamondCount = 0;
        for (int y = 0; y < WorldEngine.ChunkSize ; y++)
        {
            for (int x = 0; x < WorldEngine.ChunkSize; x++)
            {
                for (int z = 0; z < WorldEngine.ChunkSize; z++)
                {
                    
                    Vector3 pos = new Vector3(x,y,z);
                    int worldPosition_X = (int) (x + chunk.transform.position.x);
                    int worldPosition_Y = (int) (y + chunk.transform.position.y);
                    int worldPosition_Z = (int) (z + chunk.transform.position.z);
                    //int surfaceHeight = Utils.GenerateHeight(150,80,2,0.02f,0.3f,worldPosition_X, worldPosition_Z);
                    int surfaceHeight = 100;
                    if (worldPosition_Y > surfaceHeight)
                    {
                        chunkData[x, y, z] = new Block(chunk.gameObject, new Vector3(x, y, z), CubeMaterial, Block.BlockType.AIR, this);
                    }
                    else if (worldPosition_Y == surfaceHeight)
                    {
                            chunkData[x, y, z] = new Block(chunk.gameObject, new Vector3(x, y, z), CubeMaterial, Block.BlockType.GRASS, this);
                    }
                    else
                    {
                        chunkData[x, y, z] = new Block(chunk.gameObject, new Vector3(x, y, z), CubeMaterial, Block.BlockType.STONE, this);
                    }
                    
                        //else if (worldPosition_Y < Utils.GenerateHeight(100, 3, 0.02f, 0.3f, worldPosition_X, worldPosition_Z))
                        //{
                        //    // CREATES STONE LAYER

                        //    if (Utils.FractalBrownianMotion_3D(worldPosition_X + randomiser, worldPosition_Y + randomiser, worldPosition_Z + randomiser, 1, 1f,
                        //            0.13f) > 0.15)
                        //    {
                        //        chunkData[x, y, z] = new Block(chunk.gameObject, new Vector3(x, y, z), CubeMaterial, Block.BlockType.STONE, this);
                        //    }
                        //    else
                        //    {
                        //        if (diamondCount < 5 && worldPosition_Y < (WorldEngine.WorldSize * WorldEngine.ChunkSize / 1.4))
                        //        {
                        //            chunkData[x, y, z] = new Block(chunk.gameObject, new Vector3(x, y, z), CubeMaterial,
                        //                Block.BlockType.DIAMOND, this);
                        //            diamondCount++;
                        //        }
                        //        else
                        //        {
                        //            chunkData[x, y, z] = new Block(chunk.gameObject, new Vector3(x, y, z), CubeMaterial, Block.BlockType.STONE, this);
                        //        }

                        //    }
                        //}
                        //else if (worldPosition_Y == Utils.GenerateHeight(150, 2, 0.01f, 0.5f, worldPosition_X, worldPosition_Z))
                        //{
                        //    chunkData[x, y, z] = new Block(chunk.gameObject, new Vector3(x, y, z), CubeMaterial, Block.BlockType.GRASS, this);
                        //}
                        //else if (worldPosition_Y < Utils.GenerateHeight(150, 2, 0.01f, 0.5f, worldPosition_X, worldPosition_Z))
                        //{
                        //    // CREATES DIRT LAYER with 20 percent STONE
                        //    if (Utils.FractalBrownianMotion_3D(worldPosition_X, worldPosition_Y, worldPosition_Z, 1, 0.01f,
                        //            0.3f) > 0.20)
                        //    {
                        //        chunkData[x, y, z] = new Block(chunk.gameObject, new Vector3(x, y, z), CubeMaterial, Block.BlockType.DIRT, this);
                        //    }
                        //    else
                        //    {
                        //        chunkData[x, y, z] = new Block(chunk.gameObject, new Vector3(x, y, z), CubeMaterial, Block.BlockType.STONE, this);
                        //    }

                        //}

                        //else
                        //{
                        //    chunkData[x, y, z] = new Block(chunk.gameObject, new Vector3(x, y, z), CubeMaterial, Block.BlockType.AIR, this);
                        //}
                        //chunkData[x, y, z] = new Block(chunk.gameObject, new Vector3(x, y, z), CubeMaterial, Block.BlockType.STONE, this);
                        Status = ChunkStatus.Draw;
                }
                
            }
        }
        
    }

    public void DrawChunk()
    {
        for (int y = 0; y < WorldEngine.ChunkSize; y++)
        {
            for (int x = 0; x < WorldEngine.ChunkSize; x++)
            {
                for (int z = 0; z < WorldEngine.ChunkSize; z++)
                {
                    chunkData[x, y, z].DrawCube();
                }
            }
        }
        CombineQuads();
        MeshCollider collider = chunk.gameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = chunk.GetComponent<MeshFilter>().mesh;
    }
    void CombineQuads()
    {

        //1. Combine all children meshes
        MeshFilter[] meshFilters = chunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }


        //2. Create a new mesh on the parent object
        MeshFilter mf = (MeshFilter)chunk.gameObject.AddComponent(typeof(MeshFilter));

        mf.mesh = new Mesh();


        //3. Add combined meshes on children as the parent's mesh
        mf.mesh.CombineMeshes(combine);

        //4. Create a renderer for the parent
        MeshRenderer renderer = chunk.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.material = CubeMaterial;

        //5. Delete all uncombined children
        foreach (Transform quad in chunk.transform)
        {
            GameObject.Destroy(quad.gameObject);
        }
        //CombineQuads();
    }
}
