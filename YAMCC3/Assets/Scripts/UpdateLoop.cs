using UnityEngine;
using System.Collections;

public class UpdateLoop : MonoBehaviour
{
    public static Vector3 playerBlockPosWorld;


    Vector3 playerStartPos;
    public static bool initLoadDone = false;

    void Start()
    {
        playerStartPos  = new Vector3(
            (CONST.chunkSize.x * CONST.worldChunkCount.x) / 2,
            (CONST.chunkSize.y * CONST.worldChunkCount.y) / 2 + 1,
            (CONST.chunkSize.z * CONST.worldChunkCount.z) / 2);


    }
    /*
    void OnDrawGizmos()
    {
        Gizmos.DrawCube(new Vector3((playerChunkPosOld.x * CONST.chunkSize.x + CONST.chunkSize.x / 2),
                                    (playerChunkPosOld.y * CONST.chunkSize.y + CONST.chunkSize.y / 2),
                                    (playerChunkPosOld.z * CONST.chunkSize.z + CONST.chunkSize.z / 2)),
               new Vector3(CONST.chunkSize.x,
                           CONST.chunkSize.y,
                           CONST.chunkSize.z));
    }
    */
    void Update()
    {
        checkForMeshes();
    }

    static int counter = 0;
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
            initLoadDone = true;
        }
         
        if (initLoadDone)
        {
            if(counter < CONST.framesBetweenMeshes)
            {
                counter++;
                return;
            }
            else
            {
                counter = 0;
            }
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