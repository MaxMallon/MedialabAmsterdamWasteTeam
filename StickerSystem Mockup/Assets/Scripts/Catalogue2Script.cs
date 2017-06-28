using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Changes sprites.
/// </summary>
public class Catalogue2Script : MonoBehaviour
{

    bool electronic = false;

    public Sprite[] sprites;
    private ControllerScript controller;

    // Use this for initialization
    void Start()
    {

        controller = GameObject.Find("Controller").GetComponent<ControllerScript>();
        electronic = controller.catalogueElectronic;
        if (electronic)
        {
            GetComponent<Image>().sprite = sprites[1];
        }
        else
        {
            GetComponent<Image>().sprite = sprites[0];

        }
    }
}
