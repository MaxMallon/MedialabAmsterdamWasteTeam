using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// This is the controller.
/// It stays loaded for as long as the game is turned on.
/// It contains all the information that should be maintained accross scenes, such as previous pages and entered info.
/// </summary>
public class ControllerScript : MonoBehaviour {

    public int mode = 0;
    public string scannedText = ""; //To maintain the text scanned from QR codes.
    public string[] miniDateDatabase; //To store the dates stickers are scanned on, index = sticker number
    public string[] miniGPSDatabase; //To store gps location where stickers are scanned, index = sticker number.
    public bool[] findStickerScene; //Whether or not displayed data is on the findstickerscene.

    public ArrayList userNames;
    public ArrayList userPasswords;
    public ArrayList userEmails;
    public ArrayList adresses;
    public ArrayList zipCodes;
    public ArrayList surnames;
    public ArrayList names;
    public ArrayList phoneNumbers;

    public int currentUser = -1; //the user logged in, index used in the arraylists, -1 is not logged in
    public int selectedSticker; //Sticker that the user selected to purchase

    public string[] previousScene; //List of scenes the user visited earlier.
    public bool catalogueElectronic = false; //Whether the catalogue should display electronic stickers or not.
    public bool buyElectronic = false; //Whether the buyscene should display elecctronic stickers or not.

    void Awake()
    {
        previousScene = new string[8];
        findStickerScene = new bool[3];
        userNames = new ArrayList();
        userPasswords = new ArrayList();
        userEmails = new ArrayList();
        adresses = new ArrayList();
        zipCodes = new ArrayList();
        surnames = new ArrayList();
        names = new ArrayList();
        phoneNumbers = new ArrayList();
        DontDestroyOnLoad(transform.gameObject);
       // Screen.SetResolution(1920, 1200, true); //Was used to set the resolution right, should not be used actually.
        miniDateDatabase = new string[8];
        miniGPSDatabase = new string[8];
        for (int i = 0; i < miniDateDatabase.Length; i++)
        {
            miniDateDatabase[i] = null;
            miniGPSDatabase[i] = null;
        }
    }
    
    /// <summary>
    /// Why didn't I just use a stack?
    /// </summary>
    public void AddPreviousScene()
    {
        previousScene[7] = previousScene[6];
        previousScene[6] = previousScene[5];
        previousScene[5] = previousScene[4];
        previousScene[4] = previousScene[3];
        previousScene[3] = previousScene[2];
        previousScene[2] = previousScene[1];
        previousScene[1] = previousScene[0];
        previousScene[0] = SceneManager.GetActiveScene().name;
    }


/// <summary>
/// I made my own stack.
/// </summary>
   public void gotoPreviousScene()
    {
        string scene = previousScene[0];
        previousScene[0] = previousScene[1];
        previousScene[1] = previousScene[2];
        previousScene[2] = previousScene[3];
        previousScene[3] = previousScene[4];
        previousScene[4] = previousScene[5];
        previousScene[5] = previousScene[6];
        previousScene[6] = previousScene[7];

        SceneManager.LoadScene(scene);

    }
}
