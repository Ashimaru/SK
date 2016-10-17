using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "newExample", menuName = "Data/Example")]
public class Example : ScriptableObject {

    public Sprite Sprite;
    [SerializeField]
    public Node[] Nodes;


}
