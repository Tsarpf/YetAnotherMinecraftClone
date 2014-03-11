using UnityEngine;
using System.Collections;

public class UpdateLoop : MonoBehaviour
{
    public static Vector3 playerBlockPosWorld;
    public static Vector3 playerChunkPos;

    Vector3 playerStartPos;
    public GameObject player;
    bool initLoadDone = false;

    void Start()
    {
        playerStartPos  = new Vector3(
            (CONST.chunkSize.x * CONST.worldChunkCount.x) / 2,
            (CONST.chunkSize.y * CONST.worldChunkCount.y) / 2 + 1,
            (CONST.chunkSize.z * CONST.worldChunkCount.z) / 2);
    }

    void Update()
    {
        checkForMeshes();
        playerBlockPosWorld = getPlayerBlockPosWorld();
        playerChunkPos = getPlayerChunkPos();

        if (Input.GetButtonDown("Jump"))
        {
            initLoadDone = true;
        }

        if (!initLoadDone)
        {
            player.transform.position = playerStartPos;
        }
        if (initLoadDone)
        {
            InfiniteWorld.enabled = true;
        }
    }
    private Vector3 getPlayerChunkPos()
    {
        Vector3 playerPos = player.transform.position;

        int x = Mathf.CeilToInt(playerPos.x) - 1;
        int y = Mathf.CeilToInt(playerPos.y) - 1;
        int z = Mathf.CeilToInt(playerPos.z) - 1;

        x = x / CONST.chunkSize.x;
        y = y / CONST.chunkSize.y;
        z = z / CONST.chunkSize.z;

        //int3 offset = ChunkRowUtils.ChunkWorldPositionOffset;

        return new Vector3(x, y, z);
    }

    private Vector3 getPlayerBlockPosWorld()
    {
        int x = Mathf.CeilToInt(player.transform.position.x) - 1;
        int y = Mathf.CeilToInt(player.transform.position.y) - 1;
        int z = Mathf.CeilToInt(player.transform.position.z) - 1;

        Vector3 blockCoords = new Vector3();
        blockCoords.x = x;
        blockCoords.y = y;
        blockCoords.z = z;

        return blockCoords;
    }
    private void checkForMeshes()
    {
        
        if (!initLoadDone)
        {
            for (ChunkDrawDataArray chunkdrawdata = MeshGenerationQueue.GetFromQueue(); chunkdrawdata != null; chunkdrawdata = MeshGenerationQueue.GetFromQueue())
            {
                int3 offset = InfiniteWorld.ChunkWorldPositionOffset;
                //int3 key = new int3(chunkdrawdata.chunkPos.x - offset.x, chunkdrawdata.chunkPos.y - offset.y, chunkdrawdata.chunkPos.z - offset.z);
                chunkdrawdata.chunk.updateMesh(chunkdrawdata.vertexList, chunkdrawdata.UVList, chunkdrawdata.triangleList);
            }
            checkForDestroys();
        }
         
        if (initLoadDone)
        {
            ChunkDrawDataArray chunkdrawdata = MeshGenerationQueue.GetFromQueue();
            if (chunkdrawdata != null)
            {
                /*
                int3 offset = ScrollWorldAroundObject.ChunkWorldPositionOffset;
                //int3 key = new int3(chunkdrawdata.chunkPos.x, chunkdrawdata.chunkPos.y, chunkdrawdata.chunkPos.z);
                int3 key = new int3(chunkdrawdata.chunkPos.x - offset.x, chunkdrawdata.chunkPos.y - offset.y, chunkdrawdata.chunkPos.z - offset.z);

                if (key.x < 0) key.x = 0;
                if (key.y < 0) key.y = 0;
                if (key.z < 0) key.z = 0;

                if (key.x > CONST.worldChunkCount.x - 1) { key.x = CONST.worldChunkCount.x - 1; }
                if (key.y > CONST.worldChunkCount.y - 1) { key.y = CONST.worldChunkCount.y - 1; }
                if (key.z > CONST.worldChunkCount.z - 1) { key.z = CONST.worldChunkCount.z - 1; }
                */
                //lock (WorldGameObject.chunks)
                //{
                //Debug.Log(key.x + " " + key.y + " " + key.z);
                chunkdrawdata.chunk.updateMesh(chunkdrawdata.vertexList,
                chunkdrawdata.UVList,
                chunkdrawdata.triangleList);
                //}
            }
            else
            {
                checkForDestroys();
            }
        }
    }

    private static void checkForDestroys()
    {
        /*
        int i = -1;
        for (Chunk chunk = InfiniteWorld.GetFromRemoveQueue(); chunk != null; chunk = InfiniteWorld.GetFromRemoveQueue())
        {
            Destroy(chunk.chunk);
            i++;
            if (i >= 1) { break; }
        }
        */
        
        for (Chunk chunk = MeshGeneration.GetFromRemoveQueue(); chunk != null; chunk = MeshGeneration.GetFromRemoveQueue())
        {

            if (chunk != null)
            {
                Destroy(chunk.chunk);
            }
        }
        
        /*
        Chunk chunk = MeshGeneration.GetFromRemoveQueue();
        if (chunk != null)
        {
            Destroy(chunk.chunk);
        }
        */
        
    }



}