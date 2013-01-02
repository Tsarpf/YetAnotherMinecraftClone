using UnityEngine;
using System.Collections;

public class TestCock : MonoBehaviour {

    Material material;
    void Start()
    {
        material = new Material(Shader.Find("Particles/Additive"));
        //createStartUpCircles();


        Vector3[] innerVerts = getCircleVerts(10);
        Vector3[] outerVerts = getCircleVerts(20, 62);

        for(int i = 0; i < 62; i++)
        {

        }
        //GetAndDrawVerticePoints(20);
    }
    private Vector3[] getCircleVerts(int radius)
    {
        float wholeCircleRadians = Mathf.PI * 2;
        int pointCount = (int)(2 * Mathf.PI * radius);
        float step = wholeCircleRadians / (pointCount - 1);

        Vector3[] verts = new Vector3[pointCount];
        int i = 0;
        while (i < pointCount)
        {
            Vector3 pos = new Vector3(radius * Mathf.Cos(step * i), radius * Mathf.Sin(step * i));
            verts[i] = pos;
            i++;
        }
        return verts;
    }

    private Vector3[] getCircleVerts(int radius, int pointCount)
    {
        float wholeCircleRadians = Mathf.PI * 2;
        //int pointCount = (int)(2 * Mathf.PI * radius);
        float step = wholeCircleRadians / (pointCount - 1);

        Vector3[] verts = new Vector3[pointCount];
        int i = 0;
        while (i < pointCount)
        {
            Vector3 pos = new Vector3(radius * Mathf.Cos(step * i), radius * Mathf.Sin(step * i));
            verts[i] = pos;
            i++;
        }
        return verts;
    }

    void GetAndDrawVerticePoints(int radius)
    {
        float wholeCircleRadians = Mathf.PI * 2;

        GameObject Gobject = new GameObject();

        ParticleEmitter pEmitter = gameObject.particleEmitter;
      
        int pointCount = (int)(2 * Mathf.PI * radius);

        pEmitter.Emit(pointCount);


        var particles = pEmitter.particles;
        float step = wholeCircleRadians / (pointCount - 1);

        int i = 0;
        while (i < pointCount)
        {
            Vector3 pos = new Vector3(radius * Mathf.Cos(step * i), radius * Mathf.Sin(step * i));
            particles[i].position = pos;
            i++;
        }
        pEmitter.particles = particles;

        //pEmitter.ClearParticles();
    }

    void createStartUpCircles()
    {
        int increaser = 10;
        int ringCount = 10;
        for (int i = 1; i < ringCount + 1; i++)
        {
            createCircleXY(increaser, 30);
            createCircleYZ(increaser, 30);
            increaser += 10;
        }
    }
    void createCircleXY(float radius, int pointCount)
    {
        Color c1 = Color.yellow;
        Color c2 = Color.red;
        float wholeCircleRadians = Mathf.PI * 2;
        float step = wholeCircleRadians / (pointCount - 1);
        GameObject Gobject = new GameObject();
        LineRenderer lineRenderer = Gobject.AddComponent<LineRenderer>();
        lineRenderer.material = material;
        lineRenderer.SetColors(c1, c2);
        lineRenderer.SetWidth(1F, 1F);
        lineRenderer.SetVertexCount(pointCount);

        int i = 0;
        while (i < pointCount)
        {
            Vector3 pos = new Vector3(radius * Mathf.Cos(step * i), radius * Mathf.Sin(step * i));
            lineRenderer.SetPosition(i, pos);
            i++;
        }
    }

    void createCircleYZ(float radius, int pointCount)
    {
        Color c1 = Color.yellow;
        Color c2 = Color.red;
        float wholeCircleRadians = Mathf.PI * 2;
        float step = 0;

        step = wholeCircleRadians / (pointCount - 1);
        GameObject Gobject = new GameObject();
        LineRenderer lineRenderer = Gobject.AddComponent<LineRenderer>();
        lineRenderer.material = material;
        lineRenderer.SetColors(c1, c2);
        lineRenderer.SetWidth(1F, 1F);
        lineRenderer.SetVertexCount(pointCount);

        int i = 0;
        while (i < pointCount)
        {
            Vector3 pos = new Vector3(0, radius * Mathf.Cos(step * i), radius * Mathf.Sin(step * i));
            lineRenderer.SetPosition(i, pos);
            i++;
        }
    }
}
