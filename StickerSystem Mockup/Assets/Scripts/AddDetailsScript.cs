using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/// <summary>
/// This script is used on the Add Details Scene
/// </summary>
public class AddDetailsScript : MonoBehaviour {

    public InputField nameField;
    public InputField surnameField;
    public InputField phoneField;
    public InputField addressField;
    public InputField zipField;


    private ControllerScript controller;
    // Use this for initialization
    void Start()
    {

        phoneField.contentType = InputField.ContentType.DecimalNumber;

        controller = GameObject.Find("Controller").GetComponent<ControllerScript>();
        //This part replaces all the fields's text with previously saved text, if there is any.
        if ((string)controller.names[controller.currentUser] != "")
        {
            nameField.text = (string)controller.names[controller.currentUser] + "";
        }
        if ((string)controller.surnames[controller.currentUser] != "")
        {
            surnameField.text = (string)controller.surnames[controller.currentUser] + "";
        }
        if ((string)controller.phoneNumbers[controller.currentUser] != "")
        {
            phoneField.text = (string)controller.phoneNumbers[controller.currentUser] + "";
        }
        if ((string)controller.adresses[controller.currentUser] != "")
        {
            addressField.text = (string)controller.adresses[controller.currentUser] + "";
        }
        if ((string)controller.zipCodes[controller.currentUser] != "")
        {
            zipField.text = (string)controller.zipCodes[controller.currentUser] + "";
        }

    }

    /// <summary>
    /// Saves all the entered text, and loads the account scene. 
    /// </summary>
    public void AddDetails() {
        string nameFieldInput = nameField.transform.Find("Text").GetComponent<Text>().text;
        string surnameFieldInput = surnameField.transform.Find("Text").GetComponent<Text>().text;
        string phoneFieldInput = phoneField.transform.Find("Text").GetComponent<Text>().text;
        string addressFieldInput = addressField.transform.Find("Text").GetComponent<Text>().text;
        string zipFieldInput = zipField.transform.Find("Text").GetComponent<Text>().text;

        controller.names[controller.currentUser] = nameFieldInput;
        controller.surnames[controller.currentUser] = surnameFieldInput;
        controller.adresses[controller.currentUser] = addressFieldInput;
        controller.phoneNumbers[controller.currentUser] = phoneFieldInput;
        controller.zipCodes[controller.currentUser] = zipFieldInput;

        SceneManager.LoadScene("AccountScene");
    }
}
