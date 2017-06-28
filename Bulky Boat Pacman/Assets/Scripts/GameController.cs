using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/// <summary>
/// This is the game controller, it stays loaded and does stuff like score.
/// </summary>
public class GameController : MonoBehaviour {

    int score = 0;
    public int dotScore = 100;
    public int superDotScore = 1000;
    public int[] fruitScores = new int[5];
    public int[] ghostReleaseTimes = new int[4];
    public GameObject[] ghosts = new GameObject[4];
    public int level;
    public int lives;

    public int startLives;
    public int newLifeScoreInterval;

    public int ghostEatScore;
    public ArrayList nodes = new ArrayList();
    public ArrayList yumyumDots = new ArrayList();
    public Vector3 cameraPosition;

    public Text text;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start ()
    {
        lives = startLives;
        cameraPosition = GameObject.Find("Camera").transform.position;
        StartCoroutine(SpawnFruit());
        if (GameObject.Find("GameController") != gameObject)
        {
            GameObject.Destroy(gameObject);
        }
        
    }


    /// <summary>
    /// To be executed every time the level spawns. Basically 'Start()' for an object that stays loaded.
    /// </summary>
    /// <param name="level"></param>
    private void OnLevelWasLoaded(int level)
    {
        nodes = new ArrayList();
        yumyumDots = new ArrayList();
        if (SceneManager.GetActiveScene().name == "TitleScreen")
        {
            GameObject.Destroy(gameObject);
        }
        for (int i = 0; i < 4; i++)
        {
            StartCoroutine(ReleaseGhost(i, ghostReleaseTimes[i]));
        }
        text = GameObject.Find("Text").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update () {
        text.text = "Score = " + score + "\nLives = " + lives + "\nLevel = " + level;
		
	}

    /// <summary>
    /// Release the ghosts.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator ReleaseGhost(int index, float time)
    {
        yield return new WaitForSeconds(time);
        if (index == 0 || !ghosts[index - 1].GetComponent<GhostBehaviour>().startPhase)
        {
            ghosts[index].GetComponent<GhostBehaviour>().startPhase = false;
        }
    }


    /// <summary>
    /// Spawn a fruit.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnFruit()
    {
        yield return new WaitForSeconds(Random.Range(25,50));
        for (int i = 0; i < nodes.Count; i++)
        {
            GameObject nodeEval = nodes[i] as GameObject;
            nodeEval.GetComponent<NodeScript>().SpawnStuffNoDots(true);
        }
        StartCoroutine(SpawnFruit());
    }

    public int FruitScores
    {
        get
        {
            return fruitScores[level % fruitScores.Length];
        }
    }

    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            float a = score % newLifeScoreInterval;
            score = value;
            float b = score % newLifeScoreInterval;
            if (b < a)
            {
                lives++;
            }
        }
    }

    /// <summary>
    /// What should happen if all the dots are gone.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Win()
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].GetComponent<MovementBehaviour>().enabled = false;
        }
        yield return new WaitForSeconds(2);
        level++;
        SceneManager.LoadScene("CrookedPacman");
    }

    /// <summary>
    /// What should happen when the player dies.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Die()
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].GetComponent<MovementBehaviour>().enabled = false;
        }
        yield return new WaitForSeconds(2);
        lives--;
        if (lives >= 0)
        {
            for (int i = 0; i < ghosts.Length; i++)
            {
                GameObject.Destroy(ghosts[i]);
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                GameObject nodeEval = nodes[i] as GameObject;
                nodeEval.GetComponent<NodeScript>().SpawnStuffNoDots(false);
            }

            for (int i = 0; i < 4; i++)
            {
                StartCoroutine(ReleaseGhost(i, ghostReleaseTimes[i]));
            }
            StartCoroutine(SpawnFruit());
        }
        else
        {
            SceneManager.LoadScene("TitleScreen");

        }

    }
    /// <summary>
    /// Tell the ghosts the player ate a super dot.
    /// </summary>
    public void SuperDot()
    {
        for (int i = 0; i < 4; i++)
        {

            ghosts[i].GetComponent<GhostBehaviour>().PlayerAteSuperDot();
        }
    }

    /// <summary>
    /// Finds node closest to a location.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public GameObject FindNode(Vector2 position)
    {
        float closest = float.MaxValue;
        int closestIndex = -1;
        for (int i = 0; i < nodes.Count; i++)
        {
            GameObject nodeEval = nodes[i] as GameObject;
            float thisDistance = Vector2.Distance(nodeEval.transform.position, position);

            if (thisDistance < closest && !nodeEval.GetComponent<NodeScript>().ignore)
            {
                closest = thisDistance;
                closestIndex = i;
            }

        }
        GameObject node = nodes[closestIndex] as GameObject;
        return node;
    }
}
