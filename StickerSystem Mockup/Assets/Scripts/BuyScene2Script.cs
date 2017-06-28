using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is used in the BuyScene2 scene. It changes the sprites depending on what the user chose.
/// </summary>
public class BuyScene2Script : MonoBehaviour {

    bool electronic = false; //TODO make this work

    public Sprite[] sprites;
    public Sprite[] normalButtonSprites, electricButtonSprites;
    public GameObject[] stickerButtons;
    private ControllerScript controller;

    // Use this for initialization
    void Start () {
        controller = GameObject.Find("Controller").GetComponent<ControllerScript>();
        electronic = controller.buyElectronic;
        if (electronic)
        {
            GetComponent<Image>().sprite = sprites[1];
            for (int i = 0; i < stickerButtons.Length; i++)
            {
                stickerButtons[i].GetComponent<Image>().sprite = electricButtonSprites[i];
                switch (i)
                {
                    case 0:
                        stickerButtons[i].GetComponent<StickerScript>().type = 4;
                        break;
                    case 1:
                        stickerButtons[i].GetComponent<StickerScript>().type = 6;
                        break;
                    case 2:
                        stickerButtons[i].GetComponent<StickerScript>().type = 5;
                        break;
                    case 3:
                        stickerButtons[i].GetComponent<StickerScript>().type = 7;
                        break;
                }
            }
        }
        else
        {
            GetComponent<Image>().sprite = sprites[0];
            for (int i = 0; i < stickerButtons.Length; i++)
            {
                stickerButtons[i].GetComponent<Image>().sprite = normalButtonSprites[i];
                switch (i)
                {
                    case 0:
                        stickerButtons[i].GetComponent<StickerScript>().type = 0;
                        break;
                    case 1:
                        stickerButtons[i].GetComponent<StickerScript>().type = 2;
                        break;
                    case 2:
                        stickerButtons[i].GetComponent<StickerScript>().type = 1;
                        break;
                    case 3:
                        stickerButtons[i].GetComponent<StickerScript>().type = 3;
                        break;
                }
            }
        }
	}
}
