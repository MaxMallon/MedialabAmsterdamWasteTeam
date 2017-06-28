using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/// <summary>
/// Just looks at the user's answers to then know what to display in the next scene.
/// </summary>
public class FindStickerScript : MonoBehaviour
{

    public Toggle regular;
    public Toggle electronic;
    public Toggle other;
    public Toggle small;
    public Toggle big;


    private ControllerScript controller;
    // Use this for initialization
    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerScript>();
    }

    /// <summary>
    /// Looks at the answers of the user, and saves them in the controller.
    /// </summary>
    public void Find()
    {
        controller.findStickerScene[0] = electronic.isOn;
        controller.findStickerScene[1] = other.isOn;
        controller.findStickerScene[2] = big.isOn;

        SceneManager.LoadScene("CatalogueScene3");

    }
}
