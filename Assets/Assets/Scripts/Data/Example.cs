using UnityEngine;

[CreateAssetMenu(fileName = "newExample", menuName = "Data/Example")]
public class Example : ScriptableObject
{

    public Mesh Mesh;
    FiniteElement[] elements;

    public void Load()
    {
        Materiall material = new Materiall();
        material.ConductCoefficient = 5;
        elements = new FiniteElement[Mesh.triangles.Length / 3];
        for (int i = 0; i < elements.Length; i++)
        {
            Node[] nodes = new Node[]
            {
                new Node(Mesh.vertices[Mesh.triangles[3*i]], Mesh.triangles[3*i]),
                new Node(Mesh.vertices[Mesh.triangles[3*i+1]], Mesh.triangles[3*i+1]),
                new Node(Mesh.vertices[Mesh.triangles[3*i+2]], Mesh.triangles[3*i+2])
            };
            elements[i] = new FiniteElement(nodes, material);
        }

        Program program = new Program(Mesh.vertices.Length);

        program.AssembleGlobalStiffnessMatrix(elements);
    }
}
