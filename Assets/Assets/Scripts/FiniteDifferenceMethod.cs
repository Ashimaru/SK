using Assets.Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System.IO;

namespace Assets.Assets.Scripts
{
    class FiniteDifferenceMethod
    {

        public void Solve(Loader loader)
        {
            Mesh mesh = loader.activeExample.Mesh;
            var edges = Edge.GetEdges(mesh.triangles);
            var boundaries = Edge.GetBoudaries(edges);
            var boundaryNodes = Edge.GetBoundaryNodesIndexes(boundaries);
            var nodes = CreateNodes(mesh, edges);
            Materiall material = loader.activeMaterial;
            boundaryNodes.Sort();

            List<int> bndN = new List<int>();

            foreach(var node in boundaryNodes)
            {
                if (nodes[node].position.y == mesh.bounds.min.y)
                    bndN.Add(node);
            }

            Color[] colors = new Color[mesh.vertices.Length];

            mesh.colors = colors;

            var coeffiniectsMatrix = BuildCoefficientMatrix(mesh, nodes);

            coeffiniectsMatrix *= material.ConductCoefficientX;

            var temps = Program.Instance.GetBoundaryTemps(loader.EnviromentTemperature, loader.ObjectTemperature, bndN, mesh);

            var rightSide = Program.Instance.SimplifyEquation(ref coeffiniectsMatrix, temps, bndN, material);

            var temperatures = coeffiniectsMatrix.Solve(rightSide);

            for (int i = 0; i < mesh.vertexCount; i++)
                colors[i] = GetTemperatureFromValue((float)temperatures[i], loader.Gradient);

            mesh.colors = colors;
        }

        private List<FDMNode> CreateNodes (Mesh mesh, List<Edge> edges)
        {

            List<FDMNode> nodes = new List<FDMNode>();

            for (int i = 0; i < mesh.vertexCount; i++)
            {
                nodes.Add(new FDMNode(i, mesh.vertices[i]));
            }

            SetNeighbours(ref nodes, edges);

            return nodes;
        }

        private void SetNeighbours(ref List<FDMNode> nodes, List<Edge> edges)
        {
            float x;
            float y;
            Vector2 distance;

            foreach (var edge in edges)
            {
                distance = nodes[edge.Vertex1].position - nodes[edge.Vertex2].position;
                x = distance.x;
                y = distance.y;
                if (x == 0 ^ y == 0)
                {
                    if (x != 0)
                    {
                        if (x > 0)
                        {
                            nodes[edge.Vertex1].previousNode = nodes[edge.Vertex2];
                            nodes[edge.Vertex2].nextNode = nodes[edge.Vertex1];
                        }
                        else
                        {
                            nodes[edge.Vertex1].nextNode = nodes[edge.Vertex2];
                            nodes[edge.Vertex2].previousNode = nodes[edge.Vertex1];
                        }
                    }

                    if (y != 0)
                    {
                        if (y > 0)
                        {
                            nodes[edge.Vertex1].lowerNode = nodes[edge.Vertex2];
                            nodes[edge.Vertex2].upperNode = nodes[edge.Vertex1];
                        }
                        else
                        {
                            nodes[edge.Vertex1].upperNode = nodes[edge.Vertex2];
                            nodes[edge.Vertex2].lowerNode = nodes[edge.Vertex1];
                        }
                    }
                }
            }
        }

        private Matrix<double> BuildCoefficientMatrix(Mesh mesh, List<FDMNode> nodes)
        {
            Matrix<double> coefficientMatrix = Matrix<double>.Build.Dense(mesh.vertexCount, mesh.vertexCount);

            for (int i = 0; i < mesh.vertexCount; i++)
            {
                coefficientMatrix[i, nodes[i].index] += -2 / (nodes[i].DeltaX * nodes[i].DeltaX);
                coefficientMatrix[i, nodes[i].index] += -2 / (nodes[i].DeltaY * nodes[i].DeltaY);
                if (nodes[i].previousNode != null)
                    coefficientMatrix[i, nodes[i].previousNode.index] += 1 / (nodes[i].DeltaX * nodes[i].DeltaX);
                if (nodes[i].nextNode != null)
                    coefficientMatrix[i, nodes[i].nextNode.index] += 1 / (nodes[i].DeltaX * nodes[i].DeltaX);
                if (nodes[i].upperNode != null)
                    coefficientMatrix[i, nodes[i].upperNode.index] += 1 / (nodes[i].DeltaX * nodes[i].DeltaY);
                if (nodes[i].lowerNode != null)
                    coefficientMatrix[i, nodes[i].lowerNode.index] += 1 / (nodes[i].DeltaX * nodes[i].DeltaY);
            }

            return coefficientMatrix;
        }

        private Color GetTemperatureFromValue(float temperature, Texture2D gradient)
        {
            float val = temperature / 300.0f;
            return gradient.GetPixel(Mathf.FloorToInt(val * gradient.width), 1);
        }
    }
}
