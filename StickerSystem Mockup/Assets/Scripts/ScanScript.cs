using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanScript : MonoBehaviour {

   
    public QRCodeDecodeController qrcodecontroller;
    string dataText = "";


    private void Start()
    {
        qrcodecontroller.onQRScanFinished += getResult;// write this code in start()
        qrcodecontroller.useGUILayout = true;

    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, Screen.width, Screen.height / 10), "Reset"))
        {
            Reset();
        }
        GUI.Box(new Rect(0, Screen.height / 10, Screen.width, Screen.height / 10), "");
        GUI.Label(new Rect(0, Screen.height / 10, Screen.width, Screen.height / 10), dataText);
    }

    void getResult(string resultStr)
    {
        print(resultStr);
        dataText = resultStr;
    }

    private void Reset()
    {
        qrcodecontroller.Reset();
    }
}
