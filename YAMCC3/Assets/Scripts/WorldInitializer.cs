using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WorldInitializer : MonoBehaviour
{
    public static Chunk[] chunkArray = new Chunk[CONST.worldChunkCount.x * CONST.worldChunkCount.y * CONST.worldChunkCount.z];

    public static Chunk getChunk(int x, int y, int z)
    {
        return chunkArray[getChunkIdx(x, y, z)];
    }
    public static void setChunk(int x, int y, int z, Chunk chunk)
    {
        chunkArray[getChunkIdx(x, y, z)] = chunk;
    }
    private static int getChunkIdx(int x, int y, int z)
    {
        return x + CONST.worldChunkCount.x * (y + CONST.worldChunkCount.y * z);
    }

    public static Texture2D textureAtlas;
    public Texture2D textureAtlasDes;
    public Rect[] atlasUVsDes;
    public static Rect[] atlasUVs;
	void Start ()
    {

        Profiler.BeginSample("Create lookup tables");
        //Debug.Log
        initializeAtlas();
        createUVLookUpTables();
        Profiler.EndSample();
	    //Create uhm... chunks!
        //When the chunks are created they automatically use the terrain generator to set up their block array with the correct blocktypes.
        DateTime startTime = DateTime.Now;
        Profiler.BeginSample("Create chunks");
        createChunks();
        Profiler.EndSample();
        DateTime end = DateTime.Now;
        TimeSpan generationTime = end - startTime;
        Debug.Log(generationTime);
        //Check neighbours etc and prepare blocks for drawing.
        //We cannot do this in the same loop as the initialization, because all chunks have to be finished and their blocks generated before
        //we start to check blocks neighbours because the neighbours could be in unintialized neighbourchunks. Cheers.
        startTime = DateTime.Now;

        Profiler.BeginSample("get chunk datda");
        getChunkDrawData();
        Profiler.EndSample();
        end = DateTime.Now;
        generationTime = end - startTime;
        Debug.Log(generationTime);
        //Get Drawdata and draw dat shit up.
        //Thread everything.
	}

    private void createUVLookUpTables()
    {
        Dictionary<int, Dictionary<BlockType, Vector2>> UVLookUp = new Dictionary<int, Dictionary<BlockType, Vector2>>();
        for (int i = 0; i < 24; i++)
        {
            var values = Enum.GetValues(typeof(BlockType));
            foreach(BlockType val in values)
            {
                Vector2 uv = Chunk.getUVsForPreGeneration(i, val);
                if (!UVLookUp.ContainsKey(i)) { UVLookUp[i] = new Dictionary<BlockType, Vector2>();}
                UVLookUp[i][val] = uv;
            }
        }

        Chunk.UVLookUp = UVLookUp;
    }

    private void initializeAtlas()
    {
        Texture2D[] cubeTextures = new Texture2D[9];
        /* 
        cubeTextures[1] = Resources.Load("Textures/hiDefGrass") as Texture2D;

        cubeTextures[2] = Resources.Load("Textures/sidegrass") as Texture2D;

        cubeTextures[3] = Resources.Load("Textures/hiDefDirt") as Texture2D;
        cubeTextures[4] = Resources.Load("Textures/hiDefDirt") as Texture2D;
        cubeTextures[5] = Resources.Load("Textures/hiDefDirt") as Texture2D;

        cubeTextures[6] = Resources.Load("Textures/hiDefStone") as Texture2D;
        cubeTextures[7] = Resources.Load("Textures/hiDefStone") as Texture2D;
        cubeTextures[8] = Resources.Load("Textures/hiDefStone") as Texture2D;
        */
        cubeTextures[1] = Resources.Load("Textures/dokuGrasss") as Texture2D;

        cubeTextures[2] = Resources.Load("Textures/dokuGrassSides") as Texture2D;

        cubeTextures[3] = Resources.Load("Textures/dokuDirts") as Texture2D;
        cubeTextures[4] = Resources.Load("Textures/dokuDirts") as Texture2D;
        cubeTextures[5] = Resources.Load("Textures/dokuDirts") as Texture2D;

        cubeTextures[6] = Resources.Load("Textures/dokuStones") as Texture2D;
        cubeTextures[7] = Resources.Load("Textures/dokuStones") as Texture2D;
        cubeTextures[8] = Resources.Load("Textures/dokuStones") as Texture2D;
        textureAtlas = new Texture2D(4096, 4096);
        atlasUVs = textureAtlas.PackTextures(cubeTextures, 0);
        atlasUVsDes = atlasUVs;
        textureAtlas.Apply();
        textureAtlasDes = textureAtlas;
    }

    private void createChunks()
    {
        for (int x = 0; x < CONST.worldChunkCount.x; x++)
        {
            for (int y = 0; y < CONST.worldChunkCount.y; y++)
            {
                for (int z = 0; z < CONST.worldChunkCount.z; z++)
                {
                    setChunk(x, y, z, new Chunk(new int3(x, y, z)));
                }
            }
        }
    }

    private void getChunkDrawData()
    {
        for (int x = 0; x < CONST.worldChunkCount.x; x++)
        {
            for (int y = 0; y < CONST.worldChunkCount.y; y++)
            {
                for (int z = 0; z < CONST.worldChunkCount.z; z++)
                {
                    if (getChunk(x, y, z).anyNonAir)
                    {
                        getChunk(x, y, z).AddChunkDrawdataToMeshQueue();
                    }
                }
            }
        }
    }

    public static bool chunkArrayContainsKey(int x, int y, int z)
    {
        if (x >= CONST.worldChunkCount.x || y >= CONST.worldChunkCount.y || z >= CONST.worldChunkCount.z)
        {
            return false;
        }
        if (x < 0 || y < 0 || z < 0)
        {
            return false;
        }
        return true;
    }
}
