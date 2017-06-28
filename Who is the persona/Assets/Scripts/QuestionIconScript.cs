using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is for the behaviour of the question icons.
/// </summary>
public class QuestionIconScript : MonoBehaviour {

    public int number; //Icon number
    public GameObject controller; //The canvas with its Gamescript.
    public bool clickingTime = true; //Should a click do something now.
    public bool disabeled = false; //has the question been asked before
    public Sprite[] spriteList = new Sprite[10]; //List of sprites

	/// <summary>
    /// Starting and adding the buttonclick to the butoon.
    /// </summary>
	void Start () {
        GetComponent<Button>().onClick.AddListener(() => { ButtonClick(); });
    }
    
    /// <summary>
    /// If the question is asked, invoke the right stuff in the parent.
    /// </summary>
    void ButtonClick()
    {
        GetComponentInParent<GameScript>().SetQuestion(number);
    }

    /// <summary>
    /// Should a click do something now
    /// </summary>
    public bool ClickingTime
    {
        get
        {
            return clickingTime;
        }
        set
        {
            clickingTime = value;
            if (!clickingTime)
            {//If it's not the time to click, disable the button.
                GetComponent<Button>().interactable = false;
            }
            else
            {
                if (!disabeled)
                {//if it's permanently disabled, don't enable again when it's time to click again.
                    GetComponent<Button>().interactable = true;
                }
            }
        }
    }


   
}
