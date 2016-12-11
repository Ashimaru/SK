using UnityEngine;
using System.Collections.Generic;
public class Edge
{
    public int Vertex1 { get; private set; }
    public int Vertex2 { get; private set; }
    public int TriangleNumber { get; private set; }

    public Edge(int v1, int v2, int triangle)
    {
        Vertex1 = v1;
        Vertex2 = v2;
        TriangleNumber = triangle;
    }

    public static List<Edge> GetEdges(int[] indices)
    {
        List<Edge> result = new List<Edge>();
        for (int i = 0; i < indices.Length; i += 3)
        {
            int v1 = indices[i];
            int v2 = indices[i + 1];
            int v3 = indices[i + 2];
            result.Add(new Edge(v1, v2, i));
            result.Add(new Edge(v2, v3, i));
            result.Add(new Edge(v3, v1, i));
        }
        return result;
    }

    public static List<Edge> GetBoudaries(List<Edge> edges)
    {
        List<Edge> result = new List<Edge>(edges);
        for (int i = result.Count - 1; i > 0; i--)
        {
            for (int n = i - 1; n >= 0; n--)
            {
                if (result[i].Vertex1 == result[n].Vertex2 && result[i].Vertex2 == result[n].Vertex1)
                {
                    // shared edge so remove both
                    result.RemoveAt(i);
                    result.RemoveAt(n);
                    i--;
                    break;
                }
            }
        }
        return result;
    }

    public static List<int> GetBoundaryNodesIndexes(List<Edge> boundaryEdges)
    {
        List<int> result = new List<int>();

        for (int i = 0; i < boundaryEdges.Count; i++)
        {
            if (!result.Contains(boundaryEdges[i].Vertex1))
                result.Add(boundaryEdges[i].Vertex1);
            if (!result.Contains(boundaryEdges[i].Vertex2))
                result.Add(boundaryEdges[i].Vertex2);
        }

        return result;
    }

    public float Length(Mesh mesh)
    {
        float length = 0;
        float a = mesh.vertices[Vertex1].x - mesh.vertices[Vertex2].x;
        float b = mesh.vertices[Vertex2].y - mesh.vertices[Vertex2].y;

        length = Mathf.Sqrt(a * a + b * b);

        return length;
    }

}
