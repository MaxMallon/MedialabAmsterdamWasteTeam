using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;


/// <summary>
/// This script handles what happens when a button is pressed.
/// </summary>
public class ButtonScript : MonoBehaviour {


    //The button types needed.
    public enum ButtonTypes
    {
        Scan, Find, Buy, Info, Login, Stats, Map, Home, WhatIsBulkyWaste, NeedHelp, CatalogueNormal, CatalogueElectronic, Back, Activate, Deactivate,
        Done, Next, DoneAfterScan, BuyNormal, BuyElectronic, StickerButton, BuyOnline, LogInOnLogin, CreateAccount, EditProfile, CreateAccountConfirm, Skip, OK, CatalogueResult, CompleteOrder, Logout, Row1, Row2
    };
    public ButtonTypes buttonType; //The buttontype the specific button has.

    public Sprite[] sprites;//pretty sure I don't use this.
    
    private ControllerScript controller;


    // Use this for initialization
    void Awake ()
    {
        controller = GameObject.Find("Controller").GetComponent<ControllerScript>();
        if (GetComponent<Button>() != null)
                GetComponent<Button>().onClick.AddListener(() => { ButtonClick(); });
    }

    private void Start()
    {

        controller = GameObject.Find("Controller").GetComponent<ControllerScript>();
    }
    /// <summary>
    /// Let the GameScript do stuff when pressed, what, differs per button.
    /// </summary>
    void ButtonClick()
    {
        switch (buttonType)
        {
            case ButtonTypes.Scan:
                controller.AddPreviousScene();
                SceneManager.LoadScene("QRScanScene");
                break;
            case ButtonTypes.Find:
                controller.AddPreviousScene();
                SceneManager.LoadScene("CatalogueScene0");
                break;
            case ButtonTypes.Buy:
                controller.AddPreviousScene();
                SceneManager.LoadScene("BuyScene");
                break;
            case ButtonTypes.Info:
                controller.AddPreviousScene();
                SceneManager.LoadScene("InfoScene");
                break;
            case ButtonTypes.Map:
                controller.AddPreviousScene();
                SceneManager.LoadScene("MapScene");
                break;
            case ButtonTypes.Stats:
                SceneManager.LoadScene("StatsScene");
                break;
            case ButtonTypes.Login:
                controller.AddPreviousScene();
                if (controller.currentUser == -1)
                {
                    SceneManager.LoadScene("LoginScene");
                }
                else
                {
                    SceneManager.LoadScene("AccountScene");
                }
                break;
            case ButtonTypes.Home:
                controller.AddPreviousScene();
                SceneManager.LoadScene("HomeScene");
                break;
            case ButtonTypes.WhatIsBulkyWaste:
                controller.AddPreviousScene();
                SceneManager.LoadScene("FindStickerScene");
                break;
            case ButtonTypes.NeedHelp:
                controller.AddPreviousScene();
                SceneManager.LoadScene("FindStickerScene2");
                break;
            case ButtonTypes.CatalogueNormal:
                controller.catalogueElectronic = false;
                controller.AddPreviousScene();
                SceneManager.LoadScene("CatalougeScene2");
                break;
            case ButtonTypes.CatalogueElectronic:
                controller.catalogueElectronic = true;
                controller.AddPreviousScene();
                SceneManager.LoadScene("CatalougeScene2");
                break;
            case ButtonTypes.Back:
                controller.gotoPreviousScene();
                break;
            case ButtonTypes.Activate:
                controller.GetComponent<ControllerScript>().miniDateDatabase[GameObject.Find("Sticker").GetComponent<ScannedStickersScript>().type] = DateTime.Now.ToString();
                GPSLocation();
                break;
            case ButtonTypes.Deactivate:
                controller.GetComponent<ControllerScript>().miniDateDatabase[GameObject.Find("Sticker").GetComponent<ScannedStickersScript>().type] = null;
                SceneManager.LoadScene("ScannedItemScene");
                break;
            case ButtonTypes.DoneAfterScan:
                SceneManager.LoadScene("ScannedItemScene");
                break;
            case ButtonTypes.Next:
                Next();
                break;
            case ButtonTypes.Done:
                SceneManager.LoadScene("ScannedItemScene");
                break;
           
            case ButtonTypes.BuyNormal:
                controller.buyElectronic = false;
                controller.AddPreviousScene();
                SceneManager.LoadScene("BuyScene2");
                break;
            case ButtonTypes.BuyElectronic:
                controller.buyElectronic = true;
                controller.AddPreviousScene();
                SceneManager.LoadScene("BuyScene2");
                break;
            case ButtonTypes.StickerButton:
                controller.AddPreviousScene();
                SceneManager.LoadScene("BuyScene3");
                break;
            case ButtonTypes.BuyOnline:
                controller.AddPreviousScene();
                SceneManager.LoadScene("BuyScene4"); //How do I get to the scene where all the naked people have sex? LoadScene("ObScene");
                break;
            case ButtonTypes.LogInOnLogin:
                GetComponent<LogInScript>().Login();
                break;
            case ButtonTypes.CreateAccountConfirm:
                GetComponent<CreateAccountScript>().Create();
                break;
            case ButtonTypes.CreateAccount:
                controller.AddPreviousScene();
                SceneManager.LoadScene("CreateAccountScene");
                break;
            case ButtonTypes.Skip:
                SceneManager.LoadScene("AccountScene");
                break;
            case ButtonTypes.OK:
                GetComponent<AddDetailsScript>().AddDetails();
                break;
            case ButtonTypes.EditProfile:
                controller.AddPreviousScene();
                SceneManager.LoadScene("AddAccountDetailsScene");
                break;
            case ButtonTypes.CatalogueResult:
                controller.AddPreviousScene();
                controller.selectedSticker = (int)GetComponent<StickerScript>().type;
                SceneManager.LoadScene("BuyScene3");
                break;
            case ButtonTypes.CompleteOrder:
                SceneManager.LoadScene("BuyScene5");
                break;
            case ButtonTypes.Logout:
                controller.currentUser = -1;
                SceneManager.LoadScene("HomeScene");
                break;
            case ButtonTypes.Row1:
                controller.findStickerScene[0] = controller.catalogueElectronic;
                controller.findStickerScene[2] = false;
                SceneManager.LoadScene("CatalogueScene3");
                break;
            case ButtonTypes.Row2:
                controller.findStickerScene[0] = controller.catalogueElectronic;
                controller.findStickerScene[2] = true;
                SceneManager.LoadScene("CatalogueScene3");
                break;
        }
    }

