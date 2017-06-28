using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Changing sprites based on what the user did previously.
/// </summary>
public class CatalogueResultScript : MonoBehaviour {
    private ControllerScript controller;
    bool big = false;
    bool other = false;
    bool electronic = false;

    public Sprite[] sprites;
    public string[] texts;

    public GameObject image1, image2;
    // Use this for initialization
    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerScript>();
        electronic = controller.findStickerScene[0];
        other = controller.findStickerScene[1];
        big = controller.findStickerScene[2];

        int a = 0;
        if (electronic && !other)
        {
            a += 4;
        }
        if (big)
        {
            a += 2;
        }

        image1.GetComponent<Image>().sprite = sprites[a];
        image2.GetComponent<Image>().sprite = sprites[a+1];
        image1.GetComponent<Image>().preserveAspect = true;
        image2.GetComponent<Image>().preserveAspect = true;
        image1.GetComponent<StickerScript>().type += a;
        image2.GetComponent<StickerScript>().type += a + 1;

        image1.GetComponentInChildren<Text>().text = texts[a];
        image2.GetComponentInChildren<Text>().text = texts[a + 1];
    }
}
