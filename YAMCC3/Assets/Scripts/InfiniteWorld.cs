using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;


public class InfiniteWorld : MonoBehaviour
{

    int3 chunkSize = CONST.chunkSize;

    public static int3 playerChunkPosOld;

    static int3 worldChunkSize = CONST.worldChunkCount;

    //public static bool enabled = false;

    public static int3 ChunkWorldPositionOffset = new int3(0, 0, 0);

    int3 arraySize = new int3(
        CONST.worldChunkCount.x - 1,
        CONST.worldChunkCount.y - 1,
        CONST.worldChunkCount.z - 1
        );


    GameObject player;
    Vector3 middle;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        middle = new Vector3((CONST.worldChunkCount.x * CONST.chunkSize.x) / 2 + (CONST.chunkSize.x) / 2, player.transform.position.y, (CONST.worldChunkCount.z * CONST.chunkSize.z) / 2 + (CONST.chunkSize.z) / 2);

        player.transform.position = middle;

        playerChunkPosOld = getPlayerChunkPos();
    }

    void Update()
    {
        if(!UpdateLoop.initLoadDone)
        {
            player.transform.position = middle;
        }
        int3 playerChunkPos = getPlayerChunkPos();
        if (playerChunkPosOld != playerChunkPos)
        {
            Debug.Log("Chunk pos changed: " + playerChunkPos.x + " " + playerChunkPos.y + " " + playerChunkPos.z);
            int3 change = new int3(playerChunkPos.x - playerChunkPosOld.x,
                       playerChunkPos.y - playerChunkPosOld.y,
                       playerChunkPos.z - playerChunkPosOld.z);
            playerChunkPosOld = playerChunkPos;

            Debug.Log("Change: " + change.x + " " + change.y + " " + change.z);
            onPlayerChunkChanged(change.x, change.y, change.z);
        }
    }

    public void onPlayerChunkChanged(int dirX, int dirY, int dirZ)
    {
        //int dirX = (int)(playerPosNew.x - playerPosLast.x);
        //int dirY = (int)(playerPosNew.y - playerPosLast.y);
        //int dirZ = (int)(playerPosNew.z - playerPosLast.z);
        Debug.Log(dirX + " " + dirY + " " + dirZ);
        //These made everything bug the shit out of everything else.
        //ChunkWorldPositionOffset.x += dirX;
        //ChunkWorldPositionOffset.y += dirY;
        //ChunkWorldPositionOffset.z += dirZ;

        AddToDirQueue(new int3(dirX, dirY, dirZ));

        ThreadPool.QueueUserWorkItem(new WaitCallback(shiftOrganizer));
        //shiftOrganizer();
    }

    private int3 getPlayerChunkPos()
    {
        Vector3 playerPos = player.transform.position;

        int x = Mathf.CeilToInt(playerPos.x) - 1;
        int y = Mathf.CeilToInt(playerPos.y) - 1;
        int z = Mathf.CeilToInt(playerPos.z) - 1;

        x = x / CONST.chunkSize.x;
        y = y / CONST.chunkSize.y;
        z = z / CONST.chunkSize.z;

        //int3 offset = ChunkRowUtils.ChunkWorldPositionOffset;

        return new int3(x, y, z);
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

    
    private void shiftOrganizer(System.Object stateInfo)
    {

        ThreadPool.SetMinThreads(4, 4);
        ThreadPool.SetMaxThreads(4, 4);

        //Profiler.BeginSample("AAAAAAAAAAAAARRRRRGGGGHHHHH");
        // do something that takes a lot of time
        int3 dir = GetFromDirQueue();
        
        //lock (WorldInitializer.chunkArray)
        //{
        List<Chunk> chunksToBeDrawn = new List<Chunk>();
        chunksToBeDrawn.AddRange(xdirFunction(dir.x, ChunkWorldPositionOffset));
        chunksToBeDrawn.AddRange(ydirFunction(dir.y, ChunkWorldPositionOffset));
        chunksToBeDrawn.AddRange(zdirFunction(dir.z, ChunkWorldPositionOffset));
        for(int i = 0; i < chunksToBeDrawn.Count; i++)
        {
            //ThreadPool.QueueUserWorkItem(new WaitCallback(chunksToBeDrawn[i].AddChunkDrawdataToMeshQueue));

            chunksToBeDrawn[i].AddChunkDrawdataToMeshQueue();
        }
        //}
        //Profiler.EndSample();
    }
    /*
    private void shiftOrganizer()
    {


        ThreadPool.SetMinThreads(4, 4);
        ThreadPool.SetMaxThreads(4, 4);

        //Profiler.BeginSample("AAAAAAAAAAAAARRRRRGGGGHHHHH");
        // do something that takes a lot of time
        int3 dir = GetFromDirQueue();
        
        //lock (WorldInitializer.chunkArray)
        //{
        List<Chunk> chunksToBeDrawn = new List<Chunk>();
        chunksToBeDrawn.AddRange(xdirFunction(dir.x, ChunkWorldPositionOffset, WorldInitializer.chunkArray));
        chunksToBeDrawn.AddRange(ydirFunction(dir.y, ChunkWorldPositionOffset, WorldInitializer.chunkArray));
        chunksToBeDrawn.AddRange(zdirFunction(dir.z, ChunkWorldPositionOffset, WorldInitializer.chunkArray));
        for(int i = 0; i < chunksToBeDrawn.Count; i++)
        {
            chunksToBeDrawn[i].AddChunkDrawdataToMeshQueue();
        }
        //}
        //Profiler.EndSample();

    }
    */
    private static List<Chunk> xdirFunction(int xdir, int3 offset)
    {
        List<Chunk> chunksToBeDrawn = new List<Chunk>();
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
                                MeshGeneration.AddToRemoveQueue(WorldInitializer.getChunk(x, y, z));
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x + 1, y, z].AddChunkDrawdataToMeshQueue));

                            }
                            else if (x == worldChunkSize.x - 1)
                            {
                                int3 pos = new int3(x + ChunkWorldPositionOffset.x, y + ChunkWorldPositionOffset.y, z + ChunkWorldPositionOffset.z);

                                WorldInitializer.setChunk(x, y, z, new Chunk(pos, offset));
                                if (!WorldInitializer.getChunk(x, y, z).anyNonAir) continue;
                                chunksToBeDrawn.Add(WorldInitializer.getChunk(x, y, z));
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z].AddChunkDrawdataToMeshQueue));
                                //WorldInitializer.chunkArray[x, y, z].AddChunkDrawdataToMeshQueue();
                                //////ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x - 1, y, z].AddChunkDrawdataToMeshQueue));
                            }
                            else
                            {
                                WorldInitializer.setChunk(x, y, z, WorldInitializer.getChunk(x + 1, y, z));
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
                                //MeshGeneration.AddToRemoveQueue(WorldInitializer.chunkArray[x, y, z]);
                                MeshGeneration.AddToRemoveQueue(WorldInitializer.getChunk(x, y, z));


                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x - 1, y, z].AddChunkDrawdataToMeshQueue));
                            }
                            else if (x == 0)
                            {
                                int3 pos = new int3(x + ChunkWorldPositionOffset.x, y + ChunkWorldPositionOffset.y, z + ChunkWorldPositionOffset.z);
                                WorldInitializer.setChunk(x, y, z, new Chunk(pos, offset));
                                if (!WorldInitializer.getChunk(x, y, z).anyNonAir) continue;
                                chunksToBeDrawn.Add(WorldInitializer.getChunk(x, y, z));
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z].AddChunkDrawdataToMeshQueue));
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x + 1, y, z].AddChunkDrawdataToMeshQueue));

                            }
                            else
                            {
                                //chunks[x, y, z] = chunks[x - 1, y, z];
                                WorldInitializer.setChunk(x, y, z, WorldInitializer.getChunk(x - 1, y, z));

                            }
                        }
                    }
                }
            }
        }
        return chunksToBeDrawn;
    }

    private static List<Chunk> ydirFunction(int ydir, int3 offset)
    {
        List<Chunk> chunksToBeDrawn = new List<Chunk>();
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
                                //MeshGeneration.AddToRemoveQueue(WorldInitializer.chunkArray[x, y, z]);
                                MeshGeneration.AddToRemoveQueue(WorldInitializer.getChunk(x, y, z));


                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y + 1, z].AddChunkDrawdataToMeshQueue));

                            }
                            else if (y == worldChunkSize.y - 1)
                            {
                                int3 pos = new int3(x + ChunkWorldPositionOffset.x, y + ChunkWorldPositionOffset.y, z + ChunkWorldPositionOffset.z);

                                WorldInitializer.setChunk(x, y, z, new Chunk(pos, offset));
                                if (!WorldInitializer.getChunk(x, y, z).anyNonAir) continue;
                                chunksToBeDrawn.Add(WorldInitializer.getChunk(x, y, z));
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z].AddChunkDrawdataToMeshQueue));
                                //WorldInitializer.chunkArray[x, y, z].AddChunkDrawdataToMeshQueue();

                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y - 1, z].AddChunkDrawdataToMeshQueue));

                            }
                            else
                            {
                                //chunks[x, y, z] = chunks[x, y + 1, z];
                                WorldInitializer.setChunk(x, y, z, WorldInitializer.getChunk(x, y + 1, z));
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
                                //MeshGeneration.AddToRemoveQueue(WorldInitializer.chunkArray[x, y, z]);
                                MeshGeneration.AddToRemoveQueue(WorldInitializer.getChunk(x, y, z));

                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y - 1, z].AddChunkDrawdataToMeshQueue));

                            }
                            else if (y == 0)
                            {
                                int3 pos = new int3(x + ChunkWorldPositionOffset.x, y + ChunkWorldPositionOffset.y, z + ChunkWorldPositionOffset.z);
                                WorldInitializer.setChunk(x, y, z, new Chunk(pos, offset));
                                if (!WorldInitializer.getChunk(x, y, z).anyNonAir) continue;
                                chunksToBeDrawn.Add(WorldInitializer.getChunk(x, y, z));
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z].AddChunkDrawdataToMeshQueue));
                                //WorldInitializer.chunkArray[x, y, z].AddChunkDrawdataToMeshQueue();


                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y + 1, z].AddChunkDrawdataToMeshQueue));
                            }
                            else
                            {
                                //chunks[x, y, z] = chunks[x, y - 1, z];
                                WorldInitializer.setChunk(x, y, z, WorldInitializer.getChunk(x, y - 1, z));
                            }
                        }
                    }
                }
            }
        }
        return chunksToBeDrawn;
    }

    private static List<Chunk> zdirFunction(int zdir, int3 offset)
    {
        List<Chunk> chunksToBeDrawn = new List<Chunk>();
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
                                //MeshGeneration.AddToRemoveQueue(WorldInitializer.chunkArray[x, y, z]);
                                MeshGeneration.AddToRemoveQueue(WorldInitializer.getChunk(x, y, z));

                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z + 1].AddChunkDrawdataToMeshQueue));

                            }
                            else if (z == worldChunkSize.z - 1)
                            {
                                int3 pos = new int3(x + ChunkWorldPositionOffset.x, y + ChunkWorldPositionOffset.y, z + ChunkWorldPositionOffset.z);

                                WorldInitializer.setChunk(x, y, z, new Chunk(pos, offset));
                                if (!WorldInitializer.getChunk(x, y, z).anyNonAir) continue;
                                chunksToBeDrawn.Add(WorldInitializer.getChunk(x, y, z));

                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z].AddChunkDrawdataToMeshQueue));

                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z - 1].AddChunkDrawdataToMeshQueue));
                            }
                            else
                            {
                                //chunks[x, y, z] = chunks[x, y, z + 1];
                                WorldInitializer.setChunk(x, y, z, WorldInitializer.getChunk(x, y, z + 1));
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
                                //MeshGeneration.AddToRemoveQueue(WorldInitializer.chunkArray[x, y, z]);
                                MeshGeneration.AddToRemoveQueue(WorldInitializer.getChunk(x, y, z));


                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z - 1].AddChunkDrawdataToMeshQueue));

                            }
                            else if (z == 0)
                            {
                                int3 pos = new int3(x + ChunkWorldPositionOffset.x, y + ChunkWorldPositionOffset.y, z + ChunkWorldPositionOffset.z);
                                WorldInitializer.setChunk(x, y, z, new Chunk(pos, offset));
                                if (!WorldInitializer.getChunk(x, y, z).anyNonAir) continue;
                                chunksToBeDrawn.Add(WorldInitializer.getChunk(x, y, z));
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z].AddChunkDrawdataToMeshQueue));
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WorldInitializer.chunkArray[x, y, z + 1].AddChunkDrawdataToMeshQueue));

                            }
                            else
                            {
                                //chunks[x, y, z] = chunks[x, y, z - 1];
                                WorldInitializer.setChunk(x, y, z, WorldInitializer.getChunk(x, y, z -1));
                            }
                        }
                    }
                }
            }
        }
        return chunksToBeDrawn;
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
}