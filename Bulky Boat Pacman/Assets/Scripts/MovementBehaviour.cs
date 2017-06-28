using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This script allows for movement over a node-based grid. 
/// It is used by both the player and the ghosts.
/// </summary>
public class MovementBehaviour : MonoBehaviour
{

    public enum Directions { Up, Down, Left, Right, None };
    public Directions directionKey;
    public float direction;
    public NodeScript lastNode;
    public NodeScript nextNode;
    public bool onNode = true;
    private bool justTeleported = false;
    public bool frozen = true;

    public GameController gameController;

    public float speed;

    // Use this for initialization
    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        StartCoroutine(StartMoving());
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    /// <summary>
    /// Allows for movement, unfreezes the entity.
    /// </summary>
    /// <returns></returns>
    IEnumerator StartMoving()
    {
        yield return new WaitForSeconds(1);
        frozen = false;
        Move();
    }

    /// <summary>
    /// Determines whether the entety can continue moving.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="ignoreIgnores">Whether or not nodes with the flag 'ignore' should be ignored, ignoreIgnores = true > no such nodes will be used.</param>
    /// <returns>Whether it can move</returns>
    public bool PlaceFree(Directions direction, bool ignoreIgnores)
    {
        bool placeFree = false;
        //Looks whether the node it last passed is connected to a node in the direction pacman is moving at. This will be always true on a path, but once it reaches a new node, it might no longer be true, and pacman would stop there.
        for (int i = 0; i < lastNode.neighbourDirections.Length; i++)
        {
            if (lastNode.neighbourDirections[i] == direction)
            {
                if (!ignoreIgnores)
                {
                    placeFree = true;
                    transform.rotation = Quaternion.identity;
                    this.direction = (lastNode.trueNeighbourDirections[i] - 90) / -180 * Mathf.PI;
                    transform.Rotate(new Vector3(0, 0, -(lastNode.trueNeighbourDirections[i] - 90)));
                    break;
                }
                if (ignoreIgnores)
                {
                    if (!lastNode.neighbours[i].GetComponent<NodeScript>().ignore)
                    {
                        placeFree = true;
                        transform.rotation = Quaternion.identity;
                        this.direction = (lastNode.trueNeighbourDirections[i] - 90) / -180 * Mathf.PI;
                        transform.Rotate(new Vector3(0, 0, -(lastNode.trueNeighbourDirections[i] - 90)));
                        break;
                    }
                }
            }
        }

        if (nextNode != null && !placeFree) //nextnode is null if the player is not moving. 
        {
            //to turn directions midway the line, look at the neighbour in the direction of current movement attached to the lastnode.
            if (nextNode.GetComponent<NodeScript>().GetReturnPathDirection(lastNode) != Directions.None)
            {
                if (nextNode.GetComponent<NodeScript>().GetReturnPathDirection(lastNode) == direction)
                {
                    for (int i = 0; i < nextNode.neighbourDirections.Length; i++)
                    {
                        if (nextNode.neighbourDirections[i] == direction)//if multiple paths have the same diretion, too bad
                        {
                            if (ignoreIgnores)
                            {
                                if (!nextNode.neighbours[i].GetComponent<NodeScript>().ignore)
                                {
                                    placeFree = true;
                                    transform.rotation = Quaternion.identity;
                                    this.direction = (nextNode.trueNeighbourDirections[i] - 90) / 180 * Mathf.PI;
                                    transform.Rotate(new Vector3(0, 0, -(nextNode.trueNeighbourDirections[i] - 90)));
                                    break;
                                }
                            }
                            if (!ignoreIgnores)
                            {
                                placeFree = true;
                                transform.rotation = Quaternion.identity;
                                this.direction = (nextNode.trueNeighbourDirections[i] - 90) / 180 * Mathf.PI;
                                transform.Rotate(new Vector3(0, 0, -(nextNode.trueNeighbourDirections[i] - 90)));
                                break;
                            }
                        }
                    }
                }
            }
            
        }
        if (!placeFree)//the player cannot move, and thus won't, hence no next node the player is heading to.
        {
            nextNode = null;
        }
        return placeFree;
    }

