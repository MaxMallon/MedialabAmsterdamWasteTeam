using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/// <summary>
/// Allows the user to log in.
/// </summary>
public class LogInScript : MonoBehaviour {

    public InputField nameField;
    public InputField passwordField;

    private ControllerScript controller;
    // Use this for initialization
    void Start () {
        controller = GameObject.Find("Controller").GetComponent<ControllerScript>();
        passwordField.contentType = InputField.ContentType.Password;

    }

    /// <summary>
    /// Tries to log in the user with the data they entered
    /// But can only do so if that account actually exists.
    /// </summary>
    public void Login () {
		
        string namefieldInput = nameField.transform.Find("Text").GetComponent<Text>().text;
            string passwordFieldInput = passwordField.transform.Find("Text").GetComponent<Text>().text;

            int index = -1;
            for (int i = 0; i < controller.userNames.Count; i++)
            {
                if (namefieldInput == (string)controller.userNames[i])
                {
                    index = i;
                    break;
                }
            }
            if (index != -1 && (string)controller.userPasswords[index] == passwordFieldInput)
            {
                controller.currentUser = index;
                SceneManager.LoadScene("AccountScene");
            }
            else
            {
                SceneManager.LoadScene("LoginScene");
            }
        }
	
}
