using UnityEngine;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

[CreateAssetMenu(fileName = "newExample", menuName = "Data/Example")]
public class Example : ScriptableObject
{
    const int MAXTEMP = 300;
    const int MINTEMP = 0;

    public Mesh Mesh;
    public Texture2D Gradient;
    FiniteElement[] elements;
    Matrix<float> GlobalStiffnessMatrix;
    float flux;
    float[] boundryConditions;

    public void Load(Loader loader)
    {
        Materiall material = CreateInstance<Materiall>();
        boundryConditions = new float[Mesh.vertexCount];
        material.ConductCoefficient = 5;
        elements = new FiniteElement[Mesh.triangles.Length / 3];
        for (int i = 0; i < elements.Length; i++)
        {
            Node[] nodes = new Node[]
            {
                new Node(Mesh.vertices[Mesh.triangles[3*i]], Mesh.triangles[3*i], loader.ObjectTemperature),
                new Node(Mesh.vertices[Mesh.triangles[3*i+1]], Mesh.triangles[3*i+1], loader.ObjectTemperature),
                new Node(Mesh.vertices[Mesh.triangles[3*i+2]], Mesh.triangles[3*i+2], loader.ObjectTemperature)
            };
            elements[i] = new FiniteElement(nodes, material);
        }

        GlobalStiffnessMatrix = Program.Instance.AssembleGlobalStiffnessMatrix(elements, Mesh.vertices.Length);
        flux = Program.Instance.CountFlux(material, loader.EnviromentTemperature);

        var edges = Edge.GetEdges(Mesh.triangles);
        var boundaries = Edge.GetBoudaries(edges);

        boundryConditions = Program.Instance.BoundryConditions(flux, boundaries, Mesh.vertexCount, Mesh);

        Color[] colors = new Color[Mesh.vertices.Length];

        foreach (var item in elements)
        {
            foreach (var node in item.nodes)
                colors[node.GlobalIndex] = GetTemperatureFromValue(node.Temperature);
        }

        Mesh.colors = colors;

    }



    private Color GetTemperatureFromValue(float temperature)
    {
        float val = temperature / 300.0f;
        return Gradient.GetPixel(Mathf.FloorToInt(val * Gradient.width), 1);
    }
}

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
        for (int i = 0; i < indices.Length; i+=3)
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

    public static List<Edge> GetBoudaries (List<Edge> edges)
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

    public float Length(Mesh mesh)
    {
        float length = 0;
        float a = mesh.vertices[Vertex1].x - mesh.vertices[Vertex2].x;
        float b = mesh.vertices[Vertex2].y - mesh.vertices[Vertex2].y;

        length = Mathf.Sqrt(a * a + b * b);

        return length;
    }

}

