using UnityEngine;
using System.Collections;
using System;

public class UpdateLoop : MonoBehaviour
{
    public static Vector3 playerBlockPosWorld;
    public static bool initLoadDone = false;
    DateTime startTime;

    void Start()
    {
        startTime = DateTime.Now;
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
            //DateTime end = DateTime.Now;
            //TimeSpan generationTime = end - startTime;
            //Debug.Log(generationTime);
            /*
            if(counter < CONST.framesBetweenMeshes)
            {
                counter++;
                return;
            }
            else
            {
                counter = 0;
            }
            */

            for (int i = 0; i < meshesPerFrame; i++)
            {
                ChunkDrawDataArray chunkdrawdata = MeshGenerationQueue.GetFromQueue();
                if (chunkdrawdata != null)
                {
                    chunkdrawdata.chunk.updateMesh(chunkdrawdata.vertexList,
                    chunkdrawdata.UVList,
                    chunkdrawdata.triangleList);
                }
                else
                {
                    break;
                }
            }
            //else
            //{
                checkForDestroys();
            //}
        }
    }
    //static int meshesPerFrame = CONST.worldChunkCount.x / 3;
    static int meshesPerFrame = 2;
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
        /*
        for (Chunk chunk = MeshGeneration.GetFromRemoveQueue(); chunk != null; chunk = MeshGeneration.GetFromRemoveQueue())
        {

            if (chunk != null)
            {
                Destroy(chunk.chunk);
            }
        }
        */
        
        Chunk chunk = MeshGeneration.GetFromRemoveQueue();
        if (chunk != null)
        {
            Destroy(chunk.chunk);
        }
        
        
    }



}