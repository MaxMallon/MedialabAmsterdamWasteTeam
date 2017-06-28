using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// All the code for what ghosts should do.
/// </summary>
public class GhostBehaviour : MonoBehaviour {

    public enum GhostType { Chaser, Aheader, Roamer, SlowChaser} //Their AI types.
    public GhostType type; //Their actual type per instance.
    private MovementBehaviour.Directions direction; //Their walking behaviour.
    public NodeScript target, currentStartNode; //Where they are headed, and last node they visted.
    public NodeScript homeNode; //Where they spawned.
    ArrayList path; //The path of nodes to the destination.
    GameController game;
    GameObject player;
    public bool blinking;
    public bool eaten;
    public bool startPhase;
    public float edibleSpeed;
    public float normalSpeed;
    public float eatenSpeed;

    private bool startedYet = false;


    public Sprite[] sprites;


    // Use this for initialization
    void Start() {

        direction = MovementBehaviour.Directions.Up;
        target = null;
        blinking = false;
        eaten = false;
        startPhase = true;
        path = new ArrayList();
        player = GameObject.FindGameObjectWithTag("Player");
        startedYet = true;
        game = GameObject.Find("GameController").GetComponent<GameController>();
        DeterimeSprite();
        StartCoroutine(StartMoving());
    }

    /// <summary>
    /// Start moving.
    /// </summary>
    /// <returns></returns>
    IEnumerator StartMoving()
    {
        yield return new WaitForSeconds(1);
        direction = MovementBehaviour.Directions.Up;
        SendDirection();
    }

    /// <summary>
    /// Update the direction based on where the AI sends them.
    /// </summary>
    public void UpdateDirection() {
        if (!startPhase)
        {
            if (!blinking && !eaten)
            {
                switch (type)
                {
                    case GhostType.Chaser: //moves to where the player last was.
                        target = player.GetComponent<MovementBehaviour>().lastNode;
                        path = NavigateToTarget();
                        direction = ActOnPath();
                        break;
                    case GhostType.Aheader: //moves to where the player will be shortly
                        target = player.GetComponent<MovementBehaviour>().nextNode;
                        if (target == null)
                        {
                            target = player.GetComponent<MovementBehaviour>().lastNode;
                        }
                        path = NavigateToTarget();
                        direction = ActOnPath();
                        break;
                    case GhostType.Roamer: //moves random
                        if (path.Count == 1)
                        {
                            do
                            {
                                GameObject targetObject = game.nodes[(int)Mathf.Round(Random.Range(0, game.nodes.Count))] as GameObject; //komt geen goede antwoorden uit
                                target = targetObject.GetComponent<NodeScript>();
                            } while (target.ignore || target == currentStartNode);
                            path = NavigateToTarget();
                        }
                        direction = ActOnPath();
                        break;
                    case GhostType.SlowChaser: //moves to where the player last was, and actually does the whole route.
                        target = player.GetComponent<MovementBehaviour>().lastNode;
                        if (target == currentStartNode)
                        {
                            target = player.GetComponent<MovementBehaviour>().nextNode;
                        }
                        if (path.Count == 0) //if there is no path, then go there.
                        {
                            path = NavigateToTarget();
                        }
                        direction = ActOnPath();

                        break;
                }
            }
            else if (blinking)
            {
                target = game.FindNode(-(player.transform.position - game.cameraPosition)).GetComponent<NodeScript>();
                path = NavigateToTarget();
                direction = ActOnPath();
                SendDirection();
            }
            else
            {
                ReturnToBase();
                if (path.Count == 0)
                {
                    startPhase = true;
                    direction = MovementBehaviour.Directions.Up;
                    eaten = false;
                    blinking = false;
                    GetComponent<MovementBehaviour>().speed = normalSpeed;
                    DeterimeSprite();
                    StartCoroutine(ReturnToNormal(true));
                }
            }
        }
        else
        {
            if (direction == MovementBehaviour.Directions.Up)
            {
                direction = MovementBehaviour.Directions.Down;
            }
            else
            {
                direction = MovementBehaviour.Directions.Up;
            }
        }
		
	}

    /// <summary>
    /// Finds the direction to take to get to the 1st node on the path.
    /// </summary>
    /// <returns></returns>
    private MovementBehaviour.Directions ActOnPath()
    {
        MovementBehaviour.Directions direction = MovementBehaviour.Directions.None;
        if (path.Count > 0)
        {
            NodeScript nodeToGoTo = path[0] as NodeScript;
            for (int i = 0; i < currentStartNode.neighbours.Length; i++)
            {
                if (nodeToGoTo == currentStartNode.neighbours[i].GetComponent<NodeScript>())
                {
                    direction = currentStartNode.neighbourDirections[i];
                }
            }
        }
        else
        {
            direction = currentStartNode.neighbourDirections[0];
        }
        return direction;
    }

  



