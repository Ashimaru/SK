using UnityEngine;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System.IO;
using MathNet.Numerics.LinearAlgebra.Factorization;

[CreateAssetMenu(fileName = "newExample", menuName = "Data/Example")]
public class Example : ScriptableObject
{
    public Mesh Mesh;
}



