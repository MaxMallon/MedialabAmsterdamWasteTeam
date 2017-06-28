using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This script contains the behaviour of the Persona tiles
/// </summary>
public class Persona : MonoBehaviour {

    bool[] QuestionAnswers; //Contains the persona's answers to the questions.
    public int number; //Number of the persnoa.
    public bool clickingTime = false; //Whether or not the player should be able to flip the personas.
    public bool guessingTime = false; //Whether or nto the player is looking to guess what persona they are.
    public Sprite[] spriteList = new Sprite[15]; //List of all the sprites for all the persona's
    
    /// <summary>
    /// Binding events to mouse over and mouse leave, for displaying the name + box
    /// </summary>
    void Start () {
        GetComponent<Button>().onClick.AddListener(() => { ButtonClick(); });
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { MouseEnterMethod((PointerEventData)data); });
        trigger.triggers.Add(entry);

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerExit;
        entry2.callback.AddListener((data) => { MouseExitMethod((PointerEventData)data); });
        trigger.triggers.Add(entry2);

        //Name + box should be hidden at the start.
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// If the mouse enters the tile, the box + name should appear.
    /// </summary>
    /// <param name="data"></param>
    void MouseEnterMethod(PointerEventData data)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// If the mouse leaves the tile, the box + name should disappear.
    /// </summary>
    /// <param name="data"></param>
    void MouseExitMethod(PointerEventData data)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// If the player clicks on a persona tile, stuff should happen.
    /// </summary>
    void ButtonClick()
    {
        if (clickingTime) //only if it's time to click
        {
            //if the player is guessing, execute 'TookAGuess' instead.
            if (guessingTime)
            {
                GetComponentInParent<GameScript>().StartCoroutine(GetComponentInParent<GameScript>().TookAGuess(number));
                return;
            }
            //if not, disable the persona
            GetComponent<Button>().interactable = false;
            GetComponentInParent<GameScript>().openPersonas--;
            //If this was the last persona to be disabled, execute 'TookAGuess', with -1 parameter, to tell the player they are out of options.
            if (GetComponentInParent<GameScript>().openPersonas == 0)
            {
                GetComponentInParent<GameScript>().StartCoroutine(GetComponentInParent<GameScript>().TookAGuess(-1));
            }
            
        }

    }

    /// <summary>
    /// Set the answers to all questions;
    /// </summary>
    /// <param name="answers"></param>
    public void SetQuestionAnswers(bool[] answers)
    {
        QuestionAnswers = answers;
    }


    /// <summary>
    /// Ask a question, and check if it's true
    /// </summary>
    /// <param name="index">The question's index</param>
    /// <param name="value">The answers to the question</param>
    /// <returns>Whether or not the question is true.</returns>
   public bool AskQuestion(int index, bool value)
    {
        if (QuestionAnswers[index] = value)
        {
            return true;
        }
        return false;
    }
    public bool AskQuestion(int index)
    {
        return QuestionAnswers[index];
    }
}