    /// <summary>
    /// Pahtfinding using A*
    /// </summary>
    /// <returns>an arraylist with all the nodes to get to the target</returns>
    private ArrayList NavigateToTarget()
    {
        if (target == null)
        {
            return new ArrayList();
        }

        ArrayList closed = new ArrayList();
        ArrayList open = new ArrayList();

        NodeScript start = currentStartNode;
        start.gScore = 0;
        start.fScore = start.DistanceEstimate(start, target);
        open.Add(start.gameObject);
        
        while (open.Count > 0)
        {
            NodeScript currentNode;
            float shortestDistance = float.MaxValue;
            int shortestNeighbourIndex = 0;
            for (int i = 0; i < open.Count; i++)
            {
                GameObject nodeEval = open[i] as GameObject;
                nodeEval.GetComponent<NodeScript>().fScore = nodeEval.GetComponent<NodeScript>().DistanceEstimate(nodeEval.GetComponent<NodeScript>(), target);
                if (nodeEval.GetComponent<NodeScript>().fScore < shortestDistance)
                {
                    shortestDistance = nodeEval.GetComponent<NodeScript>().fScore;
                    shortestNeighbourIndex = i;
                }
            }
            GameObject node = (GameObject)open[shortestNeighbourIndex] as GameObject;
            currentNode = node.GetComponent<NodeScript>();

            if (currentNode == target)
            {
                ArrayList path = ReconstructPath(currentNode);
                //Restoring the infinity values to be better
                for (int i = 0; i < game.nodes.Count; i++)
                {
                    GameObject nodeEval = (GameObject)game.nodes[i] as GameObject;
                    nodeEval.GetComponent<NodeScript>().fScore = float.MaxValue;
                    nodeEval.GetComponent<NodeScript>().gScore = float.MaxValue;
                    nodeEval.GetComponent<NodeScript>().previousNode = null;
                }
                
                return path;

            }
            open.Remove(currentNode.gameObject);
            closed.Add(currentNode.gameObject);
                       
            for (int i = 0; i < currentNode.neighbours.Length; i++)
            {
                NodeScript neighbour = currentNode.neighbours[i].GetComponent<NodeScript>();
                if (closed.Contains(neighbour.gameObject))
                {
                    continue;
                }
                float tenativeGScore = currentNode.gScore + currentNode.distances[i];
                if (!open.Contains(neighbour.gameObject))
                {
                    open.Add(neighbour.gameObject);
                }
                else if (tenativeGScore >= neighbour.gScore)
                {
                    continue;
                }

                neighbour.previousNode = currentNode;
                neighbour.gScore = tenativeGScore;
                neighbour.fScore = neighbour.gScore + neighbour.DistanceEstimate(neighbour, target);

            }
        }
        return new ArrayList(); //FAILURE
    }

    /// <summary>
    /// Get the actual path.
    /// </summary>
    /// <param name="currentNode"></param>
    /// <returns></returns>
    private ArrayList ReconstructPath(NodeScript currentNode)
    {
        ArrayList totalPath = new ArrayList();
        //totalPath.Add(currentNode);

        while (currentNode.previousNode != null)
        {
            
            totalPath.Add(currentNode);
            currentNode = currentNode.previousNode;
        }
        totalPath.Reverse();
        return totalPath;
    }
    

    /// <summary>
    /// Checks to see if the current direction is possible on the node
    /// </summary>
    /// <returns></returns>
    private bool CheckDirectionWithNode()
    {
        bool val = false;
        for (int i = 0; i < GetComponent<MovementBehaviour>().nextNode.neighbourDirections.Length; i++)
        {
            if (GetComponent<MovementBehaviour>().nextNode.neighbourDirections[i] == direction)
            {
                val = true;
            }
        }
        return val;
    }

    /// <summary>
    /// Spam/send the direction to the movement script.
    /// </summary>
    void SendDirection()
    {
        GetComponent<MovementBehaviour>().TryChangeDirections(direction, false);
    }

    /// <summary>
    /// update the direction to send.
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Node" && startedYet)
        {
            
            if (other.GetComponent<NodeScript>() != currentStartNode)//TODO may cause problerms if movement is executed first.
            {
                currentStartNode = other.GetComponent<NodeScript>();
                
                if (path.Count > 0)
                {
                    path.RemoveAt(0);
                }
                UpdateDirection();
            }
        }
    }

    /// <summary>
    /// And send the direction once more.
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Node" && startedYet)
        {
            SendDirection();
        }
    }
    /// <summary>
    /// Start blinking
    /// </summary>
    public void PlayerAteSuperDot()
    {
        if (!startPhase)
        {
            blinking = true;
            UpdateDirection();
            GetComponent<MovementBehaviour>().speed = edibleSpeed;
            StartCoroutine(ReturnToNormal(false));
            GetComponent<SpriteRenderer>().sprite = sprites[4];
        }
    }

    /// <summary>
    /// Become eyes, and go home.
    /// </summary>
    public void GotEaten()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[5];
        blinking = false;
        eaten = true;
        ReturnToBase();
        UpdateDirection();
        GetComponent<MovementBehaviour>().speed = eatenSpeed;
    }


    /// <summary>
    /// Resume normal roaming.
    /// </summary>
    /// <param name="wasHome"></param>
    /// <returns></returns>
    public IEnumerator ReturnToNormal(bool wasHome)
    {
        yield return new WaitForSeconds(8);
        if (blinking || wasHome)
        {
            blinking = false;
            startPhase = false;
            eaten = false;
            GetComponent<MovementBehaviour>().speed = normalSpeed;
            DeterimeSprite();
        }
    }


    /// <summary>
    /// Go back home.
    /// </summary>
    public void ReturnToBase()
    {
        target = homeNode;
        path = NavigateToTarget();
        direction = ActOnPath();
        SendDirection();
    }


    /// <summary>
    /// Changes sprite based on ghost type.
    /// </summary>
    private void DeterimeSprite()
    {
        switch (type)
        {
            case GhostType.Chaser:
                GetComponent<SpriteRenderer>().sprite = sprites[0];
                break;
            case GhostType.Aheader:
                GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            case GhostType.Roamer:
                GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;
            case GhostType.SlowChaser:
                GetComponent<SpriteRenderer>().sprite = sprites[3];
                break;

        }
    }
}
