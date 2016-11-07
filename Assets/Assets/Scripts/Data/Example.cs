using UnityEngine;

[CreateAssetMenu(fileName = "newExample", menuName = "Data/Example")]
public class Example : ScriptableObject {

    public Mesh Mesh;
    FiniteElement[] elements;

    public void Load()
    {
        elements = new FiniteElement[Mesh.triangles.Length];
        for (int i = 0; i < elements.Length; i+=3)
        {
            Node[] nodes = new Node[]
            {
                new Node(Mesh.vertices[Mesh.triangles[i]]),
                new Node(Mesh.vertices[Mesh.triangles[i+1]]),
                new Node(Mesh.vertices[Mesh.triangles[i+2]])
            };
        }
    }



}
