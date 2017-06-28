using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script sends button type and button actions to the controllerscript.
/// </summary>
public class MenuButtonScript : MonoBehaviour
{

    ControllerScript controller;
    public enum ButtonTypes {a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, Record, Stop, Next, Quit, Restart, Dutch, English}; //answer 0 to 9, and the other types.  a0 = yes by default, a1 no.
    public ButtonTypes buttonType;

    public Sprite[] sprites = new Sprite[5];


    // Use this for initialization
    void Start()
    {
        controller = GameObject.Find("ControllerCanvas").GetComponent<ControllerScript>();
        GetComponent<Button>().onClick.AddListener(() => { ButtonClick(); }); 
        //add ButtonClick to button click method.if (buttonType == ButtonTypes.a0 || buttonType == ButtonTypes.a1)
        {
            //transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(0, 80); //Todo, fix this later.
        }

    }
    
    /// <summary>
    /// Send the controller the button press event.
    /// </summary>
    void ButtonClick()
    {
        controller.ButtonPress(buttonType.ToString());
        if (buttonType == ButtonTypes.Record)
        {
            ColorBlock block = new ColorBlock();
            block.normalColor = new Color32(255, 255, 255, 255);
            block.highlightedColor = new Color32(255, 0, 0, 255);
            block.disabledColor = new Color32(255, 0, 0, 255);
            block.pressedColor = new Color32(255, 0, 0, 255);
            block.fadeDuration = 0.1f;
            block.colorMultiplier = 1f;

            GetComponent<Button>().colors = block;
        }
        
    }


    /// <summary>
    /// Pressing enter = pressing next.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (buttonType == ButtonTypes.Next)
            {
                ButtonClick();
            }
        }
    }

    /// <summary>
    /// Set the type of the button.
    /// </summary>
    /// <param name="type">String representing the buttontype, if it's not an existing type, nothing happens.</param>
    public void SetButtonType(string type)
    {
        buttonType = (ButtonTypes)System.Enum.Parse(typeof(ButtonTypes), type);
        switch (buttonType)
        {
            case ButtonTypes.Next:
                GetComponent<Image>().sprite = sprites[0];
                break;
            case ButtonTypes.a0:
                //GetComponent<Image>().sprite = sprites[1];
                break;
            case ButtonTypes.a1:
                //GetComponent<Image>().sprite = sprites[2];
                break;
            case ButtonTypes.Quit:
                GetComponent<Image>().sprite = sprites[3];
                //GetComponent<RectTransform>().localScale = new Vector2(1, 1);//new Vector2(1.0f / GetComponent<RectTransform>().localScale.y, 1.0f / GetComponent<RectTransform>().localScale.x);
                break;
            case ButtonTypes.Restart:
                GetComponent<Image>().sprite = sprites[4];
                break;
            case ButtonTypes.Record:
                GetComponent<Image>().sprite = sprites[5];
                break;
            case ButtonTypes.Stop:
                GetComponent<Image>().sprite = sprites[6];
                break;
        }
        GetComponent<Image>().preserveAspect = true;
        GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta * GetComponent<Image>().sprite.pixelsPerUnit;   
    }
}
