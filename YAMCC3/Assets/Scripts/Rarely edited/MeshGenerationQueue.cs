using System.Collections;

public static class MeshGenerationQueue
{
    public static Queue q = new Queue(32);

    public static void AddToQueue(ChunkDrawDataArray chunkDrawData)
    {
        //lock (q)
        //{
            q.Enqueue(chunkDrawData);
        //}
    }

    public static ChunkDrawDataArray GetFromQueue()
    {
        ChunkDrawDataArray chunkdrawdata;

        //lock (q)
        //{
            if (q.Count > 0)
            {
                chunkdrawdata = (ChunkDrawDataArray)q.Dequeue();
            }
            else
            {
                chunkdrawdata = null;
            }
        //}

        return chunkdrawdata;
    }
}
