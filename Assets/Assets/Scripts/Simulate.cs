using UnityEngine;
using System.Collections;

public class Simulate : MonoBehaviour {

    public Example example;


    public void StartSimultaion(Loader loader)
    {
        example.Load(loader);
    }
}
