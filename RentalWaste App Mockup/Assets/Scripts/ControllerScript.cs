using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerScript : MonoBehaviour {

    public int mode = 0;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        Screen.SetResolution(608, 1080, true);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
