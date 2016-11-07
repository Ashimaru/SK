using System;
using UnityEngine;

public class Node
{
    public Vector2 Position;
    public float Temperature;
    MathNet.Numerics.LinearAlgebra.Matrix<float> matr;

    public Node(Vector2 position) : this(position, 20.0f)
    {
        matr = MathNet.Numerics.LinearAlgebra.Matrix<float>.Build.Diagonal(2, 2);
        Position = position;
    }

    public Node(Vector2 position, float temperature)
    {
        Position = position;
        Temperature = temperature;
    }

}
