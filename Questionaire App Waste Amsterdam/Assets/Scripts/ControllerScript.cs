using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System;

/// <summary>
/// Controlls all the questions, and saves the answers.
/// </summary>
public class ControllerScript : MonoBehaviour
{
    string[] questions; //The list of questions.
    public enum QuestionTypes { MultipleChoice, OpenQuestion, OpenNameQuestion, RecordQuestion, NotAQuestion }; //Types of questions
    public QuestionTypes[] questionTypes;
    enum LineTypes {QuestionCount, Question, QuestionType, AnswerCount, Answer, QuestionLength}; //Types of lines to be read.
    
    public GameObject progressionBarDotFull, progressionBarDotEmpty;
    public GameObject[] progressionBarDots;

    public GameObject flagD, flagE; //The two flags.
    public string questionFile; //Name of the files.
    string[][] answers; //Array containing multiplechoice answers to questions
    int[] notAQuestionLengths; //array with lengths that non questions need to be displayed.
    public Text question; //The questiontext.
    public Button button; //A button.
    public InputField field; //The inputfield.
    int currentQuestion = 0; //The question currently displayed.
    string currentAnswer; //Current answer when reading the files.
    string folder; //Path of where stuff needs to be saved.
    string filledInName; //name the person fills in.

    //For saving the recordings
    AudioSource aud;
    AudioClip myAudioClip;
    bool recording;
    int recordingSeconds = 0;

    
    /// <summary>
    /// Start, I mean, what does it look like.
    /// </summary>
    void Start()
    {
        filledInName = "NAME";
        folder = Application.persistentDataPath;
        folder = folder.Substring(0, folder.LastIndexOf('/'));

        aud = GetComponent<AudioSource>();
        //Start the questionaire.
        SetQuestion(-1);
    }
    
