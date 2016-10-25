using UnityEngine;
using System.Collections;

public class FiniteElement
{

    private Data[] data;
    private float surface;

    public FiniteElement(Node[] nodes)
    {
        data = new Data[3];
        for (int i = 0; i < data.Length; i++)
        {
            data[i].Node = nodes[i];
            data[i].Coefficients = GenerateShapeFunctionCoefficients(nodes, i);
        }

        surface = CountSurface(nodes);
    }

    private float[] GenerateShapeFunctionCoefficients(Node[] nodes, int number)
    {
        float[] result = { 0, 0, 0 };
        int a = (number + 1) % nodes.Length;
        int b = (number + 2) % nodes.Length;

        result[0] = nodes[a].Position.x * nodes[b].Position.y - nodes[b].Position.x * nodes[a].Position.y;
        result[1] = nodes[a].Position.y - nodes[b].Position.y;
        result[2] = nodes[b].Position.x - nodes[a].Position.x;

        return result;
    }

    private float CountSurface(Node[] nodes)
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

    public float ShapeFuncForANodeAtPoit(Data data)
    {
        return (data.Coefficients[0] + data.Coefficients[1] * data.Node.Position.x + data.Coefficients[2] * data.Node.Position.y) / (2 * surface);
    }

    struct Data
    {
        public Node Node { get; set; }
        public float[] Coefficients { get; set; }
    }

}
