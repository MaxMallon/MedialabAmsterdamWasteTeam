using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Changes the sprite of the sticker that has been scanned, and sets variables appropiately.
/// </summary>
public class ScannedStickersScript : MonoBehaviour {


    public Sprite[] sprites;
    public int type;
    public Text statusText;
    private ControllerScript controller;
    public Button Activate, Deactivate;

        // Use this for initialization
        void Start () {

        controller = GameObject.Find("Controller").GetComponent<ControllerScript>();
        switch (GameObject.Find("Controller").GetComponent<ControllerScript>().scannedText)
        {
            case "This is the small, regular, normal sticker.":
                GetComponent<Image>().sprite = sprites[1];
                type = 1;
                break;
            case "This is the large, regular, normal sticker.":
                GetComponent<Image>().sprite = sprites[3];
                type = 3;
                break;
            case "This is the small, priority, normal sticker.":
                GetComponent<Image>().sprite = sprites[0];
                type = 0;
                break;
            case "This is the large, priority, normal sticker.":
                GetComponent<Image>().sprite = sprites[2];
                type = 2;
                break;
            case "This is the small, regular, electronic sticker.":
                GetComponent<Image>().sprite = sprites[5];
                type = 5;
                break;
            case "This is the large, regular, electronic sticker.":
                GetComponent<Image>().sprite = sprites[7];
                type = 7;
                break;
            case "This is the small, priority, electronic sticker.":
                GetComponent<Image>().sprite = sprites[4];
                type = 4;
                break;
            case "This is the large, priority, electronic sticker.":
                GetComponent<Image>().sprite = sprites[6];
                type = 6;
                break;
            default:
                GetComponent<Image>().sprite = sprites[8];
                type = 8;
                break;
        }
        GetComponent<Image>().preserveAspect = true;
        statusText.text = "Current status:\n";
        if (controller.miniDateDatabase[type] == null)
        {
            statusText.text += "not activated";
            Deactivate.interactable = false;
        }
        else
        {
            statusText.text += "activated\n\nScanned on:\n";
            statusText.text += controller.miniDateDatabase[type];
            statusText.text += "\n\nLocation:\n"; 
            statusText.text += controller.miniGPSDatabase[type];
            Activate.interactable = false;
        }
	}
	
}
