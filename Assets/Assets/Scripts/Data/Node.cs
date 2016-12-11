using System;
using UnityEngine;

public class Node
{
    public Vector2 Position;
    public int GlobalIndex;
    public double Temperature;


    public Node(Vector2 position) : this(position, 20.0f)
    {
        Position = position;
    }

    public Node(Vector2 position, double temperature)
    {
        Position = position;
        Temperature = temperature;
    }

    public Node(Vector2 position, int index, double temperature)
    {
        Position = position;
        GlobalIndex = index;
        Temperature = temperature;
    }
}
