using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// These are the nodes that together form the grid that entities move over. 
/// </summary>
public class NodeScript : MonoBehaviour {

    public GameObject[] neighbours; //The neighbouring nodes. 
    public MovementBehaviour.Directions[] neighbourDirections; //The directions those neighbouring nodes have as they would be bound to arrow keys.
    public float[] trueNeighbourDirections; //The directions the neighbours have as angle
    public float[] distances; //How far each neighbour is.
    public float gScore, fScore; //for a* pathfinding
    public NodeScript previousNode; //for a* pathfinding
    public bool startNode; //spawn a player?
    public bool spawnSuperDot; //spawn a super dot?
    public bool spawnFruit; //Spawn fruits?
    public bool spawnGhost; //Spawn a ghost?
    public bool spawnDots; //Spawn dots?
    public bool ignore; //Ignore in pathfinding?
    public bool teleportConnectionEnabeled; //Connected with a teleport?
    public GameObject teleportConnection; //connected to teleport
    public GhostBehaviour.GhostType ghostType; //What ghosttype should it spawn?
    public GameObject player; //bunch of prefabs.
    public GameObject normalFood;
    public GameObject superFood;
    public GameObject fruit;
    public GameObject ghost;
    private GameController game;
    


    // Use this for initialization
    void Start ()
    {
        gScore = float.MaxValue;
        fScore = float.MaxValue;
        previousNode = null;
        int[] directionsTaken = new int[4];
        directionsTaken[0] = -1; //up
        directionsTaken[1] = -1; //right
        directionsTaken[2] = -1; //down
        directionsTaken[3] = -1; //left  //index in array is the direction, -1 is 'direction not taken', other values are indeci of what took that direction.

        neighbourDirections = new MovementBehaviour.Directions[neighbours.Length];
        trueNeighbourDirections = new float[neighbours.Length];

        CalculateAngles();
        CalculateDistances();

        PlaceDirections();

        game = GameObject.Find("GameController").GetComponent<GameController>();
        game.nodes.Add(gameObject);

        if (startNode)
        {
            GameObject playerInstance = GameObject.Instantiate(player, transform.position, Quaternion.identity);
            playerInstance.GetComponent<MovementBehaviour>().lastNode = this;
            playerInstance.GetComponent<MovementBehaviour>().nextNode = neighbours[0].GetComponent<NodeScript>();
            playerInstance.GetComponent<MovementBehaviour>().directionKey = MovementBehaviour.Directions.None;
            playerInstance.GetComponent<MovementBehaviour>().direction = 0;//neighbourDirections[0];

        }
        if (spawnGhost)
        {
            GameObject ghostInstance = GameObject.Instantiate(ghost, transform.position, Quaternion.identity);
            ghostInstance.GetComponent<MovementBehaviour>().lastNode = this;
            ghostInstance.GetComponent<MovementBehaviour>().nextNode = neighbours[0].GetComponent<NodeScript>();
            ghostInstance.GetComponent<MovementBehaviour>().directionKey = neighbourDirections[0];
            ghostInstance.GetComponent<GhostBehaviour>().type = ghostType;
            ghostInstance.GetComponent<GhostBehaviour>().homeNode = this;
            switch (ghostType)
            {
                case GhostBehaviour.GhostType.Chaser:
                    game.ghosts[0] = ghostInstance;
                    break;
                case GhostBehaviour.GhostType.Aheader:
                    game.ghosts[1] = ghostInstance;
                    break;
                case GhostBehaviour.GhostType.Roamer:
                    game.ghosts[2] = ghostInstance;
                    break;
                case GhostBehaviour.GhostType.SlowChaser:
                    game.ghosts[3] = ghostInstance;
                    break;

            }

        }

        if (spawnDots)
        {
            PlaceNormalFood(0.15f);
        }

    }
	


    //Draw the routes, and nodes.
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (spawnSuperDot)
        {
            Gizmos.color = Color.red;
        }
        if (spawnFruit)
        {
            Gizmos.color = Color.blue;
        }
        Gizmos.DrawSphere(transform.position, 0.03f);

