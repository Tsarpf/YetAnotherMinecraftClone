using System;
using UnityEngine;

public static class TerraGenV2
{
    static int chunkSizeX = CONST.chunkSize.x;
    static int chunkSizeY = CONST.chunkSize.y;
    static int chunkSizeZ = CONST.chunkSize.z;


    public static void CreateLandscape(Chunk chunk)
    {
        int chunkX = chunk.chunkPosWorld.x;
        int chunkZ = chunk.chunkPosWorld.z;
        Block[, ,] blocks = chunk.blocks;

        ChunkNoise noise = new ChunkNoise(seed: 54321);

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
                for (int y = 0; y < chunkSizeY; y++)
                {

                    int blockWorldY = (int)chunk.blockOffset.y + y;
                    if (blockWorldY < height)
                    {
                        blocks[localX, y, localZ].blockType = BlockType.Stone;
                    }
                }
                
                // Create mountains
                int worldX = (int)chunk.blockOffset.x + localX;
                int worldZ = (int)chunk.blockOffset.z + localZ;

                for (int y = 0; y < chunkSizeY; y++)
                {
                    int blockWorldY = (int)chunk.blockOffset.y + y;

                    //if (blockWorldY >= height && blockWorldY < WorldGameObject.worldDepthBlocks)
                    if (blockWorldY >= height)
                    {
                        float noiseValue3D = noise.GetValue3D(worldX, blockWorldY, worldZ, octaves: 6, startFrequency: .05f, startAmplitude: 1);
                        if (noiseValue3D < -0.3)
                        {
                            blocks[localX, y, localZ].blockType = BlockType.Dirt;
                        }
                        else
                        {
                            blocks[localX, y, localZ].blockType = BlockType.Air;
                        }
                    }
                }
                 
            }
        }

        chunk.blocks = blocks;
    }

}