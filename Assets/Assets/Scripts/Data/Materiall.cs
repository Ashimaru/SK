using System;
using UnityEngine;
using MathNet.Numerics;

[CreateAssetMenu(fileName = "NewMaterial", menuName = "Data/Material")]
public class Materiall : ScriptableObject
{
    public float ConductCoefficientX = 0;
    public float ConductCoefficientY = 0;
    public float Density = 0;
}
