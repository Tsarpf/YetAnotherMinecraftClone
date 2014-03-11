using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;


public class InfiniteWorld : MonoBehaviour
{
    int3 chunkSize = CONST.chunkSize;

    public Vector3 playerPosLast;
    public Vector3 playerPosNew;

    static int3 worldChunkSize = CONST.worldChunkCount;

    public static bool enabled = false;

    public static int3 ChunkWorldPositionOffset = new int3(0, 0, 0);

    int3 arraySize = new int3(
        CONST.worldChunkCount.x - 1,
        CONST.worldChunkCount.y - 1,
        CONST.worldChunkCount.z - 1
        );

    void Start()
    {
        playerPosNew = UpdateLoop.playerChunkPos;
        playerPosLast = playerPosNew;
    }

    void Update()
    {

            playerPosNew = UpdateLoop.playerChunkPos;
            //if (enabled)
            //{
                if (playerPosNew != playerPosLast)
                {

                    int dirX = (int)(playerPosNew.x - playerPosLast.x);
                    int dirY = (int)(playerPosNew.y - playerPosLast.y);
                    int dirZ = (int)(playerPosNew.z - playerPosLast.z);
                    Debug.Log(dirX + " " + dirY + " " + dirZ);
                    //These made everything bug the shit out of everything else.
                    //ChunkWorldPositionOffset.x += dirX;
                    //ChunkWorldPositionOffset.y += dirY;
                    //ChunkWorldPositionOffset.z += dirZ;

                    AddToDirQueue(new int3(dirX, dirY, dirZ));

                    ThreadPool.QueueUserWorkItem(new WaitCallback(shiftOrganizer));
                    //shiftOrganizer();
                }

                playerPosLast = playerPosNew;
            //}

    }

    private void shiftOrganizer(System.Object stateInfo)
    {
        ThreadPool.SetMinThreads(4, 4);
        ThreadPool.SetMaxThreads(4, 4);
        int3 dir = GetFromDirQueue();
        lock (WorldInitializer.chunkArray)
        {
            xdirFunction(dir.x, ChunkWorldPositionOffset, WorldInitializer.chunkArray);
            ydirFunction(dir.y, ChunkWorldPositionOffset, WorldInitializer.chunkArray);
            zdirFunction(dir.z, ChunkWorldPositionOffset, WorldInitializer.chunkArray);
        }
        
    }

    private void shiftOrganizer()
    {


        ThreadPool.SetMinThreads(4, 4);
        ThreadPool.SetMaxThreads(4, 4);

        //Profiler.BeginSample("AAAAAAAAAAAAARRRRRGGGGHHHHH");
        // do something that takes a lot of time
        int3 dir = GetFromDirQueue();
        
        lock (WorldInitializer.chunkArray)
        {
            xdirFunction(dir.x, ChunkWorldPositionOffset, WorldInitializer.chunkArray);
            ydirFunction(dir.y, ChunkWorldPositionOffset, WorldInitializer.chunkArray);
            zdirFunction(dir.z, ChunkWorldPositionOffset, WorldInitializer.chunkArray);
        }
        //Profiler.EndSample();

    }


