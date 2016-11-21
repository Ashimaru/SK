using UnityEngine;
using System.Collections;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;

public class Program
{
    private static Program _instance;
    public static Program Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Program();
            }
            return _instance;
        }
    }

    private Program()
    {

    }

    public Matrix<float> AssembleGlobalStiffnessMatrix(FiniteElement[] fe, int nodesCount)
    {
        Matrix<float> GlobalStiffnessMatrix = Matrix<float>.Build.Dense(nodesCount, nodesCount);

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
        return GlobalStiffnessMatrix;
    }

    public float CountFlux(Materiall material, float temperature)
    {
        float result = -material.ConductCoefficient;

        return result * temperature;
    }

    public float[] BoundryConditions(float flux, List<Edge> boundaries, int nodesCount, Mesh mesh)
    {
        float[] boundryConditions = new float[nodesCount];
        for(int i = 0; i < boundaries.Count; i++)
        {
            boundryConditions[boundaries[i].Vertex1] += (flux * boundaries[i].Length(mesh)) / 2;
            boundryConditions[boundaries[i].Vertex2] += (flux * boundaries[i].Length(mesh)) / 2;
        }

        return boundryConditions;
    }

    public void CountingTemperatures(List<FiniteElement> fe)
    {

    }
}
