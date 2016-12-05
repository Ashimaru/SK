using UnityEngine;
using System.Collections;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
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

    public Matrix<double> AssembleGlobalStiffnessMatrix(FiniteElement[] fe, int nodesCount)
    {
        Matrix<double> GlobalStiffnessMatrix = DenseMatrix.Create(nodesCount, nodesCount, 0.0);
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

    public Vector<double> BoundryConditions(float flux, List<Edge> boundaries, int nodesCount, Mesh mesh)
    {
        Vector<double> boundryConditions = Vector<double>.Build.Dense(nodesCount, 0);
        for (int i = 0; i < boundaries.Count; i++)
        {
            boundryConditions[boundaries[i].Vertex1] += (flux * boundaries[i].Length(mesh)) / 2;
            boundryConditions[boundaries[i].Vertex2] += (flux * boundaries[i].Length(mesh)) / 2;
        }

        return boundryConditions;
    }

    public Vector<double> BoundryConditionsTemp(float tempEnv, float tempBody, List<Edge> boundaries, int nodesCount, Mesh mesh)
    {
        Vector<double> boundryConditions = Vector<double>.Build.Dense(nodesCount, tempBody);
        for (int i = 0; i < boundaries.Count; i++)
        {
            boundryConditions[boundaries[i].Vertex1] = tempEnv;
            boundryConditions[boundaries[i].Vertex2] = tempEnv;
        }

        return boundryConditions;
    }

    public Vector<double> CountSolution(Matrix<double> GlobalStiffnessMatrix, Vector<double> boundryConditions)
    {
        var U = GlobalStiffnessMatrix.UpperTriangle();
        var L = GlobalStiffnessMatrix.LowerTriangle();
        var u = GlobalStiffnessMatrix - L;
        var D = U - u;

        var y = U.Transpose().Inverse() * boundryConditions;
        var solution = U.Inverse() * D.Inverse() * y;

        return solution;
    }

}
