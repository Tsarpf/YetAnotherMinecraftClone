using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;



public class Chunk
{
    public int3 chunkPosWorld;
    Vector3 cvhdhsdv;
    public Vector3 blockOffset;
    int3 chunkOffset;

    public BlockType[] blocksArray;

    public GameObject chunk;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    MeshCollider meshCollider;

    public bool anyNonAir = true;

    public static Dictionary<int, Dictionary<BlockType, Vector2>> UVLookUp;

    public BlockType getBlock(int x, int y, int z)
    {
        return blocksArray[getBlockIdx(x, y, z)];
    }

    public void setBlock(int x, int y, int z, BlockType block)
    {
        blocksArray[getBlockIdx(x, y, z)] = block;
    }

    private int getBlockIdx(int x, int y, int z)
    {
        return x + CONST.chunkSize.x * (y + CONST.chunkSize.y * z);
    }

    bool firstRun = true;
    //Get the block positions and stuff here. Do not draw them yet though.
    public Chunk(int3 chunkposwrld)
    {
        chunkOffset = new int3(0, 0, 0);
        chunkPosWorld = chunkposwrld;
        Vector3 chunkSize = new Vector3(CONST.chunkSize.x, CONST.chunkSize.y, CONST.chunkSize.z);

        blockOffset.x = chunkposwrld.x * (chunkSize.x * ((chunkSize.x - 1) / chunkSize.x));
        blockOffset.y = chunkposwrld.y * (chunkSize.y * ((chunkSize.y - 1) / chunkSize.y));
        blockOffset.z = chunkposwrld.z * (chunkSize.z * ((chunkSize.z - 1) / chunkSize.z));

        //Debug.Log(blockOffset.x + " " + blockOffset.y + " " + blockOffset.z);
        //Debug.Log(cvhdhsdv.x + " " + cvhdhsdv.y + " " + cvhdhsdv.z);
        //Debug.Log(CONST.chunkSize.x + " " + CONST.chunkSize.y + " " + CONST.chunkSize.z);

        blocksArray = new BlockType[CONST.chunkSize.x * CONST.chunkSize.y * CONST.chunkSize.z];

        //Call the terrain generator. Terrain generator simply tells what blocktypes to put and where.
        //GenericChunkGen.GenerateChunk(this);
        TerraGen.CreateLandscape(this);
    }

