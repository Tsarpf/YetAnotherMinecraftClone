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

    public Block[, ,] blocks;

    public GameObject chunk;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    MeshCollider meshCollider;

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

        blocks = new Block[CONST.chunkSize.x, CONST.chunkSize.y, CONST.chunkSize.z];

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

        blocks = new Block[CONST.chunkSize.x, CONST.chunkSize.y, CONST.chunkSize.z];

        //Call the terrain generator. Terrain generator simply tells what blocktypes to put and where.
        //GenericChunkGen.GenerateChunk(this);
        TerraGen.CreateLandscape(this);
    }

    public void AddChunkDrawdataToMeshQueue(System.Object stateInfo)
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
                    if (blocks[x, y, z].blockType != BlockType.Air)
                    {


                        drawData = getBlockDrawData(x, y, z);

                        chunkDrawData.vertexList.AddRange(drawData.blockVertexList);
                        chunkDrawData.UVList.AddRange(drawData.blockUVList);

                        int j = 0;
                        foreach (int val in drawData.blockTriangleList)
                        {
                            drawData.blockTriangleList[j] = val + offset;
                            j++;
                        }

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
                    if (blocks[x, y, z].blockType != BlockType.Air)
                    {


                        drawData = getBlockDrawData(x, y, z);

                        chunkDrawData.vertexList.AddRange(drawData.blockVertexList);
                        chunkDrawData.UVList.AddRange(drawData.blockUVList);

                        int j = 0;
                        foreach (int val in drawData.blockTriangleList)
                        {
                            drawData.blockTriangleList[j] = val + offset;
                            j++;
                        }

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

    private BlockDrawInfo getBlockDrawData(int x, int y, int z)
    {
        //Profiler.BeginSample("AAAAAAAAAAAAAABBBBBCCCCCCCC");
        //Profiler.BeginSample("Neighbours");
        BlockType[] neighbours = getNeighbourBlocks(x,y,z);
        //Profiler.EndSample();

        //Profiler.BeginSample("grasscheck");
        if (firstRun == true)
        {
            if (blocks[x, y, z].blockType == BlockType.Dirt)
            {
                if (neighbours[(int)Neighbour.Y_plus] == BlockType.Air)
                {
                    blocks[x, y, z].blockType = BlockType.Grass;
                }
            }
        }
        //Profiler.EndSample();
        //Profiler.BeginSample("UVs");
        BlockType asd = blocks[x, y, z].blockType;

        //Profiler.EndSample();

        //Profiler.BeginSample("vertsdrawpos");
        Vector3[] vertsDrawPos = new Vector3[24];
        Vector3 pos = new Vector3(x + blockOffset.x, y + blockOffset.y, z + blockOffset.z);
        //24 vertices per cube, hence i < 24
        for (int i = 0; i < 24; i++)
        {
            vertsDrawPos[i] = genericVerts(i) + pos;
        }
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
                blockVertexList.Add(vertsDrawPos[0]);
                blockVertexList.Add(vertsDrawPos[1]);
                blockVertexList.Add(vertsDrawPos[2]);
                blockVertexList.Add(vertsDrawPos[3]);

                blockUVList.Add(getUVs(0,asd));
                blockUVList.Add(getUVs(1,asd));
                blockUVList.Add(getUVs(2,asd));
                blockUVList.Add(getUVs(3,asd));

                blockTriangleList.Add(3 + triangleIndex);
                blockTriangleList.Add(1 + triangleIndex);
                blockTriangleList.Add(0 + triangleIndex);

                blockTriangleList.Add(0 + triangleIndex);
                blockTriangleList.Add(2 + triangleIndex);
                blockTriangleList.Add(3 + triangleIndex);

                triangleIndex += 4;
            }

            if (neighbours[4] == BlockType.Air) // Back
            {
                blockVertexList.Add(vertsDrawPos[20]);
                blockVertexList.Add(vertsDrawPos[21]);
                blockVertexList.Add(vertsDrawPos[22]);
                blockVertexList.Add(vertsDrawPos[23]);

                blockUVList.Add(getUVs(20,asd));
                blockUVList.Add(getUVs(21,asd));
                blockUVList.Add(getUVs(22,asd));
                blockUVList.Add(getUVs(23,asd));

                blockTriangleList.Add(3 + triangleIndex);
                blockTriangleList.Add(1 + triangleIndex);
                blockTriangleList.Add(0 + triangleIndex);

                blockTriangleList.Add(0 + triangleIndex);
                blockTriangleList.Add(2 + triangleIndex);
                blockTriangleList.Add(3 + triangleIndex);

                triangleIndex += 4;
            }

            if (neighbours[0] == BlockType.Air) // Left
            {
                blockVertexList.Add(vertsDrawPos[8]);
                blockVertexList.Add(vertsDrawPos[9]);
                blockVertexList.Add(vertsDrawPos[10]);
                blockVertexList.Add(vertsDrawPos[11]);

                blockUVList.Add(getUVs(8,asd));
                blockUVList.Add(getUVs(9,asd));
                blockUVList.Add(getUVs(10,asd));
                blockUVList.Add(getUVs(11,asd));

                blockTriangleList.Add(3 + triangleIndex);
                blockTriangleList.Add(1 + triangleIndex);
                blockTriangleList.Add(0 + triangleIndex);

                blockTriangleList.Add(0 + triangleIndex);
                blockTriangleList.Add(2 + triangleIndex);
                blockTriangleList.Add(3 + triangleIndex);

                triangleIndex += 4;
            }

            if (neighbours[1] == BlockType.Air) // Right
            {
                blockVertexList.Add(vertsDrawPos[4]);
                blockVertexList.Add(vertsDrawPos[5]);
                blockVertexList.Add(vertsDrawPos[6]);
                blockVertexList.Add(vertsDrawPos[7]);

                blockUVList.Add(getUVs(4,asd));
                blockUVList.Add(getUVs(5,asd));
                blockUVList.Add(getUVs(6,asd));
                blockUVList.Add(getUVs(7,asd));


                blockTriangleList.Add(0 + triangleIndex);
                blockTriangleList.Add(2 + triangleIndex);
                blockTriangleList.Add(3 + triangleIndex);

                blockTriangleList.Add(3 + triangleIndex);
                blockTriangleList.Add(1 + triangleIndex);
                blockTriangleList.Add(0 + triangleIndex);

                triangleIndex += 4;
            }

            if (neighbours[2] == BlockType.Air) // Bottom
            {
                blockVertexList.Add(vertsDrawPos[16]);
                blockVertexList.Add(vertsDrawPos[17]);
                blockVertexList.Add(vertsDrawPos[18]);
                blockVertexList.Add(vertsDrawPos[19]);

                blockUVList.Add(getUVs(16,asd));
                blockUVList.Add(getUVs(17,asd));
                blockUVList.Add(getUVs(18,asd));
                blockUVList.Add(getUVs(19,asd));


                blockTriangleList.Add(0 + triangleIndex);
                blockTriangleList.Add(1 + triangleIndex);
                blockTriangleList.Add(3 + triangleIndex);

                blockTriangleList.Add(3 + triangleIndex);
                blockTriangleList.Add(2 + triangleIndex);
                blockTriangleList.Add(0 + triangleIndex);

                triangleIndex += 4;
            }

            if (neighbours[3] == BlockType.Air) // Up
            {
                blockVertexList.Add(vertsDrawPos[12]);
                blockVertexList.Add(vertsDrawPos[13]);
                blockVertexList.Add(vertsDrawPos[14]);
                blockVertexList.Add(vertsDrawPos[15]);

                blockUVList.Add(getUVs(12,asd));
                blockUVList.Add(getUVs(13,asd));
                blockUVList.Add(getUVs(14,asd));
                blockUVList.Add(getUVs(15,asd));


                blockTriangleList.Add(3 + triangleIndex);
                blockTriangleList.Add(1 + triangleIndex);
                blockTriangleList.Add(0 + triangleIndex);

                blockTriangleList.Add(0 + triangleIndex);
                blockTriangleList.Add(2 + triangleIndex);
                blockTriangleList.Add(3 + triangleIndex);

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
        */
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

    private Vector2[] getUVs(BlockType blockType)
    {

        //int asdf = 0;
        //Rect[] atlasUVs = WorldInitializer.atlasUVs;
        Vector2[] vertsUV = new Vector2[24];
        //float epsilon = 0.01f;
        //if (blockType != BlockType.Air)
        //{
        vertsUV[0] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);         // Front
        vertsUV[1] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);
        vertsUV[2] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
        vertsUV[3] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);

        vertsUV[4] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);    // Left
        vertsUV[5] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);
        vertsUV[6] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
        vertsUV[7] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);

        vertsUV[8] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);    // Right
        vertsUV[9] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);
        vertsUV[10] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
        vertsUV[11] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);


        vertsUV[12] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].xMin + 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].yMin + 0.01f);   // Top
        vertsUV[13] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].yMin + 0.01f);
        vertsUV[14] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].xMin + 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].yMax - 0.01f);
        vertsUV[15] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].yMax - 0.01f);


        vertsUV[16] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].yMin);   // Bottom
        vertsUV[17] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].xMax + 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].yMin);
        vertsUV[18] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].yMax);
        vertsUV[19] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].xMax + 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].yMax);

        vertsUV[20] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);   // Back
        vertsUV[21] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);
        vertsUV[22] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
        vertsUV[23] = new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
        // }
        return vertsUV;
    }

    private Vector2 getUVs(int idx, BlockType blockType)
    {
        switch(idx)
        {
        case 0:      
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);         // Front
        case 1:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);
        case 2:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
        case 3:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
        case 4:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);    // Left
        case 5:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);
        case 6:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
        case 7:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
        case 8:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);    // Right
        case 9:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);
        case 10:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
        case 11:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
        case 12:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].xMin + 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].yMin + 0.01f);   // Top
        case 13:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].yMin + 0.01f);
        case 14:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].xMin + 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].yMax - 0.01f);
        case 15:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Top].yMax - 0.01f);
        case 16:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].yMin);   // Bottom
        case 17:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].xMax + 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].yMin);
        case 18:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].yMax);
        case 19:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].xMax + 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Bottom].yMax);
        case 20:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);   // Back
        case 21:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMax);
        case 22:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMax - 0.01f, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
        case 23:
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);
        default:
            Debug.Log("Fail");
            return new Vector2(WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].xMin, WorldInitializer.atlasUVs[(int)blockType + (int)BlockSide.Side].yMin);


            }
    }

    private static Vector3[] genericVerts()
    {

        Vector3[] verts = new Vector3[24];
        // Front
        verts[0] = new Vector3(0, 1, 1);
        verts[1] = new Vector3(1, 1, 1);
        verts[2] = new Vector3(0, 0, 1);
        verts[3] = new Vector3(1, 0, 1);

        // Right
        verts[4] = new Vector3(1, 1, 1);
        verts[5] = new Vector3(1, 1, 0);
        verts[6] = new Vector3(1, 0, 1);
        verts[7] = new Vector3(1, 0, 0);

        // Left
        verts[8] = new Vector3(0, 1, 0);
        verts[9] = new Vector3(0, 1, 1);
        verts[10] = new Vector3(0, 0, 0);
        verts[11] = new Vector3(0, 0, 1);

        // Top
        verts[12] = new Vector3(0, 1, 0);
        verts[13] = new Vector3(1, 1, 0);
        verts[14] = new Vector3(0, 1, 1);
        verts[15] = new Vector3(1, 1, 1);

        // Bottom
        verts[16] = new Vector3(0, 0, 0);
        verts[17] = new Vector3(1, 0, 0);
        verts[18] = new Vector3(0, 0, 1);
        verts[19] = new Vector3(1, 0, 1);

        // Back
        verts[20] = new Vector3(1, 1, 0);
        verts[21] = new Vector3(0, 1, 0);
        verts[22] = new Vector3(1, 0, 0);
        verts[23] = new Vector3(0, 0, 0);

        return verts;
    }

    private Vector3 genericVerts(int idx)
    {
        switch(idx)
        {
            case 0:
                return new Vector3(0, 1, 1);
            case 1:
                return new Vector3(1, 1, 1);
            case 2:
                return new Vector3(0,0,1);
            case 3:
                return new Vector3(1,0,1);
            case 4:
                return new Vector3(1,1,1);
            case 5:
                return new Vector3(1,1,0);
            case 6:
                return new Vector3(1,0,1);
            case 7:
                return new Vector3(1,0,0);
            case 8:
                return new Vector3(0,1,0);
            case 9:
                return new Vector3(0,1,1);
            case 10:
                return new Vector3(0,0,0);
            case 11:
                return new Vector3(0, 0, 1);
            case 12:
                return new Vector3(0, 1, 0);
            case 13:
                return new Vector3(1, 1, 0);
            case 14:
                return new Vector3(0, 1, 1);
            case 15:
                return new Vector3(1, 1, 1);
            case 16:
                return new Vector3(0, 0, 0);
            case 17:
                return new Vector3(1, 0, 0);
            case 18:
                return new Vector3(0, 0, 1);
            case 19:
                return new Vector3(1, 0, 1);
            case 20:
                return new Vector3(1, 1, 0);
            case 21:
                return new Vector3(0, 1, 0);
            case 22:
                return new Vector3(1, 0, 0);
            case 23:
                return new Vector3(0, 0, 0);
            default:
                Debug.Log("Sage");
                return new Vector3(0, 0, 0);


        }
    }

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

        int3 key;


        //Xminus
        if (x - 1 >= 0)
        {
            x_minus = blocks[x - 1, y, z].blockType;
        }
        else
        {
            key = new int3(arrayBasedChunkPos.x - 1, arrayBasedChunkPos.y, arrayBasedChunkPos.z);
            if (WorldInitializer.chunkArrayContainsKey(key))
            {
                x_minus = WorldInitializer.chunkArray[key.x, key.y, key.z].blocks[CONST.chunkSize.x - 1, y, z].blockType;
            }
            else
            {
                x_minus = BlockType.Air;
            }
        }

        //Xplus
        if (x + 1 <= CONST.chunkSize.x - 1)
        {
            x_plus = blocks[x + 1, y, z].blockType;
        }
        else
        {
            key = new int3(arrayBasedChunkPos.x + 1, arrayBasedChunkPos.y, arrayBasedChunkPos.z);
            if (WorldInitializer.chunkArrayContainsKey(key))
            {
                x_plus = WorldInitializer.chunkArray[key.x, key.y, key.z].blocks[0, y, z].blockType;
            }
            else
            {
                x_plus = BlockType.Air;
            }
        }


        //Yminus
        if (y - 1 >= 0)
        {
            y_minus = blocks[x, y - 1, z].blockType;
        }
        else
        {
            key = new int3(arrayBasedChunkPos.x, arrayBasedChunkPos.y - 1, arrayBasedChunkPos.z);
            if (WorldInitializer.chunkArrayContainsKey(key))
            {
                y_minus = WorldInitializer.chunkArray[key.x, key.y, key.z].blocks[x, CONST.chunkSize.y - 1, z].blockType;
            }
            else
            {
                y_minus = BlockType.Air;
            }
        }

        //Yplus
        if (y + 1 <= CONST.chunkSize.y - 1)
        {
            y_plus = blocks[x, y + 1, z].blockType;
        }
        else
        {
            key = new int3(arrayBasedChunkPos.x, arrayBasedChunkPos.y + 1, arrayBasedChunkPos.z);
            if (WorldInitializer.chunkArrayContainsKey(key))
            {
                y_plus = WorldInitializer.chunkArray[key.x, key.y, key.z].blocks[x, 0, z].blockType;
            }
            else
            {
                y_plus = BlockType.Air;
            }
        }


        //Zminus
        if (z - 1 >= 0)
        {
            z_minus = blocks[x, y, z - 1].blockType;
        }
        else
        {
            key = new int3(arrayBasedChunkPos.x, arrayBasedChunkPos.y, arrayBasedChunkPos.z - 1);
            if (WorldInitializer.chunkArrayContainsKey(key))
            {
                z_minus = WorldInitializer.chunkArray[key.x, key.y, key.z].blocks[x, y, CONST.chunkSize.z - 1].blockType;
            }
            else
            {
                z_minus = BlockType.Air;
            }
        }

        //Zplus
        if (z + 1 <= CONST.chunkSize.z - 1)
        {
            z_plus = blocks[x, y, z + 1].blockType;
        }
        else
        {
            key = new int3(arrayBasedChunkPos.x, arrayBasedChunkPos.y, arrayBasedChunkPos.z + 1);
            if (WorldInitializer.chunkArrayContainsKey(key))
            {
                z_plus = WorldInitializer.chunkArray[key.x, key.y, key.z].blocks[x, y, 0].blockType;
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
			//chunk.AddComponent("MeshCollider");

            chunk.renderer.material.mainTexture = WorldInitializer.textureAtlas;

			//meshCollider = chunk.GetComponent<MeshCollider>();
			//meshCollider.sharedMesh = mesh;

            meshFilter = chunk.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            meshFilter.mesh.RecalculateNormals();
            //meshFilter.mesh.RecalculateBounds();
            firstRun = false;
        }
        else
        {
            //meshCollider.sharedMesh = null;
            Mesh mesh = meshFilter.mesh;

            mesh.Clear();
            mesh.vertices = verts;
            mesh.uv = UVs;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            //meshCollider.sharedMesh = mesh;
        }
    }
    //desudesudesudesu
}
