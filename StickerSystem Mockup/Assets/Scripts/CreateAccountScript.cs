using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// The script that saves all the inputed info from the input fields to create an account.
/// </summary>
public class CreateAccountScript : MonoBehaviour
{

    public InputField nameField;
    public InputField emailField;
    public InputField passwordField;

    private ControllerScript controller;
    // Use this for initialization
    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerScript>();
        passwordField.contentType = InputField.ContentType.Password;
        emailField.contentType = InputField.ContentType.EmailAddress;

    }

    /// <summary>
    /// Save all the info
    /// </summary>
    public void Create()
    {
        
        string nameFieldInput = nameField.transform.Find("Text").GetComponent<Text>().text;
        string emailFieldInput = emailField.transform.Find("Text").GetComponent<Text>().text;
        string passwordFieldInput = passwordField.transform.Find("Text").GetComponent<Text>().text;

        int index = -1;
        for (int i = 0; i < controller.userNames.Count; i++)
        {
            if (nameFieldInput == (string)controller.userNames[i])
            {
                index = i;
                break;
            }
        }
        if (index == -1)
        {
            controller.userNames.Add(nameFieldInput);
            controller.userEmails.Add(emailFieldInput);
            controller.userPasswords.Add(passwordFieldInput);
            controller.names.Add("");
            controller.surnames.Add("");
            controller.adresses.Add("");
            controller.phoneNumbers.Add("");
            controller.zipCodes.Add("");
            controller.currentUser = controller.userNames.Count - 1;
            SceneManager.LoadScene("AddAccountDetailsScene");
        }
        else
        {
            SceneManager.LoadScene("CreateAccountScene");
        }
    }

}
