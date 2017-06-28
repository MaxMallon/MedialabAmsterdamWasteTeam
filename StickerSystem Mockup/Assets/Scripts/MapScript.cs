using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This script displays icons on the map based on real gps data.
/// </summary>
public class MapScript : MonoBehaviour {

    private ControllerScript controller;

    public Vector2 OnScreenUpperLeftCorner, OnScreenLowerRightCorner; //Two locations that indicate the two opposite corners of the map as it's displayed on the screen.
    public double upperLeftLatitude, upperLeftLongitude, lowerRightLatitude, lowerRightLongitude; //The coordinates that corrospond to the locations the 2 corners occupy. 
    Vector2 onScreenSize, realSize, pixelSize; //Used for calculation.
    double deltaLatitude, deltaLongitude, deltaWidth, deltaHeight; //lati = horizontal lines, so vertical position, longi = vertical lines, so horizontal position.

    public GameObject UpperLeftCorner, LowerRightCorner; //Two objects that the corner locations are extracted from, makes it easier to work with in the editor. 
    public GameObject Placementmarker; //Prefab of the icons.

    public int type;
    // Use this for initialization
    void Start()
    {
        OnScreenUpperLeftCorner = UpperLeftCorner.GetComponent<RectTransform>().position; //Get the 2 locations
        OnScreenLowerRightCorner = LowerRightCorner.GetComponent<RectTransform>().position;

        controller = GameObject.Find("Controller").GetComponent<ControllerScript>();
        onScreenSize = OnScreenLowerRightCorner - OnScreenUpperLeftCorner; //Calculate how much 1 pixel is in coordinates in the next couple of liens. 
        deltaLatitude = lowerRightLatitude - upperLeftLatitude;
        deltaLongitude = lowerRightLongitude - upperLeftLongitude;
        deltaWidth = OnScreenLowerRightCorner.x - OnScreenUpperLeftCorner.x;
        deltaHeight = OnScreenLowerRightCorner.y - OnScreenUpperLeftCorner.y;

        pixelSize.x = (float) (deltaWidth / deltaLongitude);
        pixelSize.y = (float) (deltaHeight / deltaLatitude);


        controller = GameObject.Find("Controller").GetComponent<ControllerScript>();
        for (int i = 0; i < controller.miniGPSDatabase.Length; i++)
        {
            if (controller.miniGPSDatabase[i] != null  && controller.miniGPSDatabase[i] != "")
            {
                PlaceMarker(controller.miniGPSDatabase[i]); //Go through all the scanned stickers, and place a marker for each one of them.
            }
        }
        GPSLocation(); //Doesn't work because I don't even know why
        //PlaceMarker("52.360345 4.873639"); a test line.
    }


    /// <summary>
    /// Gets the user's current location and places a marker.
    /// </summary>
    void GPSLocation()
    {
        string stringer = "";

        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            return;
        }

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 3000;

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            stringer = ("Timed out");
            controller.GetComponent<ControllerScript>().miniGPSDatabase[GameObject.Find("Sticker").GetComponent<ScannedStickersScript>().type] = stringer;
            Input.location.Stop();
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            stringer = ("Unable to determine device location");
            controller.GetComponent<ControllerScript>().miniGPSDatabase[GameObject.Find("Sticker").GetComponent<ScannedStickersScript>().type] = stringer;
            Input.location.Stop();
        }
        else
        {
            // Access granted and location value could be retrieved
            stringer = (Input.location.lastData.latitude + " " + Input.location.lastData.longitude);
            controller.GetComponent<ControllerScript>().miniGPSDatabase[GameObject.Find("Sticker").GetComponent<ScannedStickersScript>().type] = stringer;
            PlaceMarker(stringer);
            Input.location.Stop();
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }


    /// <summary>
    /// Places a marker at coordinates
    /// </summary>
    /// <param name="coordinates">Coordintes of the place that needs to be marked</param>
    public void PlaceMarker(Vector2 coordinates)
    {
        GameObject marker = GameObject.Instantiate(Placementmarker, transform.position, Quaternion.identity, transform);
        marker.GetComponent<RectTransform>().position = SetCoordinates(coordinates);
    }

    /// <summary>
    /// Places a marker at coordinates
    /// </summary>
    /// <param name="coordinates">Coordintes of the place that needs to be marked, as a string, with a space in between</param>
    public void PlaceMarker(string coordinates)
    {
        GameObject marker = GameObject.Instantiate(Placementmarker, transform.position, Quaternion.identity, transform);
        marker.GetComponent<RectTransform>().position = SetCoordinates(coordinates);
    }


    /// <summary>
    /// Changes actual coordinates into onscreen location.
    /// </summary>
    /// <param name="coordinates">Actual coordinates</param>
    /// <returns>Location on screen</returns>
    public Vector2 SetCoordinates(Vector2 coordinates)
    {
        //TODO don't worry, nothing todo here, but this way of delcaring an object is useful as fuck.
        Vector2 position = new Vector2()
        {
            x = (float)(OnScreenUpperLeftCorner.x + pixelSize.x * (coordinates.x - upperLeftLongitude)),
            y = (float)(OnScreenUpperLeftCorner.y + pixelSize.y * (coordinates.y - upperLeftLatitude)) 
        };
        return position;
    }

    /// <summary>
    /// Changes actual coordinates into onscreen location.
    /// </summary>
    /// <param name="coordinates">Actual coordinates as a string with a space inbetween</param>
    /// <returns>Location on screen</returns>
    public Vector2 SetCoordinates(string coordinates)
    {
        double[] doubleArray = ReadCoordinates(coordinates);

        Vector2 position = new Vector2()
        {
            x = (float)(OnScreenUpperLeftCorner.x + pixelSize.x * ((float)(doubleArray[0] - upperLeftLongitude))),
            y = (float)(OnScreenUpperLeftCorner.y + pixelSize.y * ((float)(doubleArray[1] - upperLeftLatitude)))
        };
        return position;
    }


    /// <summary>
    /// Changes a string of coordinates into a double array
    /// </summary>
    /// <param name="stringer">Coordinates in a string with a space in between</param>
    /// <returns>a double array with the 2 coordinates, lattitude and longitude </returns>
    public double[] ReadCoordinates(string stringer)
    {
        string[] array = stringer.Split(' ');
        double[] doubleArray = new double[2];
        double.TryParse(array[0], out doubleArray[1]);
        double.TryParse(array[1], out doubleArray[0]);

        return doubleArray;
    }
}
