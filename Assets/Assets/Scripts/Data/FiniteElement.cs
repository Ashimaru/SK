using UnityEngine;
using System.Collections;

public class FiniteElement
{
    private float[,] ShapeFunctionsMatrix;
    private float[,] B;
    private Node[] nodes;
    private float surface;
    private Vector2 flux;

    public FiniteElement(Node[] nodes, Materiall material)
    {
        this.nodes = nodes;

        ShapeFunctionsMatrix = GenerateShapeFunctionsCoefficientsMatrix();

        surface = CountSurface();

        B = CountB();

        flux = CountFlux(material);
    }

    private float[,] GenerateShapeFunctionsCoefficientsMatrix()
    {
        float[,] result = new float[3, 3];
        int a;
        int b;

        for (int i = 0; i < 3; i++)
        {
            a = (i + 1) % nodes.Length;
            b = (i + 2) % nodes.Length;

            result[i, 0] = nodes[a].Position.x * nodes[b].Position.y - nodes[b].Position.x * nodes[a].Position.y;
            result[i, 1] = nodes[a].Position.y - nodes[b].Position.y;
            result[i, 2] = nodes[b].Position.x - nodes[a].Position.x;
        }

        return result;
    }

    private float CountSurface()
    {
        float twiceA = 0;
        int a;
        int b;

        for (int i = 0; i < nodes.Length; i++)
        {
            a = i % nodes.Length;
            b = (i + 1) % nodes.Length;
            twiceA += nodes[a].Position.x * nodes[b].Position.y - nodes[a].Position.y * nodes[b].Position.x;
        }

        return twiceA / 2;
    }

    private float[,] CountB()
    {
        float[,] result = new float[2, 3];

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                result[i, j] = ShapeFunctionsMatrix[j, i + 1] / (2 * surface);
            }
        }

        return result;
    }

    private Vector2 CountFlux(Materiall material)
    {
        Vector2 result = new Vector2(material.ConductCoefficient, material.ConductCoefficient);

        for (int i = 0; i < 3; i++)
        {
            result.x += B[0, i] * nodes[i].Temperature;
            result.y += B[1, i] * nodes[i].Temperature;
        }

        return result;
    }




    //public float ShapeFuncForANodeAtPoit(Data data)
    //{
    //    return (data.Coefficients[0] + data.Coefficients[1] * data.Node.Position.x + data.Coefficients[2] * data.Node.Position.y) / (2 * surface);
    //

}
