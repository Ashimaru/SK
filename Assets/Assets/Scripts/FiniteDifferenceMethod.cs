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
            List<FDMNode> nodes = new List<FDMNode>();

            Mesh mesh = loader.activeExample.Mesh;
            var edges = Edge.GetEdges(mesh.triangles);
            var boundaries = Edge.GetBoudaries(edges);
            var boundaryNodes = Edge.GetBoundaryNodesIndexes(boundaries);

            Color[] colors = new Color[mesh.vertices.Length];

            mesh.colors = colors;

            for (int  i = 0; i < mesh.vertexCount; i++)
            {
                nodes.Add(new FDMNode(i, mesh.vertices[i]));
            }

            float x;
            float y;
            foreach (var edge in edges)
            {
                var distance = nodes[edge.Vertex1].position - nodes[edge.Vertex2].position;
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
                            nodes[edge.Vertex2].upperNode= nodes[edge.Vertex1];
                        }
                        else
                        {
                            nodes[edge.Vertex1].upperNode = nodes[edge.Vertex2];
                            nodes[edge.Vertex2].lowerNode = nodes[edge.Vertex1];
                        }
                    }
                }
            }
            
            Matrix<double> coeffiniectsMatrix = Matrix<double>.Build.Dense(mesh.vertexCount, mesh.vertexCount);

            for(int i = 0; i < mesh.vertexCount; i++)
            {
                coeffiniectsMatrix[i, nodes[i].index] += -2 / (nodes[i].DeltaX * nodes[i].DeltaX);
                coeffiniectsMatrix[i, nodes[i].index] += -2 / (nodes[i].DeltaY * nodes[i].DeltaY);
                if (nodes[i].previousNode != null)
                    coeffiniectsMatrix[i, nodes[i].previousNode.index] += 1 / (nodes[i].DeltaX * nodes[i].DeltaX);
                if (nodes[i].nextNode != null)
                    coeffiniectsMatrix[i, nodes[i].nextNode.index] += 1 / (nodes[i].DeltaX * nodes[i].DeltaX);
                if (nodes[i].upperNode != null)
                    coeffiniectsMatrix[i, nodes[i].upperNode.index] += 1 / (nodes[i].DeltaX * nodes[i].DeltaY);
                if (nodes[i].lowerNode != null)
                    coeffiniectsMatrix[i, nodes[i].lowerNode.index] += 1 / (nodes[i].DeltaX * nodes[i].DeltaY);
            }

            string str = coeffiniectsMatrix.ToMatrixString(mesh.vertexCount, mesh.vertexCount);

            File.WriteAllText(@"D:\CoefficientMatrix.txt", str);

            boundaryNodes.Sort();

            List<int> bndN = new List<int>();

            for(int i = 0; i <= 25; i++)
            {
                bndN.Add(boundaryNodes[i]);
            }

            for (int i = boundaryNodes.Count - 1; i > boundaryNodes.Count - 26; i--)
            {
                bndN.Add(boundaryNodes[i]);
            }

            var temps = Program.Instance.GetBoundaryTemps(loader.EnviromentTemperature, loader.ObjectTemperature, bndN, mesh);

            var rightSide = Program.Instance.SimplifyEquation(ref coeffiniectsMatrix, temps, bndN);

            var temperatures = coeffiniectsMatrix.Solve(rightSide);

            for (int i = 0; i < mesh.vertexCount; i++)
                colors[i] = GetTemperatureFromValue((float)temperatures[i], loader.Gradient);

            mesh.colors = colors;
        }

        private Color GetTemperatureFromValue(float temperature, Texture2D gradient)
        {
            float val = temperature / 300.0f;
            return gradient.GetPixel(Mathf.FloorToInt(val * gradient.width), 1);
        }
    }
}
