using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class ChunkDrawDataArray
{
    public Vector3[] vertexList;
    public int[] triangleList;
    public Vector2[] UVList;
    public Chunk chunk;

    public ChunkDrawDataArray(Vector3[] vertexlist, int[] trianglelist, Vector2[] uvlist, Chunk chnk)
    {
        vertexList = vertexlist;
        triangleList = trianglelist;
        UVList = uvlist;
        chunk = chnk;
    }
}

