using UnityEngine;
using System.Collections;
using Assets.Assets.Scripts;

public class Simulate : MonoBehaviour {

    public void StartSimultaion(Loader loader)
    {
        FiniteDifferenceMethod fdm = new FiniteDifferenceMethod();
        Solver solver = new Solver();
        fdm.Solve(loader);
        //solver.Solve(loader);
    }



}
