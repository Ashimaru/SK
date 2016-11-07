using System;
using UnityEngine;

public class Node
{
    public Vector2 Position;
    public float Temperature;


    public Node(Vector2 position) : this(position, 20.0f)
    {
        Position = position;
    }

    public Node(Vector2 position, float temperature)
    {
        Position = position;
        Temperature = temperature;
    }

}
