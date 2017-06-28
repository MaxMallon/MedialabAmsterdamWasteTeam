using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Should have been called 'FakeLoadScript', but oh well. It plays the animation, and afterwards allows the user to go to the main scene.
/// </summary>
public class LoadFakeScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        //StartCoroutine(FakeLoad());
    }

    /// <summary>
    /// Wait till the animation is done playing.
    /// </summary>
    /// <returns></returns>
    IEnumerator FakeLoad()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("HomeScene");
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            SceneManager.LoadScene("HomeScene");
        }
    }
}
