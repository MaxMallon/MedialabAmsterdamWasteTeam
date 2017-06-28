using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This script is the core of the game, most happens here.
/// </summary>
public class GameScript : MonoBehaviour {
    public GameObject personaTile; //Prefab for making the persona tiles.
    GameObject[] personaTiles; //Array for the persona tiles to go into.
    public GameObject questionIcon; //Prefab for making the question icons.
    GameObject[] questionIcons; //Array for the question icons to go into.
    int currentQuestion = 0; //The question currently selected by the player.
    int currentPersona; //The persona selected by the program that the player 'is'.
    public int openPersonas; //Number of unflipped persona's.

    public GameObject questionText; //The text that tells the player everything.
    public GameObject cancelButton; 
    public GameObject selectButton;
    public GameObject nextButton;
    public GameObject guessButton;

    public GameObject background;
    bool gameOver = false; //Whether or not the player has solved/lost the game.

    public Sprite[] personaBackGrounds = new Sprite[15];  //Array with all the end screens.

    string[] questions; //Array with all the questions.
    string[] statementsPositive; //Array with all the questions formulated as confirming statements.
    string[] statementsNegative; //Array of all the questions formulated as denying statements.
    string[] personaNames; //Array of all the names of the personas

    string folder; //Path to the txt files.

    /// <summary>
    /// Starting up everything.
    /// </summary>
    void Start () {
        //Set folder to right place.
        folder = Application.streamingAssetsPath;
        //folder = folder.Substring(0, folder.LastIndexOf('/'));
        //folder += "\\" + Application.productName + "\\";

        //Import the questions, statements and names.
        questions = ReadTextFile("questions");
        statementsPositive = ReadTextFile("statementsP");
        statementsNegative = ReadTextFile("statementsN");
        personaNames = ReadTextFile("personaNames");

        //Make the persona tiles
        personaTiles = new GameObject[14];
        float interval = (Screen.width - (130 * 2)) / (7 - 1); //Spacing between the personas.
		for (int i = 0; i < 7; i++)
        {
            personaTiles[i] = GameObject.Instantiate(personaTile, transform); //Make new persona, and add it to the Array.
            bool[] answerSet = ReadPersona(i); //Create a Array with answers for the persona (loaded from file).
            personaTiles[i].GetComponent<Persona>().SetQuestionAnswers(answerSet); //Give the Array to the persona.
            personaTiles[i].GetComponent<RectTransform>().position += new Vector3(interval * i, 1050, 0); //Set the correct position
            personaTiles[i].GetComponent<Persona>().number = i; //Give the right number
            personaTiles[i].GetComponent<Image>().sprite = personaTiles[i].GetComponent<Persona>().spriteList[i]; //Set the right sprite
            personaTiles[i].GetComponentInChildren<Text>().text = personaNames[i]; //Set the right name (loaded from file)
        }
        //interval = (Screen.width - (130 * 2)) / (7 - 1); //In case there needs to be a different number of personas on the 2nd row.
        for (int i = 7; i < 14; i++) //2nd row
        {
            personaTiles[i] = GameObject.Instantiate(personaTile, transform);
            bool[] answerSet = ReadPersona(i);
            personaTiles[i].GetComponent<Persona>().SetQuestionAnswers(answerSet);
            personaTiles[i].GetComponent<RectTransform>().position += new Vector3(interval * (i - 7), 700, 0);
            personaTiles[i].GetComponent<Persona>().number = i;
            personaTiles[i].GetComponent<Image>().sprite = personaTiles[i].GetComponent<Persona>().spriteList[i];
            personaTiles[i].GetComponentInChildren<Text>().text = personaNames[i];
        }
        openPersonas = personaTiles.Length; //Set how many personas there are.

        questionIcons = new GameObject[questions.Length]; //Make questionIcon Array as long as the Array of questions.
        interval = (Screen.width - (80 * 2)) / (questions.Length - 1); //Spacing between the icons.
        for (int i = 0; i < questions.Length; i++) //Make the icons.
        {
            questionIcons[i] = GameObject.Instantiate(questionIcon, transform); //Make and add to Array
            questionIcons[i].GetComponent<QuestionIconScript>().number = i; //Set number
            questionIcons[i].GetComponent<RectTransform>().position += new Vector3(interval * i, 0, 0); //set position
            questionIcons[i].GetComponent<Image>().sprite = questionIcons[i].GetComponent<QuestionIconScript>().spriteList[i]; //Set sprite
        }

        currentPersona = (int)Mathf.Round(Random.Range(0, personaTiles.Length)); //curent persona is random. who you are
       
    }
	
