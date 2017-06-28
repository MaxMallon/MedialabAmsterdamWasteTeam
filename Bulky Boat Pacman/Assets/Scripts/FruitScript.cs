using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script makes sure the fruit doesn't stay forever.
/// </summary>
public class FruitScript : MonoBehaviour {

    public Sprite[] sprites;
    public GameController game;

	// Use this for initialization
	void Start () {
        game = GameObject.Find("GameController").GetComponent<GameController>();
        GetComponent<SpriteRenderer>().sprite = sprites[game.level % game.fruitScores.Length];
        StartCoroutine(KillYourself());
		
	}
	

    /// <summary>
    /// ...do it.
    /// 
    /// It removes the fruit after a while.
    /// </summary>
    /// <returns></returns>
    private IEnumerator KillYourself()
    {
        yield return new WaitForSeconds(10);
        GameObject.Destroy(gameObject);
    }
}
