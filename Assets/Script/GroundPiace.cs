using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPiace : MonoBehaviour
{
    public bool isColored = false;

    public void ChangeColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
        isColored = true;

        GameManager.singleton.CheckComplete();
    }
}