    public Chunk(int3 chunkposwrld, int3 offset)
    {
        chunkOffset = offset;

        chunkPosWorld = chunkposwrld;

        Vector3 chunkSize = new Vector3(CONST.chunkSize.x, CONST.chunkSize.y, CONST.chunkSize.z);

        blockOffset.x = chunkposwrld.x * (chunkSize.x * ((chunkSize.x - 1) / chunkSize.x));
        blockOffset.y = chunkposwrld.y * (chunkSize.y * ((chunkSize.y - 1) / chunkSize.y));
        blockOffset.z = chunkposwrld.z * (chunkSize.z * ((chunkSize.z - 1) / chunkSize.z));

        //Debug.Log(blockOffset.x + " " + blockOffset.y + " " + blockOffset.z);
        //Debug.Log(cvhdhsdv.x + " " + cvhdhsdv.y + " " + cvhdhsdv.z);
        //Debug.Log(CONST.chunkSize.x + " " + CONST.chunkSize.y + " " + CONST.chunkSize.z);

        blocksArray = new BlockType[CONST.chunkSize.x * CONST.chunkSize.y * CONST.chunkSize.z];

        //Call the terrain generator. Terrain generator simply tells what blocktypes to put and where.
        //GenericChunkGen.GenerateChunk(this);
        TerraGen.CreateLandscape(this);
    }
    static int totalVertexAndUVCount = CONST.chunkSize.x * CONST.chunkSize.y * CONST.chunkSize.z * 24;
    static int totalTriangleCount = CONST.chunkSize.x * CONST.chunkSize.y * CONST.chunkSize.z * 36;
    public void AddChunkDrawdataToMeshQueue(System.Object stateInfo)
    {
        int3 chunkSize = CONST.chunkSize;

        ChunkDrawData chunkDrawData = new ChunkDrawData(new List<Vector3>(), new List<int>(), new List<Vector2>(), this);

        int offset = 0;

        BlockDrawInfo drawData;
        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int y = 0; y < chunkSize.y; y++)
            {
                for (int z = 0; z < chunkSize.z; z++)
                {
                    if (getBlock(x, y, z) != BlockType.Air)
                    {
                        drawData = getBlockDrawData(x, y, z, offset);

                        chunkDrawData.vertexList.AddRange(drawData.blockVertexList);
                        chunkDrawData.UVList.AddRange(drawData.blockUVList);
                        /*
                        int j = 0;
                        foreach (int val in drawData.blockTriangleList)
                        {
                            drawData.blockTriangleList[j] = val + offset;
                            j++;
                        }
                        */
                        offset += drawData.triangleCount;

                        chunkDrawData.triangleList.AddRange(drawData.blockTriangleList);
                    }
                }
            }
        }

        ChunkDrawDataArray cdda = new ChunkDrawDataArray(chunkDrawData.vertexList.ToArray(),
            chunkDrawData.triangleList.ToArray(), chunkDrawData.UVList.ToArray(), this);

        MeshGenerationQueue.AddToQueue(cdda);
    }
    
    public void AddChunkDrawdataToMeshQueue()
    {

        int3 chunkSize = CONST.chunkSize;

        ChunkDrawData chunkDrawData = new ChunkDrawData(new List<Vector3>(), new List<int>(), new List<Vector2>(),this);

        int offset = 0;

        BlockDrawInfo drawData;
        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int y = 0; y < chunkSize.y; y++)
            {
                for (int z = 0; z < chunkSize.z; z++)
                {
                    if (getBlock(x, y, z) != BlockType.Air)
                    {


                        drawData = getBlockDrawData(x, y, z, offset);

                        chunkDrawData.vertexList.AddRange(drawData.blockVertexList);
                        chunkDrawData.UVList.AddRange(drawData.blockUVList);

                        /*
                        int j = 0;
                        foreach (int val in drawData.blockTriangleList)
                        {
                            drawData.blockTriangleList[j] = val + offset;
                            j++;
                        }
                        */

                        offset += drawData.triangleCount;

                        chunkDrawData.triangleList.AddRange(drawData.blockTriangleList);
                    }
                }
            }
        }

        ChunkDrawDataArray cdda = new ChunkDrawDataArray(chunkDrawData.vertexList.ToArray(),
            chunkDrawData.triangleList.ToArray(), chunkDrawData.UVList.ToArray(), this);

        MeshGenerationQueue.AddToQueue(cdda);

    }

    private BlockDrawInfo getBlockDrawData(int x, int y, int z, int offset)
    {
        //Profiler.BeginSample("AAAAAAAAAAAAAABBBBBCCCCCCCC");
        //Profiler.BeginSample("Neighbours");
        BlockType[] neighbours = getNeighbourBlocks(x,y,z);
        //Profiler.EndSample();

        //Profiler.BeginSample("grasscheck");
        if (firstRun)
        {
            if (getBlock(x, y, z) == BlockType.Dirt)
            {
                if (neighbours[(int)Neighbour.Y_plus] == BlockType.Air)
                {
                    //blocks[x, y, z].blockType = BlockType.Grass;
                    setBlock(x, y, z, BlockType.Grass);
                }
            }
        }
        //Profiler.EndSample();
        //Profiler.BeginSample("UVs");
        BlockType current = getBlock(x, y, z);

        //Profiler.EndSample();

        //Profiler.BeginSample("vertsdrawpos");
        //Vector3[] vertsDrawPos = new Vector3[24];
        Vector3 pos = new Vector3(x + blockOffset.x, y + blockOffset.y, z + blockOffset.z);
        //24 vertices per cube, hence i < 24
        /*
        for (int i = 0; i < 24; i++)
        {
            vertsDrawPos[i] = genericVerts[i] + pos;
        }
        */
        //Profiler.EndSample();
        //Profiler.BeginSample("declaring");
        List<Vector3> blockVertexList = new List<Vector3>();
        List<Vector2> blockUVList = new List<Vector2>();
        List<int> blockTriangleList = new List<int>();
        int triangleIndex = 0;
        bool enclosed = false;
        //Profiler.EndSample();
        /*
        BlockType neighbours[0] = neighbours[0];
        BlockType neighbours[1] = neighbours[1];

        BlockType neighbours[2] = neighbours[2];
        BlockType neighbours[3] = neighbours[3];

        BlockType neighbours[4] = neighbours[4];
        BlockType neighbours[5] = neighbours[5];
        */
        //Profiler.BeginSample("facecheck");
        // Check which sides to draw and get the vertex, UV and triangle lists accordingly. Also triangle count.
        #region populate the lists using the stuff
        // First let's check if the whole block is surrounded, in which case let's not add anything to the vertex nor triangle lists.
        if (neighbours[0] != BlockType.Air &&
            neighbours[1] != BlockType.Air &&
            neighbours[2] != BlockType.Air &&
            neighbours[3] != BlockType.Air &&
            neighbours[5] != BlockType.Air &&
            neighbours[4] != BlockType.Air)
        {
            enclosed = true;
        }

        if (!enclosed)
        {
            // Then proceed to checking each face at a time and adding the draw-info accordingly.
            if (neighbours[5] == BlockType.Air) // Front
            {
                
                blockVertexList.Add(genericVerts[0] + pos);
                blockVertexList.Add(genericVerts[1] + pos);
                blockVertexList.Add(genericVerts[2] + pos);
                blockVertexList.Add(genericVerts[3] + pos);
                //blockVertexList.Add(vertsDrawPos[0]);
                //blockVertexList.Add(vertsDrawPos[1]);
                //blockVertexList.Add(vertsDrawPos[2]);
                //blockVertexList.Add(vertsDrawPos[3]);
                

                blockUVList.Add(UVLookUp[0][current]);
                blockUVList.Add(UVLookUp[1][current]);
                blockUVList.Add(UVLookUp[2][current]);
                blockUVList.Add(UVLookUp[3][current]);


                blockTriangleList.Add(3 + triangleIndex + offset);
                blockTriangleList.Add(1 + triangleIndex + offset);
                blockTriangleList.Add(0 + triangleIndex + offset);

                blockTriangleList.Add(0 + triangleIndex + offset);
                blockTriangleList.Add(2 + triangleIndex + offset);
                blockTriangleList.Add(3 + triangleIndex + offset);

                triangleIndex += 4;
            }

            if (neighbours[4] == BlockType.Air) // Back
            {
                //blockVertexList.Add(vertsDrawPos[20]);
                //blockVertexList.Add(vertsDrawPos[21]);
                //blockVertexList.Add(vertsDrawPos[22]);
                //blockVertexList.Add(vertsDrawPos[23]);
                blockVertexList.Add(genericVerts[20] + pos);
                blockVertexList.Add(genericVerts[21] + pos);
                blockVertexList.Add(genericVerts[22] + pos);
                blockVertexList.Add(genericVerts[23] + pos);

                blockUVList.Add(UVLookUp[20][current]);
                blockUVList.Add(UVLookUp[21][current]);
                blockUVList.Add(UVLookUp[22][current]);
                blockUVList.Add(UVLookUp[23][current]);

                blockTriangleList.Add(3 + triangleIndex + offset);
                blockTriangleList.Add(1 + triangleIndex + offset);
                blockTriangleList.Add(0 + triangleIndex + offset);

                blockTriangleList.Add(0 + triangleIndex + offset);
                blockTriangleList.Add(2 + triangleIndex + offset);
                blockTriangleList.Add(3 + triangleIndex + offset);

                triangleIndex += 4;
            }

            if (neighbours[0] == BlockType.Air) // Left
            {
                //blockVertexList.Add(vertsDrawPos[8]);
                //blockVertexList.Add(vertsDrawPos[9]);
                //blockVertexList.Add(vertsDrawPos[10]);
                //blockVertexList.Add(vertsDrawPos[11]);
                blockVertexList.Add(genericVerts[8] + pos);
                blockVertexList.Add(genericVerts[9] + pos);
                blockVertexList.Add(genericVerts[10] + pos);
                blockVertexList.Add(genericVerts[11] + pos);

                blockUVList.Add(UVLookUp[8][current]);
                blockUVList.Add(UVLookUp[9][current]);
                blockUVList.Add(UVLookUp[10][current]);
                blockUVList.Add(UVLookUp[11][current]);

                blockTriangleList.Add(3 + triangleIndex + offset);
                blockTriangleList.Add(1 + triangleIndex + offset);
                blockTriangleList.Add(0 + triangleIndex + offset);

                blockTriangleList.Add(0 + triangleIndex + offset);
                blockTriangleList.Add(2 + triangleIndex + offset);
                blockTriangleList.Add(3 + triangleIndex + offset);

                triangleIndex += 4;
            }

            if (neighbours[1] == BlockType.Air) // Right
            {
                //blockVertexList.Add(vertsDrawPos[4]);
                //blockVertexList.Add(vertsDrawPos[5]);
                //blockVertexList.Add(vertsDrawPos[6]);
                //blockVertexList.Add(vertsDrawPos[7]);
                blockVertexList.Add(genericVerts[4] + pos);
                blockVertexList.Add(genericVerts[5] + pos);
                blockVertexList.Add(genericVerts[6] + pos);
                blockVertexList.Add(genericVerts[7] + pos);


                blockUVList.Add(UVLookUp[4][current]);
                blockUVList.Add(UVLookUp[5][current]);
                blockUVList.Add(UVLookUp[6][current]);
                blockUVList.Add(UVLookUp[7][current]);


                blockTriangleList.Add(0 + triangleIndex + offset);
                blockTriangleList.Add(2 + triangleIndex + offset);
                blockTriangleList.Add(3 + triangleIndex + offset);

                blockTriangleList.Add(3 + triangleIndex + offset);
                blockTriangleList.Add(1 + triangleIndex + offset);
                blockTriangleList.Add(0 + triangleIndex + offset);

                triangleIndex += 4;
            }

            if (neighbours[2] == BlockType.Air) // Bottom
            {
                //blockVertexList.Add(vertsDrawPos[16]);
                //blockVertexList.Add(vertsDrawPos[17]);
                //blockVertexList.Add(vertsDrawPos[18]);
                //blockVertexList.Add(vertsDrawPos[19]);
                blockVertexList.Add(genericVerts[16] + pos);
                blockVertexList.Add(genericVerts[17] + pos);
                blockVertexList.Add(genericVerts[18] + pos);
                blockVertexList.Add(genericVerts[19] + pos);

                blockUVList.Add(UVLookUp[16][current]);
                blockUVList.Add(UVLookUp[17][current]);
                blockUVList.Add(UVLookUp[18][current]);
                blockUVList.Add(UVLookUp[19][current]);

                blockTriangleList.Add(0 + triangleIndex + offset);
                blockTriangleList.Add(1 + triangleIndex + offset);
                blockTriangleList.Add(3 + triangleIndex + offset);

                blockTriangleList.Add(3 + triangleIndex + offset);
                blockTriangleList.Add(2 + triangleIndex + offset);
                blockTriangleList.Add(0 + triangleIndex + offset);

                triangleIndex += 4;
            }

            if (neighbours[3] == BlockType.Air) // Up
            {
                //blockVertexList.Add(vertsDrawPos[12]);
                //blockVertexList.Add(vertsDrawPos[13]);
                //blockVertexList.Add(vertsDrawPos[14]);
                //blockVertexList.Add(vertsDrawPos[15]);
                blockVertexList.Add(genericVerts[12] + pos);
                blockVertexList.Add(genericVerts[13] + pos);
                blockVertexList.Add(genericVerts[14] + pos);
                blockVertexList.Add(genericVerts[15] + pos);

                blockUVList.Add(UVLookUp[12][current]);
                blockUVList.Add(UVLookUp[13][current]);
                blockUVList.Add(UVLookUp[14][current]);
                blockUVList.Add(UVLookUp[15][current]);


                blockTriangleList.Add(3 + triangleIndex + offset);
                blockTriangleList.Add(1 + triangleIndex + offset);
                blockTriangleList.Add(0 + triangleIndex + offset);

                blockTriangleList.Add(0 + triangleIndex + offset);
                blockTriangleList.Add(2 + triangleIndex + offset);
                blockTriangleList.Add(3 + triangleIndex + offset);

                triangleIndex += 4;
            }
        }

        #endregion
        // Dis a long region cuz I suck at coding.
        //Profiler.EndSample();

        //BlockDrawInfo returnStruct = new BlockDrawInfo(blockVertexList, blockUVList, blockTriangleList, triangleIndex);
        //Profiler.EndSample();
        return new BlockDrawInfo(blockVertexList, blockUVList, blockTriangleList, triangleIndex);

    }
    /*
    public void getCollisionMeshRectanglesForFace()
    {
        /*
        int width = -1; // Changements should always occur so force an error if notsdfglsd
        int height= -1;
            
        switch(face)
        {
            case Neighbour.X_minus:
                width = CONST.chunkSize.y;
                height = CONST.chunkSize.z;
            break;
            case Neighbour.X_plus:
                width = CONST.chunkSize.y;
                height = CONST.chunkSize.z;
            break;
            case Neighbour.Y_minus:
                width = CONST.chunkSize.x;
                height = CONST.chunkSize.z;
            break;
            case Neighbour.Y_plus:
                width = CONST.chunkSize.x;
                height = CONST.chunkSize.z;
            break;
           case Neighbour.Z_minus:
                width = CONST.chunkSize.x;
                height = CONST.chunkSize.y;
            break;
            case Neighbour.Z_plus:
                width = CONST.chunkSize.x;
                height = CONST.chunkSize.y;
            break;
        }
        

        bool[,] tileNeedsChecking = new bool[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tileNeedsChecking[i, j] = true;
            }
        }
     
        bool[,] tileNeedsChecking = new bool[CONST.chunkSize.x, CONST.chunkSize.y];
        for (int i = 0; i < CONST.chunkSize.x; i++)
        {
            for (int j = 0; j < CONST.chunkSize.y; j++)
            {
                tileNeedsChecking[i, j] = true;
            }
        }

        int selectionWidthEnd;
        int selectionWidthStart = 0;
        int selectionHeightStart = 0;
        int selectionHeightEnd = 0;
        BlockType comparer = blocks[0,0,0].blockType;
        List<Cubicle3D> rectangles = new List<Cubicle3D>();
        Vector3 start;
        Vector3 end;

        for (int z = 0; z < CONST.chunkSize.z; z++)
        {
            for (int y = 0; y < CONST.chunkSize.y; y++)
            {
                for (int x = 0; x < CONST.chunkSize.x; x++)
                {
                    if (tileNeedsChecking[x, y] == false) { continue; }

                    if (blocks[x, y, z].blockType == comparer)
                    {
                        tileNeedsChecking[x, y] = false;
                    }

                    else
                    {
                        selectionWidthEnd = x - 1;
                        //selectionHeightStart + 1 because we already went through the first row.
                        for (int y2 = selectionHeightStart + 1; y2 < CONST.chunkSize.y; y2++)
                        {
                            for (int x2 = selectionWidthStart; x2 <= selectionWidthEnd; x2++)
                            {
                                if (blocks[x2, y2, z].blockType == comparer) { tileNeedsChecking[x, y] = false; }
                                if (blocks[x2, y2, z].blockType != comparer)
                                {
                                    //This row wasn't homogenous, end selection rectangle at the previous row.
                                    selectionHeightEnd = y2 - 1;

                                    start = new Vector3(selectionWidthStart, selectionHeightStart, z);
                                    end = new Vector3(selectionWidthEnd, selectionHeightEnd, z);

                                    rectangles.Add(new Cubicle3D(start, end));
                                    goto endThisRectangle;
                                }
                                else if (x2 == selectionWidthEnd && y2 == CONST.chunkSize.y - 1)
                                {
                                    //Means we reached the chunk's border.
                                    start = new Vector3(selectionWidthStart, selectionHeightStart, z);
                                    end = new Vector3(selectionWidthEnd, selectionHeightEnd, z);

                                    rectangles.Add(new Cubicle3D(start, end));
                                    //goto endRectangle; //Dunno if this is needed.
                                }
                            }
                        }
                    endThisRectangle: ;
                        Vector2 asd = findNextTile(tileNeedsChecking);
                        if (asd.x == -1)
                        {
                            goto faceFinished;
                        }
                        int xindex = (int)asd.x;
                        int yindex = (int)asd.y;
                        comparer = blocks[xindex, yindex, z].blockType;
                        selectionWidthStart = xindex;
                        selectionHeightStart = yindex;
                    }
                }
            }
        }
    faceFinished: ;

    }
    */
    /*
    private Vector2 findNextTile(bool[,] tiles)
    {
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                if (tiles[x, y] == true)
                {
                    return new Vector2(x, y);
                }
            }
        }
        //Return [-1,-1] if all indexes are false;
        return new Vector2(-1, -1);
    }
    */
    public static Vector2 getUVsForPreGeneration(int idx, BlockType blockType)
    {
        switch(idx)
        {
            case 0:      
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);         // Front
            case 1:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);
            case 2:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
            case 3:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
            case 4:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);    // Left
            case 5:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);
            case 6:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
            case 7:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
            case 8:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);    // Right
            case 9:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);
            case 10:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
            case 11:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
            case 12:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].yMin);   // Top
            case 13:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].xMax, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].yMin);
            case 14:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].yMax);
            case 15:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].xMax, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].yMax);
            case 16:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].yMin);   // Bottom
            case 17:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].xMax, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].yMin);
            case 18:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].yMax);
            case 19:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].xMax, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].yMax);
            case 20:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);   // Back
            case 21:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);
            case 22:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
            case 23:
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
            default:
                Debug.Log("Fail");
                return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
        }
    }
    static Vector3[] genericVerts = new Vector3[24]
        {
            // Front
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),

            
            new Vector3(1, 1, 1),
            new Vector3(1, 1, 0),
            new Vector3(1, 0, 1),
            new Vector3(1, 0, 0),

            
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1),
             new Vector3(0, 0, 0),
             new Vector3(0, 0, 1),

            
             new Vector3(0, 1, 0),
             new Vector3(1, 1, 0),
             new Vector3(0, 1, 1),
             new Vector3(1, 1, 1),

            
             new Vector3(0, 0, 0),
             new Vector3(1, 0, 0),
             new Vector3(0, 0, 1),
             new Vector3(1, 0, 1),

            
             new Vector3(1, 1, 0),
             new Vector3(0, 1, 0),
             new Vector3(1, 0, 0),
             new Vector3(0, 0, 0)
        };

    private BlockType[] getNeighbourBlocks(int x, int y, int z)
    {
        int3 offset = InfiniteWorld.ChunkWorldPositionOffset;

        int3 arrayBasedChunkPos = new int3(chunkPosWorld.x - offset.x, chunkPosWorld.y - offset.y, chunkPosWorld.z - offset.z);

        

        BlockType x_minus;
        BlockType x_plus;

        BlockType y_minus;
        BlockType y_plus;

        BlockType z_minus;
        BlockType z_plus;

        int xKey, yKey, zKey;


        //Xminus
        if (x - 1 >= 0)
        {
            //x_minus = blocks[x - 1, y, z].blockType;
            x_minus = getBlock(x - 1, y, z);
        }
        else
        {
            //key = new int3(arrayBasedChunkPos.x - 1, arrayBasedChunkPos.y, arrayBasedChunkPos.z);
            xKey = arrayBasedChunkPos.x - 1;
            yKey = arrayBasedChunkPos.y;
            zKey = arrayBasedChunkPos.z;
            if (WorldInitializer.chunkArrayContainsKey(xKey, yKey, zKey))
            {
                x_minus = WorldInitializer.getChunk(xKey, yKey, zKey).getBlock(CONST.chunkSize.x - 1, y, z);
            }
            else
            {
                x_minus = BlockType.Air;
            }
        }

        //Xplus
        if (x + 1 <= CONST.chunkSize.x - 1)
        {
            x_plus = getBlock(x + 1, y, z);
        }
        else
        {
            xKey = arrayBasedChunkPos.x + 1;
            yKey = arrayBasedChunkPos.y;
            zKey = arrayBasedChunkPos.z;
            if (WorldInitializer.chunkArrayContainsKey(xKey, yKey, zKey))
            {
                x_plus = WorldInitializer.getChunk(xKey, yKey, zKey).getBlock(0, y, z);
            }
            else
            {
                x_plus = BlockType.Air;
            }
        }


        //Yminus
        if (y - 1 >= 0)
        {
            y_minus = getBlock(x, y - 1, z);
        }
        else
        {
            xKey = arrayBasedChunkPos.x;
            yKey = arrayBasedChunkPos.y - 1 ;
            zKey = arrayBasedChunkPos.z;
            if (WorldInitializer.chunkArrayContainsKey(xKey, yKey, zKey))
            {
                y_minus = WorldInitializer.getChunk(xKey, yKey, zKey).getBlock(x, CONST.chunkSize.y - 1, z);
            }
            else
            {
                y_minus = BlockType.Air;
            }
        }

        //Yplus
        if (y + 1 <= CONST.chunkSize.y - 1)
        {
            y_plus = getBlock(x, y + 1, z);
        }
        else
        {
            xKey = arrayBasedChunkPos.x;
            yKey = arrayBasedChunkPos.y + 1;
            zKey = arrayBasedChunkPos.z;
            if (WorldInitializer.chunkArrayContainsKey(xKey, yKey, zKey))
            {
                y_plus = WorldInitializer.getChunk(xKey, yKey, zKey).getBlock(x, 0, z);
            }
            else
            {
                y_plus = BlockType.Air;
            }
        }


        //Zminus
        if (z - 1 >= 0)
        {
            z_minus = getBlock(x, y, z - 1);
        }
        else
        {
            xKey = arrayBasedChunkPos.x;
            yKey = arrayBasedChunkPos.y;
            zKey = arrayBasedChunkPos.z - 1;
            if (WorldInitializer.chunkArrayContainsKey(xKey, yKey, zKey))
            {
                z_minus = WorldInitializer.getChunk(xKey, yKey, zKey).getBlock(x, y, CONST.chunkSize.z - 1);
            }
            else
            {
                z_minus = BlockType.Air;
            }
        }

        //Zplus
        if (z + 1 <= CONST.chunkSize.z - 1)
        {
            z_plus = getBlock(x, y, z + 1);
        }
        else
        {
            xKey = arrayBasedChunkPos.x;
            yKey = arrayBasedChunkPos.y;
            zKey = arrayBasedChunkPos.z + 1;
            if (WorldInitializer.chunkArrayContainsKey(xKey, yKey, zKey))
            {
                z_plus = WorldInitializer.getChunk(xKey, yKey, zKey).getBlock(x, y, 0);
            }
            else
            {
                z_plus = BlockType.Air;
            }
        }


        BlockType[] neighbours = new BlockType[6];

        neighbours[0] = x_minus;
        neighbours[1] = x_plus;

        neighbours[2] = y_minus;
        neighbours[3] = y_plus;

        neighbours[4] = z_minus;
        neighbours[5] = z_plus;

        return neighbours;
    }

    public void updateMesh(Vector3[] verts, Vector2[] UVs, int[] triangles)
    {
        if (firstRun)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = verts;
            mesh.uv = UVs;
            mesh.triangles = triangles;

            chunk = new GameObject("Chunk (" + chunkPosWorld.x + "," + chunkPosWorld.y + "," + chunkPosWorld.z + ")");
            chunk.transform.position = new Vector3(chunkPosWorld.x, chunkPosWorld.y, chunkPosWorld.z);
            chunk.AddComponent("MeshFilter");
            chunk.AddComponent("MeshRenderer");


            chunk.renderer.material.mainTexture = WorldInitializer.textureAtlas;
            chunk.renderer.material.shader = shader;
            
            chunk.AddComponent("MeshCollider");
			meshCollider = chunk.GetComponent<MeshCollider>();
			meshCollider.sharedMesh = mesh;
            

            meshFilter = chunk.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            meshFilter.mesh.RecalculateNormals();
            meshFilter.mesh.RecalculateBounds();
            firstRun = false;
        }
        else
        {
            meshCollider.sharedMesh = null;
            Mesh mesh = meshFilter.mesh;

            mesh.Clear();
            mesh.vertices = verts;
            mesh.uv = UVs;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            meshCollider.sharedMesh = mesh;
        }
    }
    static Shader shader = Shader.Find("Unlit/Texture");
}
