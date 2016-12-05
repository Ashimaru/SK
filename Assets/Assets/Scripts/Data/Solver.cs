using System.IO;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;



public class Solver
{
    FiniteElement[] elements;
    Matrix<double> GlobalStiffnessMatrix;
    float flux;
    Vector<double> boundryConditions;
    Vector<double> temperatures;

    public void Solve(Loader loader)
    {
        Materiall material = loader.activeMaterial;
        Mesh mesh = loader.activeExample.Mesh;
        elements = new FiniteElement[mesh.triangles.Length / 3];
        var edges = Edge.GetEdges(mesh.triangles);
        var boundaries = Edge.GetBoudaries(edges);

        for (int i = 0; i < elements.Length; i++)
        {
            Node[] nodes = new Node[]
            {
                new Node(mesh.vertices[mesh.triangles[3*i]], mesh.triangles[3*i], loader.ObjectTemperature),
                new Node(mesh.vertices[mesh.triangles[3*i+1]], mesh.triangles[3*i+1], loader.ObjectTemperature),
                new Node(mesh.vertices[mesh.triangles[3*i+2]], mesh.triangles[3*i+2], loader.ObjectTemperature)
            };
            elements[i] = new FiniteElement(nodes, material);
        }

//        flux = Program.Instance.CountFlux(material, loader.EnviromentTemperature);

        GlobalStiffnessMatrix = Program.Instance.AssembleGlobalStiffnessMatrix(elements, mesh.vertices.Length);

        boundryConditions = Program.Instance.BoundryConditionsTemp(loader.EnviromentTemperature, loader.ObjectTemperature, boundaries, mesh.vertexCount, mesh);

        temperatures = Program.Instance.CountSolution(GlobalStiffnessMatrix, boundryConditions);

        Color[] colors = new Color[mesh.vertices.Length];

        for (int i = 0; i < mesh.vertexCount; i++)
            colors[i] = GetTemperatureFromValue((float)temperatures[i], loader.Gradient);

        //Mesh.colors = colors;

    }

    private Color GetTemperatureFromValue(float temperature, Texture2D gradient)
    {
        float val = temperature / 300.0f;
        return gradient.GetPixel(Mathf.FloorToInt(val * gradient.width), 1);
    }
}