    /// <summary>
    /// Next button has a different function on each scene.
    /// </summary>
    void Next()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "InfoScene":
                controller.AddPreviousScene();
                SceneManager.LoadScene("InfoScene2");
                break;
            case "FindStickerScene":
                controller.AddPreviousScene();
                SceneManager.LoadScene("FindStickerScene2");
                break;
            case "FindStickerScene2":
                controller.AddPreviousScene();
                SceneManager.LoadScene("CatalogueScene1");
                break;
            case "CatalogueScene1":
                controller.AddPreviousScene();
                GetComponent<FindStickerScript>().Find();
                break;
        }
    }

    /// <summary>
    /// Saves the GPS location
    /// </summary>
    void GPSLocation()
    {
        string stringer = "";

        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            SceneManager.LoadScene("ActivateScene");
            return;
        }

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 3000; //waitfornewseconds didn't work for some reason.
        
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            maxWait--;
        }

        print("c");
        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            stringer = ("Timed out");
            controller.GetComponent<ControllerScript>().miniGPSDatabase[GameObject.Find("Sticker").GetComponent<ScannedStickersScript>().type] = stringer;
            Input.location.Stop();
            SceneManager.LoadScene("ScannedItemScene");
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            stringer = ("Unable to determine device location");
            controller.GetComponent<ControllerScript>().miniGPSDatabase[GameObject.Find("Sticker").GetComponent<ScannedStickersScript>().type] = stringer;
            Input.location.Stop();
            SceneManager.LoadScene("ScannedItemScene");
        }
        else
        {
            // Access granted and location value could be retrieved
            stringer = (Input.location.lastData.latitude + " " + Input.location.lastData.longitude);
            controller.GetComponent<ControllerScript>().miniGPSDatabase[GameObject.Find("Sticker").GetComponent<ScannedStickersScript>().type] = stringer;
            Input.location.Stop();
            SceneManager.LoadScene("ScannedItemScene");
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }
}
