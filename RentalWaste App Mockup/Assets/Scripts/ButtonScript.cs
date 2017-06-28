using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour {


    //The button types needed.
    public enum ButtonTypes {Home, Action, Info, Rent, Trade, Give, Rent1, Rent2, Rent3, Rent4, Picture};
    public ButtonTypes buttonType;

    public Sprite[] spriteArray = new Sprite[4];
    public GameObject rent, trade, give;
    private GameObject controller;


    // Use this for initialization
    void Start ()
    {
        controller = GameObject.Find("Controller");
        if (GetComponent<Button>() != null)
                GetComponent<Button>().onClick.AddListener(() => { ButtonClick(); });

        if (buttonType == ButtonTypes.Rent || buttonType == ButtonTypes.Trade || buttonType == ButtonTypes.Give)
        {
            gameObject.SetActive(false);
        }
        if (buttonType == ButtonTypes.Picture)
        {
            GetComponent<Image>().sprite = spriteArray[controller.GetComponent<ControllerScript>().mode - 1];
        }
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0) && buttonType == ButtonTypes.Action && rent.active)
        {
            StartCoroutine(HideList());
        }
    }

    private IEnumerator HideList()
    {
        yield return new WaitForSeconds(0.1f);
        rent.SetActive(false);
        trade.SetActive(false);
        give.SetActive(false);
    }

    /// <summary>
    /// Let the GameScript do stuff when pressed, what, differs per button.
    /// </summary>
    void ButtonClick()
    {
        switch (buttonType)
        {
            case ButtonTypes.Home:
                SceneManager.LoadScene("HomeScene");
                break;
            case ButtonTypes.Action:
                rent.SetActive(true);
                trade.SetActive(true);
                give.SetActive(true);
                break;
            case ButtonTypes.Info:
                SceneManager.LoadScene("InfoScene");
                break;
            case ButtonTypes.Rent:
                SceneManager.LoadScene("RentScene");
                break;
            case ButtonTypes.Trade:
                SceneManager.LoadScene("TradeScene");
                break;
            case ButtonTypes.Give:
                SceneManager.LoadScene("GiveScene");
                break;
            case ButtonTypes.Rent1:
                SceneManager.LoadScene("SelectScene");
                controller.GetComponent<ControllerScript>().mode = 1;
                break;
            case ButtonTypes.Rent2:
                SceneManager.LoadScene("SelectScene");
                controller.GetComponent<ControllerScript>().mode = 2;
                break;
            case ButtonTypes.Rent3:
                SceneManager.LoadScene("SelectScene");
                controller.GetComponent<ControllerScript>().mode = 3;
                break;
            case ButtonTypes.Rent4:
                SceneManager.LoadScene("SelectScene");
                controller.GetComponent<ControllerScript>().mode = 4;
                break;

        }
    }
}