	/// <summary>
    /// Check for button input.
    /// </summary>
	void Update () {
        //If the game's over, space will restart it.
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("menu");
            }
        }
        //escape quits the game.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}

    
    /// <summary>
    /// Reads the persona's file. All the answers to the questions. Only boolean.
    /// </summary>
    /// <param name="personaNumber">What persona needs to be read</param>
    /// <returns>The boolean questions.</returns>
    private bool[] ReadPersona(int personaNumber)
    {
        string name = "persona" + personaNumber.ToString() + ".txt"; //name of the file we're looking for.
        string path = Path.Combine(folder, name); //full path+name of the file we're looking for.
        string rawText = ReadFile(path); //The file read
        string[] splitText = rawText.Split('\n'); //The file split into different lines
        
        //removing junk and empty lines.
        int length = 0; //length the junkless array will have.
        for (int i = 0; i < splitText.Length; i++)
        {
            if (splitText[i] == "true" || splitText[i] == "false") //ignore everything but 'true' and 'false'
            {
                length++;
            }
        }
        bool[] answers = new bool[length]; //The the array that will contain the actual answers.
        length = 0; //Recyling time, now that Length is used, we can just use it again to keep track of number of answers inserted.
        for (int i = 0; i < splitText.Length; i++)
        {
            if (splitText[i] == "true" || splitText[i] == "false") //Ignore everything that's not 'true' or 'false
            {
                answers[length] = bool.Parse(splitText[i]); //parse it into a bool, and add it to the array
                length++;
            }
        }

        return answers;
    }

    /// <summary>
    /// Reads text from a file.
    /// </summary>
    /// <param name="file">file name to be read</param>
    /// <returns>A string array containing the lines of the file.</returns>
    private string[] ReadTextFile(string file)
    {
        string rawText = ReadFile(Path.Combine(folder, file + ".txt")); //Read and store in string.
        string[] splitText = rawText.Split('\n'); //split into lines.

        //Remove empty lines and other junk.
        int length = 0; //Length array of actual lines should have.
        for (int i = 0; i < splitText.Length; i++)
        {
            if (splitText[i] != "") //ignore empty lines
            {
                length++;
            }
        }
       string [] lines = new string[length]; //Actual array to be retunred
        length = 0;
        for (int i = 0; i < splitText.Length; i++)
        {
            if (splitText[i] != "")
            {
                lines[length] = splitText[i]; //Add to array
                length++;
            }
        }

        return lines;
    }
    

    /// <summary>
    /// Reads a file.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>The text in the file in 1 string</returns>
    private string ReadFile(string fileName)
    {
        string fileString = "";
        string line;
        // Create a new StreamReader, tell it which file to read and what encoding the file
        // was saved as
        StreamReader theReader = new StreamReader(fileName, Encoding.Default);
        // Immediately clean up the reader after this block of code is done.
        // You generally use the "using" statement for potentially memory-intensive objects
        // instead of relying on garbage collection.
        // (Do not confuse this with the using directive for namespace at the 
        // beginning of a class!)
        using (theReader)
        {
            // While there's lines left in the text file, keep on reading em.
            do
            {
                line = theReader.ReadLine();
                if (line != null)
                {
                    //Add line to the string.
                    fileString += "\n" + line;
                }
            }
            while (line != null);
            // Done reading, close the reader  
            theReader.Close();
        }
        return fileString;
    }


    /// <summary>
    /// Set the current question.
    /// </summary>
    /// <param name="number">Number of the question</param>
    public void SetQuestion(int number)
    {
        currentQuestion = number; //change currentquestion to the number
        if (currentQuestion != -1) //If it's a positive number, and thus an actually existing question
        {
            questionText.GetComponent<Text>().text = questions[currentQuestion];  //Set the text to display the question
            cancelButton.SetActive(true); //Show the cancel and select butoon
            selectButton.SetActive(true);
            StartCoroutine(selectButton.GetComponent<ActionButtonScript>().SelectBlink());
            foreach (GameObject questionIcon in questionIcons) //Disable all the other question icons
            {
                questionIcon.GetComponent<QuestionIconScript>().ClickingTime = false;
            }
            foreach (GameObject personaTile in personaTiles) //Disable the persona tiles.
            {
                personaTile.GetComponent<Persona>().clickingTime = false;
                personaTile.GetComponent<Persona>().guessingTime = false;
            }
        }
        else //If the number does equal -1, execute the NextQuestion method.
        {
            NextQuestion();
        }
        
    }

    /// <summary>
    /// The player guessed a persona
    /// </summary>
    /// <param name="number">The persona they guessed</param>
    /// <returns></returns>
    public IEnumerator TookAGuess(int number)
    {
        //Display whether or not they were right.
        if (number == currentPersona)
        {
            questionText.GetComponent<Text>().text = "You are right!";
        }
        //If the number equals -1, it means the player elliminated all the personas, and lost by default.
        else if (number == -1)
        {
            questionText.GetComponent<Text>().text = "You elliminated all options, and got it wrong!";
        }
        else
        {
            questionText.GetComponent<Text>().text = "No, you are wrong.";
        }

        //Remove all buttons, and disable everything.
        cancelButton.SetActive(false);
        selectButton.SetActive(false);
        nextButton.SetActive(false);
        guessButton.SetActive(false);
        foreach (GameObject personaTile in personaTiles)
        {
            personaTile.GetComponent<Persona>().clickingTime = false;
        }
        foreach (GameObject questionIcon in questionIcons)
        {
            questionIcon.GetComponent<QuestionIconScript>().ClickingTime = false;
        }
        //Wait 3 seconds
        yield return new WaitForSeconds(3);

        //After the wait, change the background to show the end screen saying which persona they were, and delete all other objects that are in the way
        background.GetComponent<Image>().sprite = personaBackGrounds[currentPersona];
        foreach (GameObject personaTile in personaTiles)
        {
            GameObject.Destroy(personaTile);
        }
        foreach (GameObject questionIcon in questionIcons)
        {
            GameObject.Destroy(questionIcon);
        }
        GameObject.Destroy(nextButton);
        GameObject.Destroy(guessButton);
        GameObject.Destroy(cancelButton);
        GameObject.Destroy(nextButton);

        //The game is done, tell the player to hit space to restart.
        questionText.GetComponent<Text>().text = "\nHit space to restart                                 ";
        gameOver = true;
    }

    /// <summary>
    /// Ask a question
    /// </summary>
    /// <returns></returns>
    public IEnumerator AskQuestion()
    {
        //Set the question asked permanently disabled, and remove cancel and select buttons.
        questionIcons[currentQuestion].GetComponent<QuestionIconScript>().disabeled = true;
        cancelButton.SetActive(false);
        selectButton.SetActive(false);
        //Looks up answer, then show it to the player
        bool questionAnswer = personaTiles[currentPersona].GetComponent<Persona>().AskQuestion(currentQuestion);
        if (questionAnswer)
        {
            questionText.GetComponent<Text>().text = "Yes! You " + statementsPositive[currentQuestion];
        }
        else
        {
            questionText.GetComponent<Text>().text = "No! You " + statementsNegative[currentQuestion];
        }

        //wait 3 seconds
        yield return new WaitForSeconds(3);

        //Tell the player to put down all wrong personas
        if (questionAnswer)
        {
            questionText.GetComponent<Text>().text = "Click on those whom you think " + statementsNegative[currentQuestion];
        }
        else
        {
            questionText.GetComponent<Text>().text = "Click on those whom you think " + statementsPositive[currentQuestion];
        }
        //Allow the player to click the personas down
        foreach (GameObject personaTile in personaTiles)
        {
            personaTile.GetComponent<Persona>().clickingTime = true;
        }
        nextButton.SetActive(true); //Make next button active again, to go to the next question.
    }

    /// <summary>
    /// The player wants to guess who they are.
    /// </summary>
    public void GuessPersona()
    {
        //change the text.
        questionText.GetComponent<Text>().text = "Take a guess then! Guess who!";
        //Turn on the persona clicking, and guessing.
        foreach (GameObject personaTile in personaTiles)
        {
            personaTile.GetComponent<Persona>().clickingTime = true;
            personaTile.GetComponent<Persona>().guessingTime = true;
        }
        //Turn off the question icons.
        foreach (GameObject questionIcon in questionIcons)
        {
            questionIcon.GetComponent<QuestionIconScript>().ClickingTime = false;
        }
        //Show only the cancel button, in case the player makes up their mind
        cancelButton.SetActive(true);
        selectButton.SetActive(false);
        nextButton.SetActive(false);
    }
    
    /// <summary>
    /// After asking a questiong, restore layout to ask another question.
    /// </summary>
    public void NextQuestion()
    {
        //Set the right text again.
        questionText.GetComponent<Text>().text = "Please click on one \nof the question icons below";
        //Turn off the persona tile clicking, and turn on the question icon clicking
        foreach (GameObject personaTile in personaTiles)
        {
            personaTile.GetComponent<Persona>().clickingTime = false;
        }
        foreach (GameObject questionIcon in questionIcons)
        {
            questionIcon.GetComponent<QuestionIconScript>().ClickingTime = true;
        }
        //Don't show buttons.
        cancelButton.SetActive(false);
        selectButton.SetActive(false);
        nextButton.SetActive(false);
    }
}