        Gizmos.color = Color.white;
        foreach (GameObject neighbour in neighbours)
        {
            Gizmos.DrawLine(transform.position, neighbour.transform.position);
        }
    }


    /// <summary>
    /// Looks for the neighbour that is in a certain direction.   Kinda unused...
    /// </summary>
    /// <param name="direction">The direction the neighbour you're looking for is in from this node</param>
    /// <returns>returns the index said neighbour has in all arrays of this node</returns>
    private int GetConnectingNeighbourDirectionIndex(MovementBehaviour.Directions direction)
    {
        int index = -1;
        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbourDirections[i] == direction)
            {
                index = i; 
            }
        }
        return index;
    }

    /// <summary>
    /// Returns the direction to get from the returnnode to this node.
    /// </summary>
    /// <param name="returnNode"></param>
    /// <returns>The direction to get from the returnnode to this node.</returns>
    public MovementBehaviour.Directions GetReturnPathDirection(NodeScript returnNode)
    {
        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i].GetComponent<NodeScript>() == returnNode)
            {
                return neighbourDirections[i];
            }
        }
        return MovementBehaviour.Directions.None;
    }

    /// <summary>
    /// Places food items on the lines.
    /// </summary>
    /// <param name="interval"></param>
    public void PlaceNormalFood(float interval)
    {
        if (spawnSuperDot)
        {
            GameObject.Instantiate(superFood, transform.position, Quaternion.identity);
        }
        else
        {
            game.yumyumDots.Add(GameObject.Instantiate(normalFood, transform.position, Quaternion.identity));
        }
        foreach (GameObject neighbour in neighbours)
        {
            float amount = Vector2.Distance(transform.position, neighbour.transform.position) / interval;
            int amount2 = (int)Mathf.Round(amount);
            Vector3 intervallos = (neighbour.transform.position - transform.position) / amount2;
            for (int i = 1; i < amount2; i++)
            {
                game.yumyumDots.Add(GameObject.Instantiate(normalFood, transform.position + i * intervallos, Quaternion.identity));
            }
        }
    }


    /// <summary>
    /// Place all the directions to the neighbours.
    /// </summary>
    private void PlaceDirections()
    {
       switch (neighbours.Length)
        {
            case 1: OneDirection();
                break;
            case 2:
                MoreDirections(2);
                break;
            case 3:
                MoreDirections(3);
                break;
            case 4:
                MoreDirections(4);
                break;
        }
    }

    /// <summary>
    /// Gets the angle between 2 gameobjects in the format I want.
    /// </summary>
    /// <param name="a">'self' object</param>
    /// <param name="b">'other' object</param>
    /// <returns></returns>
    private float NodeAngle(GameObject a, GameObject b)
    {
        float angle = Mathf.Atan2(b.transform.position.y - a.transform.position.y, b.transform.position.x - a.transform.position.x);
        angle *= -180 / Mathf.PI; //transform to degrees
        angle += 90; //fix orientation
        while (angle > 360) //Get rid of access degrees
        {
            angle -= 360;
        }
        while (angle < 0) //get rid of access degrees
        {
            angle += 360;
        }
        return angle;
    }

    /// <summary>
    /// Only one neighbour, and one direction (no copyright infringement inteded)
    /// </summary>
    private void OneDirection()
    {
        neighbourDirections[0] = SimpleDirectionCast(trueNeighbourDirections[0]);
    }

    /// <summary>
    /// Transforms a float angle into a 4 directional direction.
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    private MovementBehaviour.Directions SimpleDirectionCast(float angle)
    {
        MovementBehaviour.Directions returnVal = MovementBehaviour.Directions.None;
        if ((angle >= -45 && angle < 45) || (angle >= 315 && angle < 405))
        {
            returnVal = MovementBehaviour.Directions.Up;
        }
        else if (angle >= 45 && angle < 135)
        {
            returnVal = MovementBehaviour.Directions.Right;
        }
        else if (angle >= 135 && angle < 225)
        {
            returnVal = MovementBehaviour.Directions.Down;
        }
        else if (angle >= 235 && angle < 315)
        {
            returnVal = MovementBehaviour.Directions.Left;
        }
        return returnVal;
    }
   

    /// <summary>
    /// Calculates distances between nodes.
    /// </summary>
    private void CalculateDistances()
    {
        distances = new float[neighbours.Length];
        for (int i = 0; i < neighbours.Length; i++)
        {
            distances[i] = Vector2.Distance(transform.position, neighbours[i].transform.position);
        }
    }

    /// <summary>
    /// Calculates distances between nodes.
    /// </summary>
    private void CalculateAngles()
    {
        trueNeighbourDirections = new float[neighbours.Length];
        for (int i = 0; i < neighbours.Length; i++)
        {
            trueNeighbourDirections[i] = NodeAngle(gameObject, neighbours[i].gameObject);
        }
    }

    /// <summary>
    /// In case there are several directions, and they all need to be assigned a direction, possibly with conflicts.
    /// </summary>
    /// <param name="numberOfDirections"></param>
    private void MoreDirections(int numberOfDirections)
    {
        MovementBehaviour.Directions[] directions = new MovementBehaviour.Directions[numberOfDirections]; //store the 4-dir directions
        for (int i = 0; i < numberOfDirections; i++) //Loop through all neighbours and assign the angles and directions/
        {
            directions[i] = SimpleDirectionCast(trueNeighbourDirections[i]);
        }

        bool[][] directionsCorrected = new bool[4][]; //1st array is the direction, 2nd array is direction to which 1st array's direction has been corrected.
        for (int i = 0; i < 4; i++) //TODO deze array moet de daadwerkelijke hoeken bijhouden, de richtingen, niet de verbeterde dingen, want die veranderen constant.
        {
            directionsCorrected[i] = new bool[4];
            directionsCorrected[i][0] = false; //up
            directionsCorrected[i][1] = false; //right
            directionsCorrected[i][2] = false; //down
            directionsCorrected[i][3] = false; //left  //index in array is the direction, -1 is 'direction not corrected', other values are indeci of what took that direction.
        }

        //TODO ofwel maak de corrected dingen nog complexer, dat ze elke entry en eigen array heeft met daarin elke richting, ofwel verander wat wanneer wordt veranderd.


        int maxConflicts = 15;
        int triedConflicts = 0;
        bool conflicts = false; //Let's assume there are no conflicts, and then check
        do //TODO make something that keeps track of what has already been corrected befoer, to precent endless loops, somwwhat
        {
            conflicts = false;
            triedConflicts++;
            int[] directionsTaken = new int[4];
            directionsTaken[0] = -1; //up
            directionsTaken[1] = -1; //right
            directionsTaken[2] = -1; //down
            directionsTaken[3] = -1; //left  //index in array is the direction, -1 is 'direction not taken', other values are indeci of what took that direction.

            for (int i = 0; i < directions.Length; i++) //Loop through true directions, and check for doubles. If there are doubles, resolve them, and restart this while loop, again assuming everything's been fixed till proven otherwise.
            {
                switch (directions[i])
                {
                    case MovementBehaviour.Directions.Up:
                        if (directionsTaken[0] == -1)
                        { // If the direction hasn't been taken yet, take it
                            directionsTaken[0] = i;//assign the index
                            directions[i] = MovementBehaviour.Directions.Up;
                        }
                        else
                        {
                            conflicts = true; //oh shit, it has been taken, fix it.
                            float[] angleArray = new float[2];
                            angleArray[0] = trueNeighbourDirections[i]; //current neighbour connection/direction has index 0
                            angleArray[1] = trueNeighbourDirections[directionsTaken[0]]; //conflicting neighbour connection/direction that already has UP has index 1
                            MovementBehaviour.Directions[] correctSet = CorrectTwoAngles(angleArray, MovementBehaviour.Directions.Up, directionsCorrected[0]);
                            //correctset[0] is still current angle, and [1] still the other one, only now the values are 'corrected'.
                            switch (correctSet[1])
                            {
                                case MovementBehaviour.Directions.Up:
                                    directionsTaken[0] = directionsTaken[0];
                                    directionsCorrected[0][0] = true;
                                    break;
                                case MovementBehaviour.Directions.Right:
                                    directionsTaken[1] = directionsTaken[0];
                                    directionsCorrected[0][1] = true;
                                    break;
                                case MovementBehaviour.Directions.Down:
                                    directionsTaken[2] = directionsTaken[0];
                                    directionsCorrected[0][2] = true;
                                    break;
                                case MovementBehaviour.Directions.Left:
                                    directionsTaken[3] = directionsTaken[0];
                                    directionsCorrected[0][3] = true;
                                    break;
                            }
                            switch (correctSet[0])
                            {
                                case MovementBehaviour.Directions.Up:
                                    directionsTaken[0] = i;
                                    directionsCorrected[directionsTaken[0]][0] = true;
                                    break;
                                case MovementBehaviour.Directions.Right:
                                    directionsTaken[1] = i;
                                    directionsCorrected[directionsTaken[0]][1] = true;
                                    break;
                                case MovementBehaviour.Directions.Down:
                                    directionsTaken[2] = i;
                                    directionsCorrected[directionsTaken[0]][2] = true;
                                    break;
                                case MovementBehaviour.Directions.Left:
                                    directionsTaken[3] = i;
                                    directionsCorrected[directionsTaken[0]][3] = true;
                                    break;
                            }
                            directions[i] = correctSet[0];
                            directions[directionsTaken[0]] = correctSet[1];
                            break;
                        }
                        break;
                    case MovementBehaviour.Directions.Right:
                        if (directionsTaken[1] == -1)
                        {
                            directions[i] = MovementBehaviour.Directions.Right;
                            directionsTaken[1] = i;
                        }
                        else
                        {
                            conflicts = true;
                            float[] angleArray = new float[2];
                            angleArray[0] = trueNeighbourDirections[i];
                            angleArray[1] = trueNeighbourDirections[directionsTaken[1]];
                            MovementBehaviour.Directions[] correctSet = CorrectTwoAngles(angleArray, MovementBehaviour.Directions.Right, directionsCorrected[1]);
                            switch (correctSet[1])
                            {
                                case MovementBehaviour.Directions.Up:
                                    directionsTaken[0] = directionsTaken[1];
                                    directionsCorrected[1][0] = true;
                                    break;
                                case MovementBehaviour.Directions.Right:
                                    directionsTaken[1] = directionsTaken[1];
                                    directionsCorrected[1][1] = true;
                                    break;
                                case MovementBehaviour.Directions.Down:
                                    directionsTaken[2] = directionsTaken[1];
                                    directionsCorrected[1][2] = true;
                                    break;
                                case MovementBehaviour.Directions.Left:
                                    directionsTaken[3] = directionsTaken[1];
                                    directionsCorrected[1][3] = true;
                                    break;
                            }
                            switch (correctSet[0])
                            {
                                case MovementBehaviour.Directions.Up:
                                    directionsTaken[0] = i;
                                    directionsCorrected[directionsTaken[1]][0] = true;
                                    break;
                                case MovementBehaviour.Directions.Right:
                                    directionsTaken[1] = i;
                                    directionsCorrected[directionsTaken[1]][1] = true;
                                    break;
                                case MovementBehaviour.Directions.Down:
                                    directionsTaken[2] = i;
                                    directionsCorrected[directionsTaken[1]][2] = true;
                                    break;
                                case MovementBehaviour.Directions.Left:
                                    directionsTaken[3] = i;
                                    directionsCorrected[directionsTaken[1]][3] = true;
                                    break;
                            }
                            directions[i] = correctSet[0];
                            directions[directionsTaken[1]] = correctSet[1];
                            break;
                        }
                        break;
                    case MovementBehaviour.Directions.Down:
                        if (directionsTaken[2] == -1)
                        {
                            directions[i] = MovementBehaviour.Directions.Down;
                            directionsTaken[2] = i;
                        }
                        else
                        {
                            conflicts = true;
                            float[] angleArray = new float[2];
                            angleArray[0] = trueNeighbourDirections[i];
                            angleArray[1] = trueNeighbourDirections[directionsTaken[2]];
                            MovementBehaviour.Directions[] correctSet = CorrectTwoAngles(angleArray, MovementBehaviour.Directions.Down, directionsCorrected[2]);
                            switch (correctSet[1])
                            {
                                case MovementBehaviour.Directions.Up:
                                    directionsTaken[0] = directionsTaken[2];
                                    directionsCorrected[2][0] = true;
                                    break;
                                case MovementBehaviour.Directions.Right:
                                    directionsTaken[1] = directionsTaken[2];
                                    directionsCorrected[2][1] = true;
                                    break;
                                case MovementBehaviour.Directions.Down:
                                    directionsTaken[2] = directionsTaken[2];
                                    directionsCorrected[2][2] = true;
                                    break;
                                case MovementBehaviour.Directions.Left:
                                    directionsTaken[3] = directionsTaken[2];
                                    directionsCorrected[2][3] = true;
                                    break;
                            }
                            switch (correctSet[0])
                            {
                                case MovementBehaviour.Directions.Up:
                                    directionsTaken[0] = i;
                                    directionsCorrected[directionsTaken[2]][0] = true;
                                    break;
                                case MovementBehaviour.Directions.Right:
                                    directionsTaken[1] = i;
                                    directionsCorrected[directionsTaken[2]][1] = true;
                                    break;
                                case MovementBehaviour.Directions.Down:
                                    directionsTaken[2] = i;
                                    directionsCorrected[directionsTaken[2]][2] = true;
                                    break;
                                case MovementBehaviour.Directions.Left:
                                    directionsTaken[3] = i;
                                    directionsCorrected[directionsTaken[2]][3] = true;
                                    break;
                            }
                            directions[i] = correctSet[0];
                            directions[directionsTaken[2]] = correctSet[1];
                            break;
                        }
                        break;
                    case MovementBehaviour.Directions.Left:
                        if (directionsTaken[3] == -1)
                        {
                            directions[i] = MovementBehaviour.Directions.Left;
                            directionsTaken[3] = i;
                        }
                        else
                        {
                            conflicts = true;
                            float[] angleArray = new float[2];
                            angleArray[0] = trueNeighbourDirections[i];
                            angleArray[1] = trueNeighbourDirections[directionsTaken[3]];
                            MovementBehaviour.Directions[] correctSet = CorrectTwoAngles(angleArray, MovementBehaviour.Directions.Left, directionsCorrected[3]);
                            switch (correctSet[1])
                            {
                                case MovementBehaviour.Directions.Up:
                                    directionsTaken[0] = directionsTaken[3];
                                    directionsCorrected[3][0] = true;
                                    break;
                                case MovementBehaviour.Directions.Right:
                                    directionsTaken[1] = directionsTaken[3];
                                    directionsCorrected[3][1] = true;
                                    break;
                                case MovementBehaviour.Directions.Down:
                                    directionsTaken[2] = directionsTaken[3];
                                    directionsCorrected[3][2] = true;
                                    break;
                                case MovementBehaviour.Directions.Left:
                                    directionsTaken[3] = directionsTaken[3];
                                    directionsCorrected[3][3] = true;
                                    break;
                            }
                            switch (correctSet[0])
                            {
                                case MovementBehaviour.Directions.Up:
                                    directionsTaken[0] = i;
                                    directionsCorrected[directionsTaken[2]][0] = true;
                                    break;
                                case MovementBehaviour.Directions.Right:
                                    directionsTaken[1] = i;
                                    directionsCorrected[directionsTaken[3]][1] = true;
                                    break;
                                case MovementBehaviour.Directions.Down:
                                    directionsTaken[2] = i;
                                    directionsCorrected[directionsTaken[3]][2] = true;
                                    break;
                                case MovementBehaviour.Directions.Left:
                                    directionsTaken[3] = i;
                                    directionsCorrected[directionsTaken[3]][3] = true;
                                    break;
                            }
                            directions[i] = correctSet[0];
                            directions[directionsTaken[3]] = correctSet[1];
                            break;
                        }
                        break;
                }
            }

        } while (conflicts && triedConflicts < maxConflicts); //keep doing this till there are no more conflicts.

        //assign the method-only arrays to the actual arrays now.
        this.neighbourDirections = directions;
    }

    /// <summary>
    /// Returns the distance between any 2 nodes.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public float DistanceEstimate(NodeScript from, NodeScript to)
    {
        return Vector2.Distance(from.transform.position, from.transform.position);
    }


    /// <summary>
    /// Reassigns 2 angles to directions, since they conflict.
    /// </summary>
    /// <param name="angle1">actual angle 1</param>
    /// <param name="angle2">actual angle 2</param>
    /// <param name="angle">the casted angle they both were assigned to.</param>
    /// <returns>an array containing the corrected assigned angled, 1, then 2</returns>
    private MovementBehaviour.Directions[] CorrectTwoAngles(float[] angles, MovementBehaviour.Directions angle, bool[] forbiddenAngles)
    {
        MovementBehaviour.Directions[] returnVal =  new MovementBehaviour.Directions[2];
        float perfectAngle = 0;
        switch (angle) //See what the actual angle is
        {
            case MovementBehaviour.Directions.Up:
                perfectAngle = 0;
                break;
            case MovementBehaviour.Directions.Right:
                perfectAngle = 90;
                break;
            case MovementBehaviour.Directions.Down:
                perfectAngle = 180;
                break;
            case MovementBehaviour.Directions.Left:
                perfectAngle = 270;
                break;
        }

        //Calculate the difference of the 2 actual angles, with what their (shared) button ideally represents.
        float[] angleDeltas = new float[2];
        angleDeltas[0] = (Mathf.Abs(angles[0] - perfectAngle));
        angleDeltas[1] = (Mathf.Abs(angles[1] - perfectAngle));
        Debug.Log(angleDeltas[0] + "  " + angleDeltas[1]);
        //see which of the 2 is closer to the ideal, change the other.

        //standard is angle 1 is farther from the ideal angle than angle 2
        int remainSameIndex = 1;
        int changeIndex = 0;

        //check if situation is standard.
        if (angleDeltas[0] <= angleDeltas[1])
        {
            remainSameIndex = 0;
            changeIndex = 1;
        }
        returnVal[remainSameIndex] = angle; //can stay the same
        if (angles[changeIndex] > perfectAngle) //The one that needs to be changed
        {
            switch (angle) //angle is bigger, rotate clockwise.
            {
                case MovementBehaviour.Directions.Up:
                    if (!forbiddenAngles[0])
                    {
                        returnVal[changeIndex] = MovementBehaviour.Directions.Right;
                    }
                    else
                    {
                        returnVal[changeIndex] = MovementBehaviour.Directions.Left;
                    }
                    break;
                case MovementBehaviour.Directions.Right:
                    Debug.Log(forbiddenAngles[1]);
                    if (!forbiddenAngles[1])
                    {
                        returnVal[changeIndex] = MovementBehaviour.Directions.Down;
                    }
                    else
                    {
                        returnVal[changeIndex] = MovementBehaviour.Directions.Up;
                    }
                    break;
                case MovementBehaviour.Directions.Down:
                    if (!forbiddenAngles[2])
                    {
                        returnVal[changeIndex] = MovementBehaviour.Directions.Left;
                    }
                    else
                    {
                        returnVal[changeIndex] = MovementBehaviour.Directions.Right;
                    }
                    break;
                case MovementBehaviour.Directions.Left:
                    if (!forbiddenAngles[3])
                    {
                        returnVal[changeIndex] = MovementBehaviour.Directions.Up;
                    }
                    else
                    {
                        returnVal[changeIndex] = MovementBehaviour.Directions.Down;
                    }
                    break;
            }
        }
        else //angle is smaller, rotate counter clockwise.
        {
            switch (angle)
            {
                case MovementBehaviour.Directions.Up:
                    if (!forbiddenAngles[0])
                    {
                        returnVal[changeIndex] = MovementBehaviour.Directions.Left;
                    }
                    else
                    {
                        returnVal[changeIndex] = MovementBehaviour.Directions.Right;
                    }
                    break;
                case MovementBehaviour.Directions.Right:
                    if (!forbiddenAngles[1])
                    {
                        returnVal[changeIndex] = MovementBehaviour.Directions.Up;
                    }
                    else
                    {
                        returnVal[changeIndex] = MovementBehaviour.Directions.Down;
                    }
                    break;
                case MovementBehaviour.Directions.Down:
                    if (!forbiddenAngles[2])
                    {
                        returnVal[changeIndex] = MovementBehaviour.Directions.Right;
                    }
                    else
                    {
                        returnVal[changeIndex] = MovementBehaviour.Directions.Left;
                    }
                    break;
                case MovementBehaviour.Directions.Left:
                    if (!forbiddenAngles[3])
                    {
                        returnVal[changeIndex] = MovementBehaviour.Directions.Down;
                    }
                    else
                    {
                        returnVal[changeIndex] = MovementBehaviour.Directions.Up;
                    }
                    break;
            }
        }
        Debug.Log(gameObject +  "   "  + angles[0] + "  " + angles[1] + "   " + angle + "   " + returnVal[0] + "   " + returnVal[1]);
        return returnVal;
    }


    public void SpawnStuffNoDots(bool onlyFruit)
    {
        if (spawnFruit)
        {
            GameObject.Instantiate(fruit, transform.position, Quaternion.identity);
        }
        if (!onlyFruit)
        {
            if (startNode)
            {
                GameObject playerInstance = GameObject.Instantiate(player, transform.position, Quaternion.identity);
                playerInstance.GetComponent<MovementBehaviour>().lastNode = this;
                playerInstance.GetComponent<MovementBehaviour>().nextNode = neighbours[0].GetComponent<NodeScript>();
                playerInstance.GetComponent<MovementBehaviour>().directionKey = MovementBehaviour.Directions.None;
                playerInstance.GetComponent<MovementBehaviour>().direction = 0;//neighbourDirections[0];

            }
            if (spawnGhost)
            {
                GameObject ghostInstance = GameObject.Instantiate(ghost, transform.position, Quaternion.identity);
                ghostInstance.GetComponent<MovementBehaviour>().lastNode = this;
                ghostInstance.GetComponent<MovementBehaviour>().nextNode = neighbours[0].GetComponent<NodeScript>();
                ghostInstance.GetComponent<MovementBehaviour>().directionKey = 0;// neighbourDirections[0];
                ghostInstance.GetComponent<GhostBehaviour>().type = ghostType;
                ghostInstance.GetComponent<GhostBehaviour>().homeNode = this;
                switch (ghostType)
                {
                    case GhostBehaviour.GhostType.Chaser:
                        game.ghosts[0] = ghostInstance;
                        break;
                    case GhostBehaviour.GhostType.Aheader:
                        game.ghosts[1] = ghostInstance;
                        break;
                    case GhostBehaviour.GhostType.Roamer:
                        game.ghosts[2] = ghostInstance;
                        break;
                    case GhostBehaviour.GhostType.SlowChaser:
                        game.ghosts[3] = ghostInstance;
                        break;

                }
            }
        }
    }
}