    /// <summary>
    /// Try and change directions, can only be actually done on a node.
    /// </summary>
    /// <param name="attemptedDirection"></param>
    /// <param name="ignoreIgnores"></param>
    public void TryChangeDirections(Directions attemptedDirection, bool ignoreIgnores)
    {        //If there is a nextnode, aka, the player is between 2 nodes, heading in a direction, or on a node that is connecting 2 others in a straight line
        if (nextNode != null)
        {
            //if direction attempted is the same as the direction that connects the node headed to, to the last node (aka, way back)
            if (attemptedDirection == nextNode.GetReturnPathDirection(lastNode))
            {
                if (PlaceFree(attemptedDirection, ignoreIgnores))
                {
                    directionKey = attemptedDirection;
                    if (onNode) //set position back to the node, to make sure we stay on the tracks
                    {
                        transform.position = lastNode.transform.position;
                    }
                    NodeScript node = lastNode;
                    lastNode = nextNode;
                    nextNode = node;
                }
            }
            else if (onNode && PlaceFree(attemptedDirection, ignoreIgnores)) //if we're on a node, but not trying to go back where we came from
            {
                if (directionKey != attemptedDirection)
                {
                    transform.position = lastNode.transform.position;
                }
                directionKey = attemptedDirection;
            }
        }
        else if (PlaceFree(attemptedDirection, ignoreIgnores) && onNode)
        {
            if (attemptedDirection != directionKey)
            {
                transform.position = lastNode.transform.position;
            }
            directionKey = attemptedDirection;

        }

    }


    /// <summary>
    /// Move along the grid.
    /// </summary>
    void Move()
    {
        if (PlaceFree(directionKey, false) && !frozen)
        {
            transform.position += new Vector3(Mathf.Cos(direction), Mathf.Sin(direction), 0) * speed * Time.deltaTime;
        }
    }

    /// <summary>
    /// If there's a collision between the entity and a node, the current node and such should be updated
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Node")
        {
            onNode = true;
            if (lastNode == null)
            {
                lastNode = other.GetComponent<NodeScript>();
            }
            if (other.GetComponent<NodeScript>() != lastNode)
            {
                if (other.GetComponent<NodeScript>().teleportConnectionEnabeled && !justTeleported)
                {
                    transform.position = other.GetComponent<NodeScript>().teleportConnection.transform.position;
                    lastNode = other.GetComponent<NodeScript>().teleportConnection.GetComponent<NodeScript>();
                    justTeleported = true;
                    StartCoroutine(SetTeleportActive());
                    return;
                }
                else
                {
                    lastNode = other.GetComponent<NodeScript>();
                    transform.position = other.transform.position;
                }
                //Set Nextnode if possible, placefree will set it null if there is no such neighbour.
                for (int i = 0; i < lastNode.neighbourDirections.Length; i++)
                {
                    if (lastNode.neighbourDirections[i] == directionKey)
                    {
                        nextNode = lastNode.neighbours[i].GetComponent<NodeScript>();
                        break;
                    }
                }
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Node")
        {
            onNode = false;
            lastNode = other.GetComponent<NodeScript>();
            //Set Nextnode if possible, placefree will set it null if there is no such neighbour.
            for (int i = 0; i < lastNode.neighbourDirections.Length; i++)
            {
                if (lastNode.neighbourDirections[i] == directionKey)
                {
                    nextNode = lastNode.neighbours[i].GetComponent<NodeScript>();
                    break;
                }
            }
        }
    }


    /// <summary>
    /// Don't get stuck between teleporting forever, makes sure they teleport only once, can only do so again a split second afterwards.
    /// </summary>
    /// <returns></returns>
    IEnumerator SetTeleportActive()
    {
        yield return new WaitForSeconds(0.1f);
        justTeleported = false;
    }
}
