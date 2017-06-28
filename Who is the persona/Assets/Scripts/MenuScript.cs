using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The script to be executed in the main menu.
/// </summary>
public class MenuScript : MonoBehaviour {

    public GameObject questionText; //The text that tells the player everything.
    bool onePress = false;

    /// <summary>
    /// Go to the game, or quit.
    /// </summary>
    void Update () {

        if (Input.GetKeyDown(KeyCode.Space) && !onePress)
        {
            StartCoroutine(Play());
            onePress = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }

    private IEnumerator Play()
    {
        questionText.GetComponent<Text>().text = "The computer will asign a random persona to you\nTry to Guess Who!";
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene("transitionScreen");
    }
}
