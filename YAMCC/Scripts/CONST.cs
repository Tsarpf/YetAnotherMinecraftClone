using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class CONST
{
    public static int3 worldChunkCount = new int3(8, 4, 8);
    public static int3 chunkSize = new int3(16, 25, 16);
    public static int worldDepthBlocks = worldChunkCount.y * chunkSize.y;
}