    private static void xdirFunction(int xdir, int3 offset, Chunk[, ,] chunks)
    {
        if (xdir >= 1)
        {

            for (int i = 0; i < xdir; i++)
            {
                ChunkWorldPositionOffset.x += 1;
                for (int x = 0; x < worldChunkSize.x; x++)
                {
                    for (int y = 0; y < worldChunkSize.y; y++)
                    {
                        for (int z = 0; z < worldChunkSize.z; z++)
                        {
                            if (x == 0)
                            {
                                MeshGeneration.AddToRemoveQueue(WorldInitializer.chunkArray[x, y, z]);
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x + 1, y, z].AddChunkDrawdataToMeshQueue));

                            }
                            if (x == worldChunkSize.x - 1)
                            {
                                int3 pos = new int3(x + ChunkWorldPositionOffset.x, y + ChunkWorldPositionOffset.y, z + ChunkWorldPositionOffset.z);

                                WorldInitializer.chunkArray[x, y, z] = new Chunk(pos, offset);
                                ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z].AddChunkDrawdataToMeshQueue));
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x - 1, y, z].AddChunkDrawdataToMeshQueue));
                            }
                            else
                            {
                                chunks[x, y, z] = chunks[x + 1, y, z];
                            }
                            /*
                            //newstuff after this

                            if (x == worldChunkSize.x - 1)
                            {

                            }
                             */
                        }
                    }
                }
            }
        }

        else if (xdir <= -1)
        {

            for (int i = 0; i > xdir; i--)
            {
                ChunkWorldPositionOffset.x -= 1;
                for (int x = worldChunkSize.x - 1; x >= 0; x--)
                {
                    for (int y = worldChunkSize.y - 1; y >= 0; y--)
                    {
                        for (int z = worldChunkSize.z - 1; z >= 0; z--)
                        {
                            if (x == (worldChunkSize.x - 1))
                            {
                                MeshGeneration.AddToRemoveQueue(WorldInitializer.chunkArray[x, y, z]);

                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x - 1, y, z].AddChunkDrawdataToMeshQueue));
                            }
                            if (x == 0)
                            {
                                int3 pos = new int3(x + ChunkWorldPositionOffset.x, y + ChunkWorldPositionOffset.y, z + ChunkWorldPositionOffset.z);
                                WorldInitializer.chunkArray[x, y, z] = new Chunk(pos, offset);
                                ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z].AddChunkDrawdataToMeshQueue));
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x + 1, y, z].AddChunkDrawdataToMeshQueue));

                            }
                            else
                            {
                                chunks[x, y, z] = chunks[x - 1, y, z];
                            }
                        }
                    }
                }
            }
        }

    }

    private static void ydirFunction(int ydir, int3 offset, Chunk[, ,] chunks)
    {

        if (ydir >= 1)
        {
            for (int i = 0; i < ydir; i++)
            {
                ChunkWorldPositionOffset.y += 1;
                for (int x = 0; x < worldChunkSize.x; x++)
                {
                    for (int y = 0; y < worldChunkSize.y; y++)
                    {
                        for (int z = 0; z < worldChunkSize.z; z++)
                        {
                            if (y == 0)
                            {
                                MeshGeneration.AddToRemoveQueue(WorldInitializer.chunkArray[x, y, z]);

                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y + 1, z].AddChunkDrawdataToMeshQueue));

                            }
                            if (y == worldChunkSize.y - 1)
                            {
                                int3 pos = new int3(x + ChunkWorldPositionOffset.x, y + ChunkWorldPositionOffset.y, z + ChunkWorldPositionOffset.z);

                                WorldInitializer.chunkArray[x, y, z] = new Chunk(pos, offset);
                                ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z].AddChunkDrawdataToMeshQueue));
                                //WorldInitializer.chunkArray[x, y, z].AddChunkDrawdataToMeshQueue();

                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y - 1, z].AddChunkDrawdataToMeshQueue));

                            }
                            else
                            {
                                chunks[x, y, z] = chunks[x, y + 1, z];
                            }
                        }
                    }
                }
            }
        }
        else if (ydir <= -1)
        {

            for (int i = 0; i > ydir; i--)
            {
                ChunkWorldPositionOffset.y -= 1;
                for (int x = worldChunkSize.x - 1; x >= 0; x--)
                {
                    for (int y = worldChunkSize.y - 1; y >= 0; y--)
                    {
                        for (int z = worldChunkSize.z - 1; z >= 0; z--)
                        {
                            if (y == (worldChunkSize.y - 1))
                            {
                                MeshGeneration.AddToRemoveQueue(WorldInitializer.chunkArray[x, y, z]);
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y - 1, z].AddChunkDrawdataToMeshQueue));

                            }
                            if (y == 0)
                            {
                                int3 pos = new int3(x + ChunkWorldPositionOffset.x, y + ChunkWorldPositionOffset.y, z + ChunkWorldPositionOffset.z);
                                WorldInitializer.chunkArray[x, y, z] = new Chunk(pos, offset);

                                ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z].AddChunkDrawdataToMeshQueue));
                                //WorldInitializer.chunkArray[x, y, z].AddChunkDrawdataToMeshQueue();


                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y + 1, z].AddChunkDrawdataToMeshQueue));
                            }
                            else
                            {
                                chunks[x, y, z] = chunks[x, y - 1, z];
                            }
                        }
                    }
                }
            }
        }
    }

    private static void zdirFunction(int zdir, int3 offset, Chunk[, ,] chunks)
    {
        if (zdir >= 1)
        {
            for (int i = 0; i < zdir; i++)
            {
                ChunkWorldPositionOffset.z += 1;
                for (int x = 0; x < worldChunkSize.x; x++)
                {
                    for (int y = 0; y < worldChunkSize.y; y++)
                    {
                        for (int z = 0; z < worldChunkSize.z; z++)
                        {
                            if (z == 0)
                            {
                                MeshGeneration.AddToRemoveQueue(WorldInitializer.chunkArray[x, y, z]);
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z + 1].AddChunkDrawdataToMeshQueue));

                            }
                            if (z == worldChunkSize.z - 1)
                            {
                                int3 pos = new int3(x + ChunkWorldPositionOffset.x, y + ChunkWorldPositionOffset.y, z + ChunkWorldPositionOffset.z);

                                WorldInitializer.chunkArray[x, y, z] = new Chunk(pos, offset);
                                ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z].AddChunkDrawdataToMeshQueue));

                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z - 1].AddChunkDrawdataToMeshQueue));
                            }
                            else
                            {
                                chunks[x, y, z] = chunks[x, y, z + 1];
                            }
                        }
                    }
                }
            }
        }
        else if (zdir <= -1)
        {

            for (int i = 0; i > zdir; i--)
            {
                ChunkWorldPositionOffset.z -= 1;
                for (int x = worldChunkSize.x - 1; x >= 0; x--)
                {
                    for (int y = worldChunkSize.y - 1; y >= 0; y--)
                    {
                        for (int z = worldChunkSize.z - 1; z >= 0; z--)
                        {
                            if (z == (worldChunkSize.z - 1))
                            {
                                MeshGeneration.AddToRemoveQueue(WorldInitializer.chunkArray[x, y, z]);

                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z - 1].AddChunkDrawdataToMeshQueue));

                            }
                            if (z == 0)
                            {
                                int3 pos = new int3(x + ChunkWorldPositionOffset.x, y + ChunkWorldPositionOffset.y, z + ChunkWorldPositionOffset.z);
                                WorldInitializer.chunkArray[x, y, z] = new Chunk(pos, offset);
                                ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z].AddChunkDrawdataToMeshQueue));
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z + 1].AddChunkDrawdataToMeshQueue));

                            }
                            else
                            {
                                chunks[x, y, z] = chunks[x, y, z - 1];
                            }
                        }
                    }
                }
            }
        }
    }



    public static Queue qShiftDirection = new Queue(CONST.worldChunkCount.x * CONST.worldChunkCount.z + 10);
    public static void AddToDirQueue(int3 dir)
    {
        //lock (qShiftDirection)
        //{
            qShiftDirection.Enqueue(dir);
        //}
    }

    public static int3 GetFromDirQueue()
    {
        int3 dir;

        //lock (qShiftDirection)
        //{
            if (qShiftDirection.Count > 0)
            {
                dir = (int3)qShiftDirection.Dequeue();
            }
            else
            {
                Debug.Log("GetFromDirQueue was null");
                dir = new int3(0, 0, 0);
            }
        //}
        return dir;
    }
    /*

     */
}