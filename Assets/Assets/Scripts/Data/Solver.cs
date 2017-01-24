using System.IO;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;



public class Solver
{
    FiniteElement[] elements;
    Matrix<double> GlobalStiffnessMatrix;
    float flux;
    Vector<double> temps;
    Vector<double> temperatures;

    public void Solve(Loader loader)
    {
        Materiall material = loader.activeMaterial;
        Mesh mesh = loader.activeExample.Mesh;
        elements = new FiniteElement[mesh.triangles.Length / 3];
        var edges = Edge.GetEdges(mesh.triangles);
        var boundaries = Edge.GetBoudaries(edges);
        var boundaryNodes = Edge.GetBoundaryNodesIndexes(boundaries);

        boundaryNodes.Sort();

        Color[] colors = new Color[mesh.vertices.Length];

        mesh.colors = colors;

        Test();

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

        for(int i = 0; i < elements.Length; i++)
        {
            string s = elements[i].LocalStiffnessMatrix.ToMatrixString(3, 3);
            File.WriteAllText(@"D:\LocalStifnessMatrix" + i + ".txt", s);
        }

        GlobalStiffnessMatrix = Program.Instance.AssembleGlobalStiffnessMatrix(elements, mesh.vertices.Length);

        double[,] A = new double[,] {   { -3.7789, 5.78805, -7.62743, 0, 0, 0, 0, 0, -9.969672, 6.18115, 0.0417701, 0, 0, 0, 0, 0 },
                                        { 5.78805, -41.87343, 1.39282, 5.92057, 12.4384, 0, 0, 0, 9.59497, 0, -5.73882, 0, 0, 0, 0, 0 },
                                        { -7.62743, 1.39282, -0.169700000000001, 17.73411, 10.5128, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                        { 0, 17.25037, 6.40431, -17.05938, -0.5406453, -6.0546, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                        { 0, 22.9512, 0, -0.5406453, -33.8782, -3.6762, 31.95365, 0, 0, 0, -9.4744, -7.33534, 0, 0, 0, 0 },
                                        { 0, 0, 0, -6.0546, -3.6762, 18.3219, 10.89105, -19.4821, 0, 0, 0, 0, 0, 0, 0, 0 },
                                        { 0, 0, 0, 0, 31.95365, 10.89105, -52.17642, -28.9698, 0, 0, 0, 38.3015, 0, 0, 0, 0 },
                                        { 0, 0, 0, 0, 0, -19.4821, -28.9698, 48.4519, 0, 0, 0, 0, 0, 0, 0, 0 },
                                        { -9.969672, 9.59497, 0, 0, 0, 0, 0, 0, 4.02099, 0.340399999999999, 5.2473, 0, -9.98461, 0, 0.750631, 0 },
                                        { 6.18115, 0, 0, 0, 0, 0, 0, 0, 0.340399999999999, -28.365, 0, 0, -8.71305, 30.5565, 0, 0 },
                                        { 0.0417701, -5.73882, 0, 0, -9.4744, 0, 0, 0, 5.2473, 0, 16.86528, -6.64443, 0, 0, 2.57429, -2.870879 },
                                        { 0, 0, 0, 0, -7.33534, 0, 38.3015, 0, 0, 0, -6.64443, -42.53872, 0, 0, 0, 18.217 },
                                        { 0, 0, 0, 0, 0, 0, 0, 0, -9.98461, -8.71305, 0, 0, 21.95786, 9.54011, -12.8003, 0 },
                                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 30.5565, 0, 0, 9.54011, -40.0966, 0, 0 },
                                        { 0, 0, 0, 0, 0, 0, 0, 0, 0.750631, 0, 2.57429, 0, -12.8003, 0, 24.73181, -15.2564 },
                                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -2.870879, 18.217, 0, 0, -15.2564, -0.089700000000001 } };

       // GlobalStiffnessMatrix = Matrix<double>.Build.DenseOfArray(A);

        temps = Program.Instance.BoundryConditionsTemp(loader.EnviromentTemperature, boundaries, mesh.vertexCount, mesh);

        for (int i = 0; i < boundaryNodes.Count / 4; i++)
        {
            temps[boundaryNodes[i]] = 10;
        }

        string str = GlobalStiffnessMatrix.ToMatrixString(mesh.vertexCount, mesh.vertexCount);

        File.WriteAllText(@"D:\matrix.txt", str);

        str = temps.ToVectorString();

        File.WriteAllText(@"D:\vector.txt", str);

        var rightSide = Program.Instance.SimplifyEquation(ref GlobalStiffnessMatrix, temps, boundaryNodes);

        str = GlobalStiffnessMatrix.ToMatrixString(mesh.vertexCount, mesh.vertexCount);

        File.WriteAllText(@"D:\matrix2.txt", str);

        str = rightSide.ToVectorString();

        File.WriteAllText(@"D:\vector2.txt", str);

        temperatures = GlobalStiffnessMatrix.Solve(rightSide);

        var temperatures2 = Program.Instance.CountSolution(GlobalStiffnessMatrix, rightSide);

        double[,] d = { { 2, 1, 3 },
            { 2, 6, 8 },
            { 6, 8, 18 } };

        Matrix<double> matrix = Matrix<double>.Build.DenseOfArray(d);

        double[] v = { 1, 3, 5 };

        Vector<double> vect = Vector<double>.Build.DenseOfArray(v);

        var result = matrix.Solve(vect);

        for (int i = 0; i < mesh.vertexCount; i++)
            colors[i] = GetTemperatureFromValue((float)temperatures[i], loader.Gradient);

        mesh.colors = colors;

    }

    private Color GetTemperatureFromValue(float temperature, Texture2D gradient)
    {
        float val = temperature / 300.0f;
        return gradient.GetPixel(Mathf.FloorToInt(val * gradient.width), 1);
    }

    void Test()
    {
        Node[] nodes = new Node[]
        {
            new Node(new Vector2(1,0), 0, 0),
            new Node(new Vector2(3,2), 1, 0),
            new Node(new Vector2(-1,1), 2, 0)
        };

        Materiall testMaterial = new Materiall();
        testMaterial.ConductCoefficientX = 4.7f;
        testMaterial.ConductCoefficientY = 5.1f;

        FiniteElement fe = new FiniteElement(nodes, testMaterial);
    }


}