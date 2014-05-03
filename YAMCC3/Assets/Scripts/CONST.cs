using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class CONST
{
    public static int3 worldChunkCount = new int3(6, 4, 6);
    public static int3 chunkSize = new int3(32, 32, 32);
    public static int worldDepthBlocks = worldChunkCount.y * chunkSize.y;
}
