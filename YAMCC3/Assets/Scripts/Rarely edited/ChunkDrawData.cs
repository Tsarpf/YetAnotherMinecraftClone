using System.Collections.Generic;
using UnityEngine;


public class ChunkDrawData
{
    public List<Vector3> vertexList;
    public List<int> triangleList;
    public List<Vector2> UVList;
    public Chunk chunk;

    public ChunkDrawData(List<Vector3> vertexlist, List<int> trianglelist, List<Vector2> uvlist, Chunk chnk)
    {
        vertexList = vertexlist;
        triangleList = trianglelist;
        UVList = uvlist;
        chunk = chnk;
    }
}

