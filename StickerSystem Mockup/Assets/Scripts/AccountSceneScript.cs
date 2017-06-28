using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// This script places the name of the user currently logged in on the account page.
/// </summary>
public class AccountSceneScript : MonoBehaviour
{

    private ControllerScript controller;
    // Use this for initialization
    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerScript>();
        if (controller.currentUser != -1)
        {
            if ((string)controller.names[controller.currentUser] == "")
            {
                //Use the username if no first name has been given.
                GetComponent<Text>().text = "Hello, " + (string)controller.userNames[controller.currentUser] + "!";
            }
            else
            {
                GetComponent<Text>().text = "Hello, " + (string)controller.names[controller.currentUser] + "!";
            }
        }
    }
}
