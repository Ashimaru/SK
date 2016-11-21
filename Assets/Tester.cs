using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Tester : MonoBehaviour
{

    public GameObject prefab;
    public Transform Position;

    // Use this for initialization
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        var positions = mesh.vertices;
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

        ChangeVisible(ref meshColors, ref boundaries, ref positions);



        mesh.colors = meshColors;

    }

    private void ChangeVisible(ref Color[] meshColors, ref List<Edge> edges, ref Vector3[] positions)
    {
        int detected = 0;
        List<GameObject> objects = new List<GameObject>();

        foreach (var item in edges)
        {
            objects.Add(Instantiate(prefab, transform.TransformPoint(positions[item.Vertex1]), Quaternion.identity)as GameObject);
            objects.Add(Instantiate(prefab, transform.TransformPoint(positions[item.Vertex2]), Quaternion.identity)as GameObject);
        }

        for (int i = 0; i < objects.Count; i++)
        { 
            RaycastHit hit;
            if(Physics.Raycast(Position.position, objects[i].transform.position, out hit))
            {
                if (hit.collider.transform.root.name == objects[i].name)
                {
                    meshColors[i] = Color.green;
                    detected++;
                }
            }
        }
        Debug.Log(detected);
    }

}
