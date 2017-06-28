using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Behaviour unique to the player, aka, being controlled and eating and such.
/// </summary>
public class PlayerBehaviour : MonoBehaviour
{

    public GameController game;

    public Sprite[] sprites;

    // Use this for initialization
    void Awake()
    {
        game = GameObject.Find("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        ButtonInput();
       
    }

    /// <summary>
    /// Parse the button in put, and make it work with the grid movement.
    /// </summary>
    void ButtonInput()
    {
        MovementBehaviour.Directions temp = GetComponent<MovementBehaviour>().directionKey;
        MovementBehaviour movement = GetComponent<MovementBehaviour>();
        bool tryChange = false;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            temp = MovementBehaviour.Directions.Up;
            tryChange = true;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            temp = MovementBehaviour.Directions.Down;
            tryChange = true;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            temp = MovementBehaviour.Directions.Left;
            tryChange = true;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            temp = MovementBehaviour.Directions.Right;
            tryChange = true;
        }
        if (tryChange)
            movement.TryChangeDirections(temp, true);
    }

    
    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "NormalFood":
                game.yumyumDots.Remove(other.gameObject);
                GameObject.Destroy(other.gameObject);
                game.Score += game.dotScore;
                if (game.yumyumDots.Count == 0)
                {
                    GetComponent<MovementBehaviour>().enabled = false;
                    game.StartCoroutine(game.Win());
                }
                break;
            case "SuperFood":
                GameObject.Destroy(other.gameObject);
                game.SuperDot();
                game.Score += game.superDotScore;
                break;
            case "PointsFood":
                GameObject.Destroy(other.gameObject);
                game.Score += game.FruitScores;                
                break;
            case "Ghost":
                if (other.GetComponent<GhostBehaviour>().blinking)
                {

                    other.GetComponent<GhostBehaviour>().GotEaten();
                    game.Score += game.ghostEatScore;
                }
                else if (!other.GetComponent<GhostBehaviour>().eaten)
                {
                    StartCoroutine(Die(0));
                    game.StartCoroutine(game.Die());
                }
                break;
        }
    }


    /// <summary>
    /// The player that is. yes you.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    IEnumerator Die(int i)
    {
        GetComponent<MovementBehaviour>().enabled = false;


        GetComponent<SpriteRenderer>().sprite = sprites[i];
        yield return new WaitForSeconds(0.3f);
        if (i < sprites.Length - 1)
        {
            StartCoroutine(Die(++i));
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
        //gameover and stuff
    }
}
