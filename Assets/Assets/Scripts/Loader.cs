using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Loader : MonoBehaviour
{

    public Example[] Examples;
    public Materiall[] Materials;

    public Example activeExample;
    public Materiall activeMaterial;
    public float EnviromentTemperature;
    public float ObjectTemperature;

    public Dropdown Example;
    public Dropdown Material;
    public InputField EnviromentValue;
    public InputField ObjectValue;


    void Start()
    {
        //Node[] nodes =
        //{
        //    new Node(new Vector2(0,0)),
        //    new Node(new Vector2(4,0.5f)),
        //    new Node(new Vector2(2,5))
        //};

        //FiniteElement fe = new FiniteElement(nodes);
    }

    public void LoadExample()
    {

    }

    public void SetEnviromentTemperature()
    {
        float val;
        try
        {
            float.TryParse(EnviromentValue.text, out val);
            if (val > 300 || val < 60)
            {
                val = 60.0f;
                EnviromentValue.text = "60.0";
            }
        }
        catch (System.Exception)
        {
            EnviromentValue.text = "60.0";
            val = 60.0f;
        }
        EnviromentTemperature = val;
    }

    public void LoadMaterial()
    {

    }

    public void SetObjectTemperature()
    {
        float val = 0;
        try
        {
            float.TryParse(ObjectValue.text, out val);
            if (val > 35.0f || val < 0.0f)
            {
                val = 0.0f;
                ObjectValue.text = "0.0";
            }
        }
        catch (System.Exception)
        {
            val = 0;
            ObjectValue.text = "0.0";
        }

        ObjectTemperature = val;
    }

}