    /// <summary>
    /// This method will load the appropiate ellements for each question.
    /// </summary>
    /// <param name="questionNumber">The number of the question that needs to loaded.</param>
    public void SetQuestion(int questionNumber)
    {
        //Remove ellements from the previous question.
        foreach (Transform child in transform)
        {
            if (child.tag == "Answer" || child.tag == "Input")
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        
        //for (re)start.
        if (questionNumber == -1)
        {
            question.GetComponent<Text>().text = "Please select a language. \nSelecteer uw taal.";
            return;
        }

        float interval = (Screen.width * 0.75f) / questions.Length;
        for (int i = 0; i < currentQuestion; i++)
        {
            progressionBarDots[i] = GameObject.Instantiate(progressionBarDotFull, transform);

            progressionBarDots[i].GetComponent<RectTransform>().position = new Vector2((Screen.width * 0.125f) + (i + 0.5f) * interval, 80);
        }
        for (int i = currentQuestion; i < questions.Length; i++)
        {
            progressionBarDots[i] = GameObject.Instantiate(progressionBarDotEmpty, transform);
            progressionBarDots[i].GetComponent<RectTransform>().position = new Vector2((Screen.width * 0.125f) + (i + 0.5f) * interval, 80);
        }
        question.GetComponent<Text>().text = FixQuestionText(questions[questionNumber]);
        switch (questionTypes[questionNumber])
        {
            case QuestionTypes.MultipleChoice:
                MultipleChoiceQuestion(questionNumber);
                break;
            case QuestionTypes.OpenQuestion:
                OpenQuestion(questionNumber);
                break;
            case QuestionTypes.OpenNameQuestion:
                OpenNameQuestion(questionNumber);
                break;
            case QuestionTypes.RecordQuestion:
                RecordingQuestion(questionNumber);
                break;
            case QuestionTypes.NotAQuestion:
                NotAQuestion(questionNumber);
                break;
        }

        Button quitButton = GameObject.Instantiate(button, transform);
        quitButton.GetComponent<RectTransform>().position = GetComponentInParent<Transform>().position - new Vector3(-880, 390, 0);
        //Text quitButtonText = quitButton.transform.GetChild(0).GetComponent<Text>();
        //quitButtonText.text = "Quit";
        quitButton.GetComponent<MenuButtonScript>().SetButtonType("Quit");

        Button restartButton = GameObject.Instantiate(button, transform);
        restartButton.GetComponent<RectTransform>().position = GetComponentInParent<Transform>().position - new Vector3(-880, 510, 0);
        //Text restartButtonText = restartButton.transform.GetChild(0).GetComponent<Text>();
        //restartButtonText.text = "Restart";
        restartButton.GetComponent<MenuButtonScript>().SetButtonType("Restart");
    }

    /// <summary>
    /// Inserts the name into the question
    /// </summary>
    /// <param name="text">The text to be altered.</param>
    /// <returns>The 'fixed' text. </returns>
    private string FixQuestionText(string text)
    {
        string[] splitEllements = text.Split(new string[] { "[NAME]" }, StringSplitOptions.None);
        text = "";
        for (int i = 0; i < splitEllements.Length; i++)
        {
            text += splitEllements[i];
            if (i < splitEllements.Length - 1)
            {
                text += filledInName;
            }
        }
        return text;
    }

    /// <summary>
    /// What should happen when a button is pressed.
    /// </summary>
    /// <param name="type">The string containing the type of the button that was pressed.</param>
    public void ButtonPress(string type)
    {
        switch (type)
        {
            case "Dutch":
                questionFile = "questions_dutch";
                SetQuestions(questionFile);
                SetQuestion(0);
                flagD.SetActive(false);
                flagE.SetActive(false);
                break;
            case "English":
                questionFile = "questions_english";
                SetQuestions(questionFile);
                SetQuestion(0);
                flagD.SetActive(false);
                flagE.SetActive(false);
                break;
            case "Next":
                if (questionTypes[currentQuestion] == QuestionTypes.OpenQuestion || questionTypes[currentQuestion] == QuestionTypes.OpenNameQuestion)
                {
                    int i = 0;
                    while (transform.GetChild(i).tag != "Input" && i < transform.childCount)
                    {
                        i++;
                    }
                    int j = 0;
                    while (transform.GetChild(i).GetChild(j).tag != "InputText" && j < transform.GetChild(i).childCount)
                    {
                        j++;
                    }
                    currentAnswer = transform.GetChild(i).GetChild(j).GetComponent<Text>().text;
                }
                NextQuestion(currentAnswer);
                break;
            case "Record":
                if (currentAnswer != "")
                {
                    myAudioClip = Microphone.Start("Built-in Microphone", true, 120, 44100);
                    recording = true;
                    currentAnswer = "";
                    StartCoroutine(Clock());
                    recordingSeconds = 0;
                }
                break;
            case "Stop":
                recording = false;
                if (currentAnswer == "")
                {
                    currentAnswer = FileNameGenerator(true, false);
                }
                break;
            case "Quit":
                Application.Quit();
                break;
            case "Restart":
                currentQuestion = 0;
                flagD.SetActive(true);
                flagE.SetActive(true);
                SetQuestion(-1);
                break;
            case "a0":
                currentAnswer = "1 / a / yes";
                break;
            case "a1":
                currentAnswer = "2 / b / no";
                break;
            case "a2":
                currentAnswer = "3 / c";
                break;
            case "a3":
                currentAnswer = "4 / d";
                break;
            case "a4":
                currentAnswer = "5 / e";
                break;
            case "a5":
                currentAnswer = "6 / f";
                break;
            case "a6":
                currentAnswer = "7 / g";
                break;
            case "a7":
                currentAnswer = "8 / h";
                break;
            case "a8":
                currentAnswer = "9 / i";
                break;
            case "a9":
                currentAnswer = "10 / j";
                break;


                // case a0 etc, store answers in a variable.
        }
    }

    /// <summary>
    /// Load a multiple choice question.
    /// </summary>
    /// <param name="questionNumber">Number of question that needs to be loaded.</param>
    private void MultipleChoiceQuestion(int questionNumber)
    {
        float interval = Screen.width * 0.9f;
        interval /= (answers[questionNumber].Length + 1);
        int i = 0;
        for (i = 0; i < answers[questionNumber].Length; i++)
        {
            Button a = GameObject.Instantiate(button, transform);
            //a.GetComponent<RectTransform>().position = GetComponentInParent<Transform>().position - new Vector3(((-(answers[questionNumber].Length - 1) / 2.0f) + i) * 500, 0, 0);
            a.GetComponent<RectTransform>().position = new Vector3(0, GetComponentInParent<Transform>().position.y, 0) + new Vector3(Screen.width * 0.05f + interval * (1 + i), 0, 0);
            Text b = a.transform.GetChild(0).GetComponent<Text>();
            b.text = answers[questionNumber][i];
            a.GetComponent<MenuButtonScript>().SetButtonType("a" + i.ToString());
        }

        Button nextButton = GameObject.Instantiate(button, transform);
        nextButton.GetComponent<RectTransform>().position = GetComponentInParent<Transform>().position - new Vector3(0, 350, 0);
        //Text nextButtonText = nextButton.transform.GetChild(0).GetComponent<Text>();
        //nextButtonText.text = "Next";
        nextButton.GetComponent<MenuButtonScript>().SetButtonType("Next");
    }

    /// <summary>
    /// Load an open question.
    /// </summary>
    /// <param name="questionNumber">Number of question that needs to be loaded.</param>
    private void OpenQuestion(int questionNumber)
    {
        Button nextButton = GameObject.Instantiate(button, transform);
        nextButton.GetComponent<RectTransform>().position = GetComponentInParent<Transform>().position - new Vector3(0, 350, 0);
        //Text nextButtonText = nextButton.transform.GetChild(0).GetComponent<Text>();
        //nextButtonText.text = "Next";
        nextButton.GetComponent<MenuButtonScript>().SetButtonType("Next");

        InputField textField = GameObject.Instantiate(field, transform);
        textField.GetComponent<RectTransform>().position = GetComponentInParent<Transform>().position;
        Text textFieldText = textField.transform.GetChild(0).GetComponent<Text>();
        textFieldText.text = "Please Type";
    }

    /// <summary>
    /// Load an open name question, it saves the input directly as variable too, to be used in later questions with [NAME]
    /// </summary>
    /// <param name="questionNumber">The number of the question that needs to be loaded</param>
    private void OpenNameQuestion(int questionNumber)
    {
        OpenQuestion(questionNumber);
    }

    /// <summary>
    /// Load a record question.
    /// </summary>
    /// <param name="questionNumber">Number of question that needs to be loaded.</param>
    private void RecordingQuestion(int questionNumber)
    {
        Button nextButton = GameObject.Instantiate(button, transform);
        nextButton.GetComponent<RectTransform>().position = GetComponentInParent<Transform>().position - new Vector3(0, 350, 0);
        //Text nextButtonText = nextButton.transform.GetChild(0).GetComponent<Text>();
        //nextButtonText.text = "Next";
        nextButton.GetComponent<MenuButtonScript>().SetButtonType("Next");

        Button recordButton = GameObject.Instantiate(button, transform);
        recordButton.GetComponent<RectTransform>().position = GetComponentInParent<Transform>().position - new Vector3(-200, 150, 0);
        //Text recordButtonText = recordButton.transform.GetChild(0).GetComponent<Text>();
        //recordButtonText.text = "Record";
        recordButton.GetComponent<MenuButtonScript>().SetButtonType("Record");

        Button stopButton = GameObject.Instantiate(button, transform);
        stopButton.GetComponent<RectTransform>().position = GetComponentInParent<Transform>().position - new Vector3(200, 150, 0);
        //Text stopButtonText = stopButton.transform.GetChild(0).GetComponent<Text>();
        //stopButtonText.text = "Stop";
        stopButton.GetComponent<MenuButtonScript>().SetButtonType("Stop");
    }

    /// <summary>
    /// Load a "question" that doesn't ask for an answer, like a time-wait.
    /// </summary>
    /// <param name="questionNumber">Number of question that needs to be loaded.</param>
    private void NotAQuestion(int questionNumber)
    {
        StartCoroutine(CountDownClock(notAQuestionLengths[questionNumber]));
    }

    /// <summary>
    /// Clock for the audio saving.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Clock()
    {
        yield return new WaitForSeconds(1);
        if (!recording)
        {
            Microphone.End("Built-in Microphone");

            SavWav.Save(FileNameGenerator(true, true), myAudioClip, recordingSeconds / 60f);

        }
        else {
            recordingSeconds++;
            StartCoroutine(Clock());
        }
    }

    /// <summary>
    /// Counts down the time till the next question should pop up in case of a answerless question.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator CountDownClock(int time)
    {
        yield return new WaitForSeconds(time);
        NextQuestion();
    }
    
    /// <summary>
    /// Generates filenames for audio-files and text-files, makes sure no old file gets overwritten.
    /// </summary>
    /// <param name="audio">Should it generate an audio file name?</param>
    /// <param name="newName">Should it give the name of the latest existing file?</param>
    /// <returns></returns>
    string FileNameGenerator(bool audio, bool newName)
    {
        string name = questionFile;
        if (audio)
            name += "_question_" + currentQuestion.ToString();
        name += "_person_";
        string type = ".text";
        if (audio)
        {
            type = ".wav";
        }
        int i = 0;
        
        while (System.IO.File.Exists(Path.Combine(folder, name + i.ToString() + type.ToString()))) //folder +  "\\" + name + i.ToString() + type.ToString()))
        {
            i++;
        }
        //reduce i by 1 if looking for most recent existing file.
        if (!newName && i != 0)
        {
            i--;
        }
        name += i.ToString() + type;
        return name;
    }

    /// <summary>
    /// Loads the next question and saves the answer from the previous one.
    /// </summary>
    void NextQuestion(string answer)
    {
       //save answers
        bool doesFileExistYet = true;
        if (!(answer == null || answer == ""))
        {
            if (currentQuestion == 1)
                doesFileExistYet = false;
            SaveQuestionToFile(answer, doesFileExistYet);
            if (questionTypes[currentQuestion] == QuestionTypes.OpenNameQuestion)
            {
                filledInName = answer;
            }
            NextQuestion();
        }
    }

    /// <summary>
    /// Loads the next question, doesn't save anything.
    /// </summary>
    void NextQuestion()
    {
        if (currentQuestion + 1 < questions.Length)
        {
            currentAnswer = null;
            SetQuestion(++currentQuestion);
        }
        else if (currentQuestion == questions.Length - 1)
        {
            ButtonPress("Restart");
        }
    }

    /// <summary>
    /// Reads the questions from a file, but doesn't store them in seperate variables yet.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private string LoadQuestionFile(string fileName)
    {
       string questionSet = "";
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
                    questionSet += "\n" + line;
                }
            }
            while (line != null);
            // Done reading, close the reader  
            theReader.Close();
        }
        return questionSet;
    }

    /// <summary>
    /// This method intrpetes a question file, and puts all the questions with correct types into variables so that it works in the app.
    /// </summary>
    /// <param name="fileName">The filename.</param>
    private void SetQuestions(string fileName)
    {
        LineTypes type = LineTypes.QuestionCount; //The type of the next line to be read, first line should state the number of questions to be read.
        int questionsInserted = 0; //Number of questions currently inserted into variables.
        int answersInserted = 0; //Number of answers currently inserted into variables.
        int answersToInsert = 0; //Number of answers the question should have.
        string questionSet = LoadQuestionFile(Path.Combine(folder, fileName + ".text")); //The raw string from the file.
        string[] splitEllements = questionSet.Split('\n'); //An array with all the lines of text from the file.
        foreach (string line in splitEllements)
        {
            if (line != "") //skip white lines.
            {
                if (!(line.Length > 1 && (line[0] == '/' && line[1] == '/'))) //Skip lines that start with //, as they're comments.
                {
                    //Actions depends on the type of the line.
                    switch (type)
                    {
                        case LineTypes.Question: //if it's a question, insert it into questions[], next line will state the question type.
                            questions[questionsInserted] = line;
                            questionsInserted++;
                            type = LineTypes.QuestionType;
                            break;
                        case LineTypes.QuestionType: //Insert the type of the question into questionTypes[]
                            questionTypes[questionsInserted - 1] = (QuestionTypes)System.Enum.Parse(typeof(QuestionTypes), line);
                            //Multiplechoice questions have answers to be inserted too.
                            if ((QuestionTypes)System.Enum.Parse(typeof(QuestionTypes), line) == QuestionTypes.MultipleChoice)
                            {
                                type = LineTypes.AnswerCount;
                            }
                            else if ((QuestionTypes)System.Enum.Parse(typeof(QuestionTypes), line) == QuestionTypes.NotAQuestion)//After a not a question, the next line should be how long it's in screen.
                            {
                                type = LineTypes.QuestionLength;
                            }
                            else //Other types of questions don't, and go straight to the next question.
                            {
                                type = LineTypes.Question;
                            }
                            break;
                        case LineTypes.AnswerCount: //set the answer array to the correct length, and save this number for now.
                            answersToInsert = int.Parse(line);
                            answers[questionsInserted - 1] = new string[answersToInsert];
                            type = LineTypes.Answer;
                            break;
                        case LineTypes.Answer:
                            //As long as there are answers left to insert, keep inserting them, else, turn type back to question.
                            if (answersInserted < answersToInsert)
                            {
                                answers[questionsInserted - 1][answersInserted] = line;
                                answersInserted++;
                            }
                            if (answersInserted >= answersToInsert)
                            {
                                type = LineTypes.Question;
                                answersInserted = 0;
                            }
                            break;
                        case LineTypes.QuestionLength:
                            //How long should a 'not a question' stay visible?
                            notAQuestionLengths[questionsInserted - 1] = int.Parse(line);
                            type = LineTypes.Question;
                            break;

                        case LineTypes.QuestionCount: //Sets the question array to correct length, and then goes on to the 1st question.
                            questions = new string[int.Parse(line)];
                            answers = new string[questions.Length][];
                            questionTypes = new QuestionTypes[questions.Length];
                            notAQuestionLengths = new int[questions.Length];
                            progressionBarDots = new GameObject[questions.Length];

                            float interval = (Screen.width * 0.75f) / questions.Length;
                            for (int i = 0; i < progressionBarDots.Length; i++)
                            {
                                progressionBarDots[i] = GameObject.Instantiate(progressionBarDotEmpty, transform);
                                
                                progressionBarDots[i].GetComponent<RectTransform>().position = new Vector2((Screen.width * 0.125f) + (i + 0.5f) * interval, 80);
                            }
                            
                            type = LineTypes.Question;
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Saves the questions to a file.
    /// </summary>
    /// <param name="questionAnswer">The answer to be saved</param>
    /// <param name="shouldFileExistYet">Whether it's the first question.</param>
    private void SaveQuestionToFile(string questionAnswer, bool shouldFileExistYet)
    {
        string name = "";
        StreamWriter theStream;
        FileStream file;
        //if the file should already exist
        if (shouldFileExistYet)
        {
            name = FileNameGenerator(false, false);
            string path = Path.Combine(folder, name);
            file = new FileStream(path, FileMode.Append, FileAccess.Write);
            theStream = new StreamWriter(file);
        }
        else
        {
            name = FileNameGenerator(false, true);
            string path = Path.Combine(folder, name);
            file = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            theStream = new StreamWriter(file);
        }
        theStream.WriteLine();
        theStream.WriteLine(currentQuestion.ToString() + "  " + questionAnswer);

        theStream.Close();
        file.Close();
    }
}
