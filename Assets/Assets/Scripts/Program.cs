﻿using UnityEngine;
using System.Collections;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.IO;

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
            string s = GlobalStiffnessMatrix.ToMatrixString(nodesCount, nodesCount);
            File.WriteAllText(@"D:\GlobalStiffnessMatrix" + i + ".txt", s);
        }
        return GlobalStiffnessMatrix;
    }

    public float CountFlux(Materiall material, float temperature)
    {
        float result = -material.ConductCoefficientX;

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

    public Vector<double> BoundryConditionsTemp(float envTemp, List<Edge> boundaries, int nodesCount, Mesh mesh)
    {
        Vector<double> boundryConditions = Vector<double>.Build.Dense(nodesCount, 0);
        for (int i = 0; i < boundaries.Count; i++)
        {
            boundryConditions[boundaries[i].Vertex1] = envTemp;
            boundryConditions[boundaries[i].Vertex2] = envTemp;
        }

        return boundryConditions;
    }

    public Vector<double> GetBoundaryTemps(float firstBoundaryTemp, float secondBoundaryTemp, List<int> boundaryNodes, Mesh mesh)
    {
        Vector<double> boundryConditions = Vector<double>.Build.Dense(mesh.vertexCount, secondBoundaryTemp);
        for (int i = 0; i < boundaryNodes.Count; i++)
        {
            boundryConditions[boundaryNodes[i]] = firstBoundaryTemp;
        }

        return boundryConditions;
    }

    public Vector<double> CountSolution(Matrix<double> GlobalStiffnessMatrix, Vector<double> boundaryConditions)
    {
        var U = GlobalStiffnessMatrix.UpperTriangle();
        var L = GlobalStiffnessMatrix.LowerTriangle();
        var u = GlobalStiffnessMatrix - L;
        var D = U - u;

        var y = U.Transpose().Inverse() * boundaryConditions;
        var solution = U.Inverse() * D.Inverse() * y;

        return solution;
    }

    public Vector<double> SimplifyEquation(ref Matrix<double> matrix, Vector<double> temperatures, List<int> boundaryNodes, Materiall material)
    {
        Vector<double> vector = Vector<double>.Build.Dense(temperatures.Count, 0);

        for (int i = 0; i < matrix.RowCount; i++)
        {
            if (boundaryNodes.Contains(i))
            {
                for (int j = 0; j < vector.Count; j++)
                {
                    if (j != i)
                    {
                        vector[j] += -matrix[j, i] * temperatures[i];
                    }
                }
                ClearMatrixRowColumn(ref matrix, i);
            }
        }

        for (int i = 0; i < boundaryNodes.Count; i++)
        {
            vector[boundaryNodes[i]] = temperatures[boundaryNodes[i]];
        }

        return vector;
    }

    public void ClearMatrixRowColumn(ref Matrix<double> matrix, int j)
    {
        for (int i = 0; i < matrix.ColumnCount; i++)
        {
            if (i != j)
            {
                matrix[j, i] = 0;
            }
            else
            {
                matrix[j, i] = 1;
            }
        }

        for (int i = 0; i < matrix.ColumnCount; i++)
        {
            if (i != j)
            {
                matrix[i, j] = 0;
            }
            else
            {
                matrix[i, j] = 1;
            }
        }
    }

    public Matrix<double> MultiplyMatrixByVector(Matrix<double> matrix, Vector<double> vector)
    {
        Matrix<double> result = Matrix<double>.Build.Dense(matrix.RowCount, matrix.ColumnCount);

        for (int i = 0; i < matrix.ColumnCount; i++)
        {
            for (int j = 0; j < matrix.RowCount; j++)
            {
                if (vector[i] != 0)
                    result[j, i] = matrix[j, i] * vector[i];
                else
                    result[j, i] = matrix[j, i];
            }
        }

        return result;
    }

}
