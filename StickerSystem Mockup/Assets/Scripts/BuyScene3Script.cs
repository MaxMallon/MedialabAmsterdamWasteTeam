using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/// <summary>
/// Basically just to hold sprites.
/// </summary>
public class BuyScene3Script : MonoBehaviour {
    private ControllerScript controller;
    public Sprite[] sprites;
    public GameObject sticker;
    // Use this for initialization
    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerScript>();
        sticker.GetComponent<Image>().sprite = sprites[controller.selectedSticker];
    }
	
	//Go to the next scene.
	public void Buy () {
        
        SceneManager.LoadScene("BuyScene4");
	}
}
