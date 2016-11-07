using System;
using UnityEngine;

public class Node
{
    public Vector2 Position;

    public Node(Vector2 position)
    {
        Position = position;
    }


    //Shape function
    public float[] Coefficients = new float[3];
    public float Temperature;
}
