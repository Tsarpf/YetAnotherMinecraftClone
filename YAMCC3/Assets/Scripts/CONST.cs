using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class CONST
{
    //public static int3 worldChunkCount = new int3(32, 16, 32);
    //public static int3 chunkSize = new int3(16, 16, 16);
    public static int3 worldChunkCount = new int3(32, 16, 32);
    public static int3 chunkSize = new int3(16, 16, 16);
    //public static int3 worldChunkCount = new int3(6, 6, 6);
    //public static int3 chunkSize = new int3(8, 8, 8);
    public static int worldDepthBlocks = worldChunkCount.y * chunkSize.y;
    public static int framesBetweenMeshes = 1;
    public static int worldSeed = 999;
}
