    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class ObjectCollisionScript : MonoBehaviour
{
    //For now the only object that needs to know it's surroundings is
    //the player. The player is <1blocks wide and 1<blocks<2 tall.
    //Thus it can be completely surrounded by 3*3*4 grid of blocks.
    static int3 arraySize = new int3(5, 5, 5);
    BlockType[,,] blockTypes = new BlockType[arraySize.x, arraySize.y, arraySize.z];
    Vector3[, ,] blockPositions = new Vector3[arraySize.x, arraySize.y, arraySize.z];
    //This prolly only works for the default size first person controller or smaller objects...
    GameObject[, ,] boxColliderGOs = new GameObject[5, 5, 5]; 

    int3 chunkSize = CONST.chunkSize;
    
    //Transform transform;
    Vector3 playerPos;

    //int3 playerChunkCoords;
    int3 playerBlockCoords;
    int3 loopStartPos = new int3(-1, -3, -1);

    //int3 offset = InfiniteWorld.ChunkWorldPositionOffset;

    Vector3 playerLastFrameBlock;

    void Start()
    {
        int x = Mathf.CeilToInt(transform.position.x) - 1;
        int y = Mathf.CeilToInt(transform.position.y) - 1;
        int z = Mathf.CeilToInt(transform.position.z) - 1;

        //Debug.Log(x);

        playerLastFrameBlock = new Vector3(x, y, z);

        for (int xa = 0; xa < arraySize.x; xa++)
        {
            for (int ya = 0; ya < arraySize.y; ya++)
            {
                for (int za = 0; za < arraySize.z; za++)
                {

                    blockPositions[xa, ya, za] = new Vector3(
                        xa + loopStartPos.x + x - 0.5f,
                        ya + loopStartPos.y + y + 0.5f,
                        za + loopStartPos.z + z - 0.5f);

                    int ccx = xa + loopStartPos.x + x;
                    int ccy = ya + loopStartPos.y + y;
                    int ccz = za + loopStartPos.z + z;

                    int3 chunkcoords = new int3(0, 0, 0);
                    int3 blockcoords = new int3(0, 0, 0);

                    int3 offset = InfiniteWorld.ChunkWorldPositionOffset;

                    chunkcoords.x = ccx / chunkSize.x - offset.x;
                    chunkcoords.y = ccy / chunkSize.y - offset.y;
                    chunkcoords.z = ccz / chunkSize.z - offset.z;

                    boxColliderGOs[xa, ya, za] = new GameObject();
                    boxColliderGOs[xa, ya, za].AddComponent<BoxCollider>();
                    BoxCollider boxCollider = boxColliderGOs[xa, ya, za].GetComponent<BoxCollider>();
                    boxCollider.enabled = false;
                    boxColliderGOs[xa, ya, za].transform.position = blockPositions[xa, ya, za];



                    if (WorldInitializer.chunkArrayContainsKey(chunkcoords))
                    {
                        blockcoords.x = ccx % chunkSize.x;
                        blockcoords.y = ccy % chunkSize.y;
                        blockcoords.z = ccz % chunkSize.z;

                        blockTypes[xa, ya, za] = WorldInitializer.chunkArray[chunkcoords.x, chunkcoords.y, chunkcoords.z].
                        blocks[blockcoords.x, blockcoords.y, blockcoords.z].blockType;

                        if (blockTypes[xa, ya, za] != BlockType.Air)
                        {
                            boxCollider.enabled = true;
                        }

                    }
                    else { blockTypes[xa, ya, za] = BlockType.Air; }
                }
            }
        }

    }

    void Update()
    {
        // Debug.Log(UpdateLoop.playerBlockPosWorld);

        if (UpdateLoop.playerBlockPosWorld != playerLastFrameBlock)
        {
            /*
            Vector3 dir = new Vector3
                (
                newPos.x - playerLastFrameBlock.x,
                newPos.y - playerLastFrameBlock.y,
                newPos.z - playerLastFrameBlock.z
                );
            */
            //Update the boxcolliders positions
            
            refreshamenttrytwo(new Vector3
                (
                UpdateLoop.playerBlockPosWorld.x - playerLastFrameBlock.x,
                UpdateLoop.playerBlockPosWorld.y - playerLastFrameBlock.y,
                UpdateLoop.playerBlockPosWorld.z - playerLastFrameBlock.z
                ));
            


            playerLastFrameBlock = UpdateLoop.playerBlockPosWorld;
        }
    }

    public void refreshamenttrytwo(Vector3 dir)
    {
        //Profiler.BeginSample("ObjectCollisionScriptUpdateAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        xdir(dir.x);
        ydir(dir.y);
        zdir(dir.z);
        //Profiler.EndSample();
    }

    public void refreshamenttrytwo()
    {
        //For when a refresh is required simply because a block was either removed or added.
        //Shouldn't be hard to implement, just loop through the blocktype array and add/remove colliders
        //if needed.

        //Though I like adding/removing blocks far away too so maybe should implement the alexxx's magic scriptey..
    }

    private void xdir(float dirx)
    {
        GameObject placeHolder;
        if (dirx >= 1)
        {
            for (int iter = 0; iter < dirx; iter++)
            {
                for (int y = 0; y < arraySize.y; y++)
                {
                    for (int z = 0; z < arraySize.z; z++)
                    {

                        placeHolder = boxColliderGOs[0, y, z];
                        for (int x = 0; x < arraySize.x - 1; x++)
                        {
                            boxColliderGOs[x, y, z] = boxColliderGOs[x + 1, y, z];
                        }

                        placeHolder.transform.position = new Vector3(
                            placeHolder.transform.position.x + arraySize.x,
                            placeHolder.transform.position.y,
                            placeHolder.transform.position.z);




                        int xa = Mathf.CeilToInt(placeHolder.transform.position.x) - 1;
                        int ya = Mathf.CeilToInt(placeHolder.transform.position.y) - 1;
                        int za = Mathf.CeilToInt(placeHolder.transform.position.z) - 1;


                        int3 chunkcoords = new int3(0, 0, 0);
                        int3 blockcoords = new int3(0, 0, 0);

                        int3 offset = InfiniteWorld.ChunkWorldPositionOffset;

                        /*
                        chunkcoords.x = Mathf.CeilToInt(((float)xa / (float)chunkSize.x - (float)offset.x)) - 1;
                        chunkcoords.y = Mathf.CeilToInt(((float)ya / (float)chunkSize.y - (float)offset.y)) - 1;
                        chunkcoords.z = Mathf.CeilToInt(((float)za / (float)chunkSize.z - (float)offset.z)) - 1;
                        */
                        chunkcoords.x = xa / chunkSize.x - offset.x;
                        chunkcoords.y = ya / chunkSize.y - offset.y;
                        chunkcoords.z = za / chunkSize.z - offset.z;
                        
                        //Debug.Log(chunkcoords.x + " " + chunkcoords.y + " " + chunkcoords.z);
                        //Debug.Log(chunkcoordsudesh.x + " " + chunkcoordsudesh.y + " " + chunkcoordsudesh.z);

                        if (WorldInitializer.chunkArrayContainsKey(chunkcoords))
                        {
                            blockcoords.x = xa % chunkSize.x;
                            blockcoords.y = ya % chunkSize.y;
                            blockcoords.z = za % chunkSize.z;

                            if (blockcoords.x < 0)
                            {
                                blockcoords.x = chunkSize.x - Math.Abs(blockcoords.x);
                                chunkcoords.x -= 1;
                            }

                            if (blockcoords.y < 0)
                            {
                                blockcoords.y = chunkSize.y - Math.Abs(blockcoords.y);
                                chunkcoords.y-= 1;
                            }

                            if (blockcoords.z < 0)
                            {
                                blockcoords.z = chunkSize.z - Math.Abs(blockcoords.z);
                                chunkcoords.z -= 1;
                            }

                            if (chunkcoords.x < 0 || chunkcoords.y < 0 || chunkcoords.z < 0)
                            {
                                placeHolder.GetComponent<BoxCollider>().enabled = false;
                            }
                            else
                            {
                                blockTypes[arraySize.x - 1, y, z] = WorldInitializer.chunkArray[chunkcoords.x, chunkcoords.y, chunkcoords.z].
                                    blocks[blockcoords.x, blockcoords.y, blockcoords.z].blockType;

                                if (blockTypes[arraySize.x - 1, y, z] != BlockType.Air)
                                {
                                    placeHolder.GetComponent<BoxCollider>().enabled = true;
                                }
                                else
                                {
                                    placeHolder.GetComponent<BoxCollider>().enabled = false;
                                }
                            }
                        }
                        boxColliderGOs[arraySize.x - 1, y, z] = placeHolder;
                    }
                }
            }
        }


        if (dirx <= -1)
        {
            for (int iter = 0; iter > dirx; iter--)
            {
                for (int y = 0; y < arraySize.y; y++)
                {
                    for (int z = 0; z < arraySize.z; z++)
                    {

                        placeHolder = boxColliderGOs[arraySize.x - 1, y, z];
                        for (int x = arraySize.x - 1; x > 0; x--)
                        {
                            boxColliderGOs[x, y, z] = boxColliderGOs[x - 1, y, z];
                        }

                        placeHolder.transform.position = new Vector3(
                            placeHolder.transform.position.x - arraySize.x,
                            placeHolder.transform.position.y,
                            placeHolder.transform.position.z);




                        int xa = Mathf.CeilToInt(placeHolder.transform.position.x) - 1;
                        int ya = Mathf.CeilToInt(placeHolder.transform.position.y) - 1;
                        int za = Mathf.CeilToInt(placeHolder.transform.position.z) - 1;


                        int3 chunkcoords = new int3(0, 0, 0);
                        int3 blockcoords = new int3(0, 0, 0);

                        int3 offset = InfiniteWorld.ChunkWorldPositionOffset;

                        /*
                        chunkcoords.x = Mathf.CeilToInt(((float)xa / (float)chunkSize.x - (float)offset.x)) - 1;
                        chunkcoords.y = Mathf.CeilToInt(((float)ya / (float)chunkSize.y - (float)offset.y)) - 1;
                        chunkcoords.z = Mathf.CeilToInt(((float)za / (float)chunkSize.z - (float)offset.z)) - 1;
                        */
                        chunkcoords.x = xa / chunkSize.x - offset.x;
                        chunkcoords.y = ya / chunkSize.y - offset.y;
                        chunkcoords.z = za / chunkSize.z - offset.z;

                        if (WorldInitializer.chunkArrayContainsKey(chunkcoords))
                        {
                            blockcoords.x = xa % chunkSize.x;
                            blockcoords.y = ya % chunkSize.y;
                            blockcoords.z = za % chunkSize.z;

                            if (blockcoords.x < 0)
                            {
                                blockcoords.x = chunkSize.x - Math.Abs(blockcoords.x);
                                chunkcoords.x -= 1;
                            }

                            if (blockcoords.y < 0)
                            {
                                blockcoords.y = chunkSize.y - Math.Abs(blockcoords.y);
                                chunkcoords.y-= 1;
                            }

                            if (blockcoords.z < 0)
                            {
                                blockcoords.z = chunkSize.z - Math.Abs(blockcoords.z);
                                chunkcoords.z -= 1;
                            }
                            if (chunkcoords.x < 0 || chunkcoords.y < 0 || chunkcoords.z < 0)
                            {
                                placeHolder.GetComponent<BoxCollider>().enabled = false;
                            }
                            else
                            {
                                blockTypes[0, y, z] = WorldInitializer.chunkArray[chunkcoords.x, chunkcoords.y, chunkcoords.z].
                                    blocks[blockcoords.x, blockcoords.y, blockcoords.z].blockType;


                                if (blockTypes[0, y, z] != BlockType.Air)
                                {
                                    placeHolder.GetComponent<BoxCollider>().enabled = true;
                                }
                                else
                                {
                                    placeHolder.GetComponent<BoxCollider>().enabled = false;
                                }
                            }
                        }
                        boxColliderGOs[0, y, z] = placeHolder;
                    }
                }
            }
        }
    }
    private void ydir(float diry)
    {
        GameObject placeHolder;
        if (diry >= 1)
        {
            for (int iter = 0; iter < diry; iter++)
            {
                for (int x = 0; x < arraySize.x; x++)
                {
                    for (int z = 0; z < arraySize.z; z++)
                    {

                        placeHolder = boxColliderGOs[x, 0, z];
                        for (int y = 0; y < arraySize.y - 1; y++)
                        {
                            boxColliderGOs[x, y, z] = boxColliderGOs[x, y + 1, z];
                        }

                        placeHolder.transform.position = new Vector3(
                            placeHolder.transform.position.x,
                            placeHolder.transform.position.y + arraySize.y,
                            placeHolder.transform.position.z);

                        int xa = Mathf.CeilToInt(placeHolder.transform.position.x) - 1;
                        int ya = Mathf.CeilToInt(placeHolder.transform.position.y) - 1;
                        int za = Mathf.CeilToInt(placeHolder.transform.position.z) - 1;


                        int3 chunkcoords = new int3(0, 0, 0);
                        int3 blockcoords = new int3(0, 0, 0);

                        int3 offset = InfiniteWorld.ChunkWorldPositionOffset;

                        /*
                        chunkcoords.x = Mathf.CeilToInt(((float)xa / (float)chunkSize.x - (float)offset.x)) - 1;
                        chunkcoords.y = Mathf.CeilToInt(((float)ya / (float)chunkSize.y - (float)offset.y)) - 1;
                        chunkcoords.z = Mathf.CeilToInt(((float)za / (float)chunkSize.z - (float)offset.z)) - 1;
                        */

                        chunkcoords.x = xa / chunkSize.x - offset.x;
                        chunkcoords.y = ya / chunkSize.y - offset.y;
                        chunkcoords.z = za / chunkSize.z - offset.z;
                        

                        if (WorldInitializer.chunkArrayContainsKey(chunkcoords))
                        {
                            blockcoords.x = xa % chunkSize.x;
                            blockcoords.y = ya % chunkSize.y;
                            blockcoords.z = za % chunkSize.z;

                            if (blockcoords.x < 0)
                            {
                                blockcoords.x = chunkSize.x - Math.Abs(blockcoords.x);
                                chunkcoords.x -= 1;
                            }

                            if (blockcoords.y < 0)
                            {
                                blockcoords.y = chunkSize.y - Math.Abs(blockcoords.y);
                                chunkcoords.y-= 1;
                            }

                            if (blockcoords.z < 0)
                            {
                                blockcoords.z = chunkSize.z - Math.Abs(blockcoords.z);
                                chunkcoords.z -= 1;
                            }

                            if (chunkcoords.x < 0 || chunkcoords.y < 0 || chunkcoords.z < 0)
                            {
                                placeHolder.GetComponent<BoxCollider>().enabled = false;
                            }
                            else
                            {
                                blockTypes[x, arraySize.y - 1, z] = WorldInitializer.chunkArray[chunkcoords.x, chunkcoords.y, chunkcoords.z].
                                    blocks[blockcoords.x, blockcoords.y, blockcoords.z].blockType;

                                if (blockTypes[x, arraySize.y - 1, z] != BlockType.Air)
                                {
                                    placeHolder.GetComponent<BoxCollider>().enabled = true;
                                }
                                else
                                {
                                    placeHolder.GetComponent<BoxCollider>().enabled = false;
                                }
                            }
                        }

                        boxColliderGOs[x, arraySize.y - 1, z] = placeHolder;
                    }
                }
            }
        }

        if (diry <= -1)
        {
            for (int iter = 0; iter > diry; iter--)
            {
                for (int x = 0; x < arraySize.x; x++)
                {
                    for (int z = 0; z < arraySize.z; z++)
                    {

                        placeHolder = boxColliderGOs[x, arraySize.y - 1, z];
                        for (int y = arraySize.y - 1; y > 0; y--)
                        {
                            boxColliderGOs[x, y, z] = boxColliderGOs[x, y - 1, z];
                        }

                        placeHolder.transform.position = new Vector3(
                            placeHolder.transform.position.x,
                            placeHolder.transform.position.y - arraySize.y,
                            placeHolder.transform.position.z);



                        int xa = Mathf.CeilToInt(placeHolder.transform.position.x) - 1;
                        int ya = Mathf.CeilToInt(placeHolder.transform.position.y) - 1;
                        int za = Mathf.CeilToInt(placeHolder.transform.position.z) - 1;


                        int3 chunkcoords = new int3(0, 0, 0);
                        int3 blockcoords = new int3(0, 0, 0);

                        int3 offset = InfiniteWorld.ChunkWorldPositionOffset;

                        /*
                        chunkcoords.x = Mathf.CeilToInt(((float)xa / (float)chunkSize.x - (float)offset.x)) - 1;
                        chunkcoords.y = Mathf.CeilToInt(((float)ya / (float)chunkSize.y - (float)offset.y)) - 1;
                        chunkcoords.z = Mathf.CeilToInt(((float)za / (float)chunkSize.z - (float)offset.z)) - 1;
                        */
                        chunkcoords.x = xa / chunkSize.x - offset.x;
                        chunkcoords.y = ya / chunkSize.y - offset.y;
                        chunkcoords.z = za / chunkSize.z - offset.z;

                        if (WorldInitializer.chunkArrayContainsKey(chunkcoords))
                        {
                            blockcoords.x = xa % chunkSize.x;
                            blockcoords.y = ya % chunkSize.y;
                            blockcoords.z = za % chunkSize.z;

                            if (blockcoords.x < 0)
                            {
                                blockcoords.x = chunkSize.x - Math.Abs(blockcoords.x);
                                chunkcoords.x -= 1;
                            }

                            if (blockcoords.y < 0)
                            {
                                blockcoords.y = chunkSize.y - Math.Abs(blockcoords.y);
                                chunkcoords.y-= 1;
                            }

                            if (blockcoords.z < 0)
                            {
                                blockcoords.z = chunkSize.z - Math.Abs(blockcoords.z);
                                chunkcoords.z -= 1;
                            }

                            if (chunkcoords.x < 0 || chunkcoords.y < 0 || chunkcoords.z < 0)
                            {
                                placeHolder.GetComponent<BoxCollider>().enabled = false;
                            }
                            else
                            {
                                blockTypes[x, 0, z] =
                                    WorldInitializer.chunkArray[chunkcoords.x, chunkcoords.y, chunkcoords.z].
                                    blocks[blockcoords.x, blockcoords.y, blockcoords.z].blockType;

                                if (blockTypes[x, 0, z] != BlockType.Air)
                                {
                                    placeHolder.GetComponent<BoxCollider>().enabled = true;
                                }
                                else
                                {
                                    placeHolder.GetComponent<BoxCollider>().enabled = false;
                                }
                            }
                        }

                        boxColliderGOs[x, 0, z] = placeHolder;
                    }
                }
            }
        }
    }
    private void zdir(float dirz)
    {
        GameObject placeHolder;
        if (dirz >= 1)
        {
            for (int iter = 0; iter < dirz; iter++)
            {
                for (int x = 0; x < arraySize.x; x++)
                {
                    for (int y = 0; y < arraySize.y; y++)
                    {

                        placeHolder = boxColliderGOs[x, y, 0];
                        for (int z = 0; z < arraySize.z - 1; z++)
                        {
                            boxColliderGOs[x, y, z] = boxColliderGOs[x, y, z + 1];
                        }

                        placeHolder.transform.position = new Vector3(
                            placeHolder.transform.position.x,
                            placeHolder.transform.position.y,
                            placeHolder.transform.position.z + arraySize.z);



                        int xa = Mathf.CeilToInt(placeHolder.transform.position.x) - 1;
                        int ya = Mathf.CeilToInt(placeHolder.transform.position.y) - 1;
                        int za = Mathf.CeilToInt(placeHolder.transform.position.z) - 1;


                        //Vector3 chunkcoords = new Vector3(0, 0, 0);
                        int3 chunkcoords = new int3(0, 0, 0);
                        int3 blockcoords = new int3(0, 0, 0);

                        int3 offset = InfiniteWorld.ChunkWorldPositionOffset;

                        /*
                        chunkcoords.x = Mathf.CeilToInt(((float)xa / (float)chunkSize.x - (float)offset.x)) - 1;
                        chunkcoords.y = Mathf.CeilToInt(((float)ya / (float)chunkSize.y - (float)offset.y)) - 1;
                        chunkcoords.z = Mathf.CeilToInt(((float)za / (float)chunkSize.z - (float)offset.z)) - 1;
                        */
                        chunkcoords.x = xa / chunkSize.x - offset.x;
                        chunkcoords.y = ya / chunkSize.y - offset.y;
                        chunkcoords.z = za / chunkSize.z - offset.z;

                        //Debug.Log(xa + " " + chunkcoords.x + " " + chunkSize.x + " " + offset.x);
                        if (WorldInitializer.chunkArrayContainsKey(chunkcoords))
                        {
                            blockcoords.x = xa % chunkSize.x;
                            blockcoords.y = ya % chunkSize.y;
                            blockcoords.z = za % chunkSize.z;

                                                      

                            if (blockcoords.x < 0)
                            {
                                blockcoords.x = chunkSize.x - Math.Abs(blockcoords.x);

                                chunkcoords.x -= 1;
                            }

                            if (blockcoords.y < 0)
                            {
                                blockcoords.y = chunkSize.y - Math.Abs(blockcoords.y);
                                chunkcoords.y -= 1;
                            }

                            if (blockcoords.z < 0)
                            {
                                blockcoords.z = chunkSize.z - Math.Abs(blockcoords.z);
                                chunkcoords.z -= 1;
                            }

                            if (chunkcoords.x < 0 || chunkcoords.y < 0 || chunkcoords.z < 0)
                            {
                                placeHolder.GetComponent<BoxCollider>().enabled = false;
                            }
                            else
                            {
                                blockTypes[x, y, arraySize.z - 1] = WorldInitializer.chunkArray[chunkcoords.x, chunkcoords.y, chunkcoords.z].
                                    blocks[blockcoords.x, blockcoords.y, blockcoords.z].blockType;

                                if (blockTypes[x, y, arraySize.z - 1] != BlockType.Air)
                                {
                                    placeHolder.GetComponent<BoxCollider>().enabled = true;
                                }
                                else
                                {
                                    placeHolder.GetComponent<BoxCollider>().enabled = false;
                                }
                            }
                        }
                        else
                        { 
                            placeHolder.GetComponent<BoxCollider>().enabled = false;
                        }
                        
                        boxColliderGOs[x, y, arraySize.z - 1] = placeHolder;
                    }
                }
            }
        }

        if (dirz <= -1)
        {
            for (int iter = 0; iter > dirz; iter--)
            {
                for (int y = 0; y < arraySize.y; y++)
                {
                    for (int x = 0; x < arraySize.x; x++)
                    {

                        placeHolder = boxColliderGOs[x, y, arraySize.z - 1];
                        for (int z = arraySize.z - 1; z > 0; z--)
                        {
                            boxColliderGOs[x, y, z] = boxColliderGOs[x, y, z - 1];
                        }

                        placeHolder.transform.position = new Vector3(
                            placeHolder.transform.position.x,
                            placeHolder.transform.position.y,
                            placeHolder.transform.position.z - arraySize.z);



                        int xa = Mathf.CeilToInt(placeHolder.transform.position.x) - 1;
                        int ya = Mathf.CeilToInt(placeHolder.transform.position.y) - 1;
                        int za = Mathf.CeilToInt(placeHolder.transform.position.z) - 1;


                        int3 chunkcoords = new int3(0, 0, 0);
                        int3 blockcoords = new int3(0, 0, 0);

                        int3 offset = InfiniteWorld.ChunkWorldPositionOffset;

                        /*
                        chunkcoords.x = Mathf.CeilToInt(((float)xa / (float)chunkSize.x - (float)offset.x)) - 1;
                        chunkcoords.y = Mathf.CeilToInt(((float)ya / (float)chunkSize.y - (float)offset.y)) - 1;
                        chunkcoords.z = Mathf.CeilToInt(((float)za / (float)chunkSize.z - (float)offset.z)) - 1;
                        */
                        chunkcoords.x = xa / chunkSize.x - offset.x;
                        chunkcoords.y = ya / chunkSize.y - offset.y;
                        chunkcoords.z = za / chunkSize.z - offset.z;

                        if (WorldInitializer.chunkArrayContainsKey(chunkcoords))
                        {
                            blockcoords.x = xa % chunkSize.x;
                            blockcoords.y = ya % chunkSize.y;
                            blockcoords.z = za % chunkSize.z;

                            if (blockcoords.x < 0)
                            {
                                blockcoords.x = chunkSize.x - Math.Abs(blockcoords.x);
                                chunkcoords.x -= 1;
                            }

                            if (blockcoords.y < 0)
                            {
                                blockcoords.y = chunkSize.y - Math.Abs(blockcoords.y);
                                chunkcoords.y-= 1;
                            }

                            if (blockcoords.z < 0)
                            {
                                blockcoords.z = chunkSize.z - Math.Abs(blockcoords.z);
                                chunkcoords.z -= 1;
                            }

                            if (chunkcoords.x < 0 || chunkcoords.y < 0 || chunkcoords.z < 0)
                            {
                                placeHolder.GetComponent<BoxCollider>().enabled = false;
                            }
                            else
                            {
                                blockTypes[x, y, 0] = WorldInitializer.chunkArray[chunkcoords.x, chunkcoords.y, chunkcoords.z].
                                    blocks[blockcoords.x, blockcoords.y, blockcoords.z].blockType;
                                //Debug.Log(chunkcoords.x + " " + chunkcoords.y + " " + chunkcoords.z);
                                //Debug.Log(blockcoords.x + " " + blockcoords.y + " " + blockcoords.z);
                                if (blockTypes[x, y, 0] != BlockType.Air)
                                {
                                    placeHolder.GetComponent<BoxCollider>().enabled = true;
                                }
                                else
                                {
                                    placeHolder.GetComponent<BoxCollider>().enabled = false;
                                }
                            }
                        }
                        boxColliderGOs[x, y, 0] = placeHolder;
                    }
                }
            }
        }
    }
}