using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Plays the video in the load screen.
/// </summary>
public class VideoLoadScript : MonoBehaviour {

    VideoPlayer video;
    public GameObject canvas;
    public GameObject light;
	// Use this for initialization
	void Start () {
        video = GetComponent<VideoPlayer>();
        StartCoroutine(VideoStop());
	}
	
	// Update is called once per frame
	IEnumerator VideoStop () {
        yield return new WaitForSeconds(5);
        canvas.SetActive(true);
        light.SetActive(false);
        this.gameObject.SetActive(false);
       
	}
}
