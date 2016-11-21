using UnityEngine;
using System.Linq;

public class Tester : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        var edges = Edge.GetEdges(mesh.triangles);
        var boundaries = Edge.GetBoudaries(edges);

        var meshColors = new Color[mesh.vertices.Length];

        for (int i = 0; i < meshColors.Length; i++)
        {
            meshColors[i] = Color.red;
        }

        foreach (var item in boundaries)
        {
            meshColors[item.Vertex1] = Color.black;
            meshColors[item.Vertex2] = Color.black;
        }

        mesh.colors = meshColors;

    }

}
