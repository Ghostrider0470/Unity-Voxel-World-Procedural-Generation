using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WorldEngine : MonoBehaviour
{
    #region Editor Exposed Variables
    //Used to allocate a player gameobject from Unity
    public GameObject Player;
    //Used to allocate a texture material from Unity
    public Material TextureAtlas;
    #endregion

    #region Public Static Exposed Variables

    //public static int WorldSize = 1;
    /// <summary>
    /// ColumnHeight defines how high the world is going to be in chunks
    /// </summary>
    public static int ColumnHeight = 1;

    /// <summary>
    /// ChunkSize value defines height, width and length of a chunk in blocks
    /// </summary>
    public static int ChunkSize = 32;

    /// <summary>
    /// Radius value defines the radius in which new chunks are built around the player
    /// </summary>
    public static int Radius = 1;

    /// <summary>
    /// ChunkData is dictionary that holds the information about all generated chunks
    /// </summary>
    public static ConcurrentDictionary<string, Chunk> ChunkData;

    /// <summary>
    /// FirstBool is used at initial generation of the world to prevent and trigger certain effects
    /// </summary>
    public bool FirstBuild = true;
    #endregion


    private Vector3 lastBuildPosition;
    private CoroutineQueue queue;
    private uint maxCorutines = 2000;
    private List<string> chunkRemovaList = new List<string>();

    /// <summary>
    /// Builds a unique standardized chunk name from its position vector
    /// </summary>
    /// <param name = "position" > Position vector of the origin block of the chunk</param>
    /// <returns></returns>
    public static string BuildChunkName(Vector3 position)
    {
        return $"{position.x};{position.y};{position.z}";
    }
    /// <summary>
    /// Creates a chunk at the provided coordinates
    /// </summary>
    /// <param name = "x" > Specifies the position of the chunk origin on the X axis</param>
    /// <param name = "y" > Specifies the position of the chunk origin on the Y axis</param>
    /// <param name = "z" > Specifies the position of the chunk origin on the Z axis</param>

        void BuildChunkAt(int x, int y, int z)
        {
        // Creates a Chunk starting position vector with the provided x, y, z parameters and ChunkSize value
            Vector3 chunkPosition = new Vector3(x * ChunkSize, y * ChunkSize, z * ChunkSize);

        // Generates a unique name for the chunk using BuildChunkName() function
            string chunkName = BuildChunkName(chunkPosition);

        // Simply declares a chunk variable for future use

        Chunk chunk;

            // Checks if the chunk is already in the ChunkData dictionary
            // if not we proceed
            if (!ChunkData.TryGetValue(chunkName, out chunk))
        {
            // Creates a new chunk instance,
            // builds the chunk
            // and allocates it to already created chunk variable
            chunk = new Chunk(chunkPosition, TextureAtlas);

            // Makes this chunk a child of the world gameobject
            chunk.chunk.transform.parent = this.transform;

            // Names the chunk with the unique standardized name
            chunk.chunk.name = chunkName;

            // Adds the chunk to the 'ChunkData' dictionary and catalogs(names) it accordingly
            ChunkData.TryAdd(chunkName, chunk);
        }
    }

    IEnumerator BuildWorldRecursivly(int x, int y, int z, int radius)
    {
        lastBuildPosition = Player.transform.position;
        //Player.transform.rotation.eulerAngles.
        radius--;
        if (radius <= 0)
            yield break;

        // Front
        BuildChunkAt(x, y, z + 1);
        queue.Run(BuildWorldRecursivly(x, y, z + 1, radius));
        yield return null;

        // Back
        BuildChunkAt(x, y, z - 1);
        queue.Run(BuildWorldRecursivly(x, y, z - 1, radius));
        yield return null;

        // Left
        BuildChunkAt(x - 1, y, z);
        queue.Run(BuildWorldRecursivly(x - 1, y, z, radius));
        yield return null;

        // Right
        BuildChunkAt(x + 1, y, z);
        queue.Run(BuildWorldRecursivly(x + 1, y, z, radius));
        yield return null;
        // Up
        BuildChunkAt(x, y + 1, z);
        queue.Run(BuildWorldRecursivly(x, y + 1, z, radius));
        yield return null;
        // Down
        BuildChunkAt(x, y - 1, z);
        queue.Run(BuildWorldRecursivly(x, y - 1, z, radius));
        yield return null;
    }
    IEnumerator DrawChunks()
    {
        foreach (KeyValuePair<string, Chunk> chunk in ChunkData)
        {
            if (chunk.Value.Status == Chunk.ChunkStatus.Draw)
            {
                chunk.Value.DrawChunk();
                chunk.Value.Status = Chunk.ChunkStatus.Keep;
            }

            if (chunk.Value.chunk && Vector3.Distance(
                    chunk.Value.chunk.transform.position,
                    Player.transform.position) > Radius * ChunkSize)
            {
                chunkRemovaList.Add(chunk.Key);
                chunk.Value.Status = Chunk.ChunkStatus.Done;
            }
            yield return null;
        }
    }

    IEnumerator RemoveOldChunks()
    {
        foreach (var chunkName in chunkRemovaList)
        {
            Chunk chunk;
            if (ChunkData.TryGetValue(chunkName, out chunk))
            {
                if (ChunkData.TryRemove(chunkName, out chunk))
                {
                    Destroy(chunk.chunk);
                    chunkRemovaList.Remove(chunkName);
                }
            }
        }

        chunkRemovaList.Clear();
        yield return null;
    }
    void BuildNearPlayer()
    {
        StopCoroutine("BuildWorldRecursivly");
        queue.Run(BuildWorldRecursivly(
            (int)Player.transform.position.x / ChunkSize,
            (int)Player.transform.position.y / ChunkSize,
            (int)Player.transform.position.z / ChunkSize,
            Radius
        ));

    }
    void Start()
    {
        // Sets the player gameobject height one block above the world to be generated
        Vector3 playerPosition = Player.transform.position;
        Player.gameObject.transform.Translate(new Vector3(
            playerPosition.x,
            Utils.GenerateHeight(150, 80, 2, 0.02f, 0.3f, playerPosition.x, playerPosition.z) + 5,
            playerPosition.z));

        // Sets player gameobject to false which prevents it from falling
        lastBuildPosition = Player.transform.position;
        Player.SetActive(false);

        // Setes FirstBuild variable to true since the world building just started
        FirstBuild = true;

        // Instantiates the dictionary to store chunks
        ChunkData = new ConcurrentDictionary<string, Chunk>();
        this.gameObject.transform.position = Vector3.zero;
        this.gameObject.transform.rotation = Quaternion.identity;
        queue = new CoroutineQueue(maxCorutines, StartCoroutine);

        BuildChunkAt((int)Player.transform.position.x / ChunkSize, (int)Player.transform.position.y / ChunkSize, (int)Player.transform.position.z / ChunkSize);


        //queue.Run(DrawChunks());
        //DrawChunks();
        Player.SetActive(true);
        //queue.Run(BuildWorldRecursivly(
        //    (int)Player.transform.position.x / ChunkSize,
        //    (int)Player.transform.position.y / ChunkSize,
        //    (int)Player.transform.position.z / ChunkSize,
        //    Radius
        //));
        BuildNearPlayer();
        queue.Run(DrawChunks());
    }

    void Update()
    {

        //Vector3 playerMovement = lastBuildPosition - Player.transform.position;
        //if (playerMovement.magnitude > ChunkSize)
        //{
        //    lastBuildPosition = Player.transform.position;
        //    BuildNearPlayer();

        //}
        if (!Player.activeInHierarchy)
        {
            Player.SetActive(true);
            FirstBuild = false;
        }
        //queue.Run(RemoveOldChunks());
        //queue.Run(DrawChunks());

    }


}
