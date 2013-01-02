using System;
using System.Collections.Generic;
using UnityEngine;


public struct BlockDrawInfo
{
    public List<Vector3> blockVertexList;
    public List<Vector2> blockUVList;
    public List<int> blockTriangleList;

    public int triangleCount;

    public BlockDrawInfo(List<Vector3> blockvertexlist, List<Vector2> blockuvlist, List<int> blocktrianglelist, int trianglecount)
    {
        blockVertexList = blockvertexlist;
        blockUVList = blockuvlist;
        blockTriangleList = blocktrianglelist;

        triangleCount = trianglecount;
    }
}
