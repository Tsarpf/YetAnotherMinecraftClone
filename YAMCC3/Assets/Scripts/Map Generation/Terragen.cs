using System;
using UnityEngine;

public static class TerraGen
{
    static int chunkSizeX = CONST.chunkSize.x;
    static int chunkSizeY = CONST.chunkSize.y;
    static int chunkSizeZ = CONST.chunkSize.z;


    public static void CreateLandscape(Chunk chunk)
    {
        int chunkX = chunk.chunkPosWorld.x;
        int chunkZ = chunk.chunkPosWorld.z;
        //Block[, ,] blocks = chunk.blocks;

        ChunkNoise noise = new ChunkNoise(seed: CONST.worldSeed);

        // Calculate heightmap
        float[,] heightmap = new float[chunkSizeX, chunkSizeZ];
        noise.FillMap2D(heightmap, chunkX, chunkZ, octaves: 5, startFrequency: .03f, startAmplitude: 5);

        // Fill chunk with blocks
        for (int localX = 0; localX < CONST.chunkSize.x; localX++)
        {
            for (int localZ = 0; localZ < CONST.chunkSize.z; localZ++)
            {
                
                // Create ground
                int height = Mathf.RoundToInt((CONST.worldDepthBlocks / 4) + heightmap[localX, localZ]);
                /*
                for (int y = 0; y < chunkSizeY; y++)
                {

                    int blockWorldY = (int)chunk.blockOffset.y + y;

                }
                */
                
                // Create mountains
                int worldX = (int)chunk.blockOffset.x + localX;
                int worldZ = (int)chunk.blockOffset.z + localZ;

                for (int y = 0; y < chunkSizeY; y++)
                {
                    int blockWorldY = (int)chunk.blockOffset.y + y;

                    if (blockWorldY < height - 3)
                    {
                        //blocks[localX, y, localZ].blockType = BlockType.Air;
                        chunk.setBlock(localX, y, localZ, BlockType.Air);
                    }
                    else if (blockWorldY < height)
                    {
                        //blocks[localX, y, localZ].blockType = BlockType.Stone;
                        chunk.setBlock(localX, y, localZ, BlockType.Stone);
                    }
                    //if (blockWorldY >= height && blockWorldY < WorldGameObject.worldDepthBlocks)
                    else if (blockWorldY >= height)
                    {
                        float noiseValue3D = noise.GetValue3D(worldX, blockWorldY, worldZ, octaves: 6, startFrequency: .05f, startAmplitude: 1);
                        if (noiseValue3D < -0.3)
                        {
                            //blocks[localX, y, localZ].blockType = BlockType.Dirt;
                            chunk.setBlock(localX, y, localZ, BlockType.Dirt);

                        }
                        else
                        {
                            chunk.setBlock(localX, y, localZ, BlockType.Air);

                            //blocks[localX, y, localZ].blockType = BlockType.Air;
                        }
                    }
                }
                 
            }
        }

        //chunk.blocks = blocks;
        
        for(int x = 0; x < chunkSizeX; x++)
        {
            for (int y = 0; y < chunkSizeY; y++)
            {
                for (int z = 0; z < chunkSizeZ; z++)
                {
                    if(chunk.getBlock(x,y,z) != BlockType.Air)
                    {
                        chunk.anyNonAir = true;
                    }
                }
            }
        }
        
    }

}