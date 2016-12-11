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

    public Vector<double> BoundryConditionsTemp(float tempBody, List<Edge> boundaries, int nodesCount, Mesh mesh)
    {
        Vector<double> boundryConditions = Vector<double>.Build.Dense(nodesCount, tempBody);
        for (int i = 0; i < boundaries.Count; i++)
        {
            boundryConditions[boundaries[i].Vertex1] = 0;
            boundryConditions[boundaries[i].Vertex2] = 0;
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

    public void SimplifyEquation(ref Matrix<double> matrix, Vector<double> vector, List<int> boundaryNodes, float envTemp)
    {
        for (int i = 0; i < matrix.RowCount; i++)
        {
            if (boundaryNodes.Contains(i))
            {
                for (int j = 0; j < vector.Count; j++)
                {
                    if (j != i)
                    {
                        vector[j] += matrix[j, i] * envTemp;
                    }
                }
                ClearMatrixRowColumn(ref matrix, i);
            }
        }

        for (int i = 0; i < boundaryNodes.Count; i++)
        {
            vector[boundaryNodes[i]] = envTemp;
        }
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
