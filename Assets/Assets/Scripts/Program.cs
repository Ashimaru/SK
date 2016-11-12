using UnityEngine;
using System.Collections;
using MathNet.Numerics.LinearAlgebra;
using System;

public class Program
{

    public Matrix<float> GlobalStiffnessMatrix;

    public Program(int nodesCount)
    {
        GlobalStiffnessMatrix = Matrix<float>.Build.Dense(nodesCount, nodesCount);
    }

    public void AssembleGlobalStiffnessMatrix(FiniteElement[] fe)
    {
        for (int i = 0; i < fe.Length; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    GlobalStiffnessMatrix[fe[i].nodes[j].GlobalIndex, fe[i].nodes[k].GlobalIndex] +=
                        fe[i].LocalStiffnessMatrix[j, k];
                }
            }
        }
    }
}
