using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// A script for the transition screen.
/// </summary>
public class TransitionScreenScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(LoadNext());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// loads the playfield
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadNext()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene("playField");
    }
}
