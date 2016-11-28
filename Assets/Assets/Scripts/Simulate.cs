using UnityEngine;
using System.Collections;

public class Simulate : MonoBehaviour {

    public void StartSimultaion(Loader loader)
    {
        Solver solver = new Solver();
        solver.Solve(loader);
    }



}
