using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The script that describes the behaviour of the several buttons in the game.
/// </summary>
public class ActionButtonScript : MonoBehaviour {

    //The button types needed.
    public enum ButtonTypes { Next, Cancel, Select, Guess};
    public ButtonTypes buttonType;

    /// <summary>
    /// All buttons aside from the guessing one should be hidden when the game starts.
    /// </summary>
    void Start ()
    {
        GetComponent<Button>().onClick.AddListener(() => { ButtonClick(); });
        if (buttonType != ButtonTypes.Guess)
            gameObject.SetActive(false);
    }


    /// <summary>
    /// Let the GameScript do stuff when pressed, what, differs per button.
    /// </summary>
    void ButtonClick()
    {
        switch (buttonType)
        {
            case ButtonTypes.Next:
                GetComponentInParent<GameScript>().NextQuestion();
                break;
            case ButtonTypes.Cancel:
                GetComponentInParent<GameScript>().SetQuestion(-1);
                break;
            case ButtonTypes.Select:
                GetComponentInParent<GameScript>().StartCoroutine(GetComponentInParent<GameScript>().AskQuestion());
                break;
            case ButtonTypes.Guess:
                GetComponentInParent<GameScript>().GuessPersona();
                break;
        }

    }

    /// <summary>
    /// Let's the select button blink.
    /// </summary>
    /// <returns></returns>
   public IEnumerator SelectBlink()
    {
        if (GetComponent<Button>().image.color == Color.white)
        {
            GetComponent<Button>().image.color = Color.green;
        }
        else
        {
            GetComponent<Button>().image.color = Color.white;
        }


        yield return new WaitForSeconds(0.7f);
        if (gameObject.activeInHierarchy)
            StartCoroutine(SelectBlink());
    }
}
