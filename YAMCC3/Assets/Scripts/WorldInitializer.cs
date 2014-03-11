using UnityEngine;
using System.Collections;

public class WorldInitializer : MonoBehaviour
{
    public static Chunk[, ,] chunkArray = new Chunk[CONST.worldChunkCount.x, CONST.worldChunkCount.y, CONST.worldChunkCount.z];
    public static Texture2D textureAtlas;
    public static Rect[] atlasUVs;
	void Start ()
    {
	    //Create uhm... chunks!
        //When the chunks are created they automatically use the terrain generator to set up their block array with the correct blocktypes.
        createChunks();

        initializeAtlas();
        //Check neighbours etc and prepare blocks for drawing.
        //We cannot do this in the same loop as the initialization, because all chunks have to be finished and their blocks generated before
        //we start to check blocks neighbours because the neighbours could be in unintialized neighbourchunks. Cheers.
        getChunkDrawData();
        //Get Drawdata and draw dat shit up.
        //Thread everything.

        //GameObject.Find("Hero").transform.position = new Vector3(
        //    (CONST.chunkSize.x * CONST.worldChunkCount.x) / 2,
        //    (CONST.chunkSize.y * CONST.worldChunkCount.y) - 50,
        //    (CONST.chunkSize.z * CONST.worldChunkCount.z) / 2);

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
        cubeTextures[1] = Resources.Load("Textures/dokuGrass") as Texture2D;

        cubeTextures[2] = Resources.Load("Textures/dokuGrassSide") as Texture2D;

        cubeTextures[3] = Resources.Load("Textures/dokuDirt") as Texture2D;
        cubeTextures[4] = Resources.Load("Textures/dokuDirt") as Texture2D;
        cubeTextures[5] = Resources.Load("Textures/dokuDirt") as Texture2D;

        cubeTextures[6] = Resources.Load("Textures/dokuStone") as Texture2D;
        cubeTextures[7] = Resources.Load("Textures/dokuStone") as Texture2D;
        cubeTextures[8] = Resources.Load("Textures/dokuStone") as Texture2D;
        textureAtlas = new Texture2D(4096, 4096);
        atlasUVs = textureAtlas.PackTextures(cubeTextures, 0);
        textureAtlas.Apply();
    }

    private void createChunks()
    {
        for (int x = 0; x < CONST.worldChunkCount.x; x++)
        {
            for (int y = 0; y < CONST.worldChunkCount.y; y++)
            {
                for (int z = 0; z < CONST.worldChunkCount.z; z++)
                {
                    chunkArray[x, y, z] = new Chunk(new int3(x, y, z));
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
                    chunkArray[x, y, z].AddChunkDrawdataToMeshQueue();
                }
            }
        }
    }

    public static bool chunkArrayContainsKey(int3 key)
    {
        if (key.x >= CONST.worldChunkCount.x || key.y >= CONST.worldChunkCount.y || key.z >= CONST.worldChunkCount.z)
        {
            return false;
        }
        if (key.x < 0 || key.y < 0 || key.z < 0)
        {
            return false;
        }
        return true;
    }






    void Update()
    {
	
	}
}
