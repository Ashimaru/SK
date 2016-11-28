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

        GlobalStiffnessMatrix = Program.Instance.AssembleGlobalStiffnessMatrix(elements, mesh.vertices.Length);
        flux = Program.Instance.CountFlux(material, loader.EnviromentTemperature);

        boundryConditions = Program.Instance.BoundryConditionsTemp(loader.EnviromentTemperature, loader.ObjectTemperature, boundaries, mesh.vertexCount, mesh);

        //double[,] vals = { { 9.1154375, -3.8130625, -5.302375 }, { -3.8130625, 2.6511875, 1.161875 }, { -5.302375, 1.161875, 4.1405 } };
        //double[] res = { 120.0, 30.0, 120.0 };
        //double[,] vals = { { 3, 2, -1 }, { 2, -2, 4 }, {-1, 0.5, -1 } };
        //double[] res = { 1, -2, 0 };
        //GlobalStiffnessMatrix = DenseMatrix.OfArray(vals);
        //boundryConditions = DenseVector.OfArray(res);
        temperatures = GlobalStiffnessMatrix.LU().Solve(boundryConditions);
        var det = GlobalStiffnessMatrix.Rank();
        string str = GlobalStiffnessMatrix.LU().ToString();
        //File.WriteAllText(@"C:\Users\Mateusz\Desktop\text.txt", str);

        Color[] colors = new Color[mesh.vertices.Length];

        //foreach (var item in elements)
        //{
        //    foreach (var node in item.nodes)
        //        colors[node.GlobalIndex] = GetTemperatureFromValue(node.Temperature);
        //}

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

