using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class QA : MonoBehaviour{

    Question[] questions;

    [SerializeField]
    public GameObject q_obj, res_obj;

    [SerializeField]
    public GameObject opt1obj, opt2obj, opt3obj, opt4obj;

    TextMeshProUGUI questionText, result, option1, option2, option3, option4;


    Question currQuestion; 
    int questionIdx = 0;

    public string path = "Assets/Data/";
    public string QAFilename = "QA.csv";
    public string QASaveFilename = "QASave.csv";

    string QAfile;
    string QASaveFile; 
    StreamWriter QAWriter;
    // Start is called before the first frame update
    void Start()
    {
        questionText = q_obj.GetComponent<TextMeshProUGUI>();
        result = res_obj.GetComponent<TextMeshProUGUI>();
        option1 = opt1obj.GetComponent<TextMeshProUGUI>();
        option2 = opt2obj.GetComponent<TextMeshProUGUI>();
        option3 = opt3obj.GetComponent<TextMeshProUGUI>();
        option4 = opt4obj.GetComponent<TextMeshProUGUI>();

        QAfile = path + QAFilename;
        QASaveFile = path+QASaveFilename;
        if(!File.Exists(QAfile)){
            File.Create(QAfile);
        }
        
        parseQAFile(QAfile);

        if(File.Exists(QASaveFile)){
            File.Delete(QASaveFile);
        }
        
        displayQuestion(questions[0]);
        currQuestion = questions[0];
        Debug.Log(currQuestion.question + " " + currQuestion.answer + " " + currQuestion.difficulty); 
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void parseQAFile(string filename){
        // File Stucture: 
        // Question, Answer (int), Option1, Option2, Option3, Option4, difficulty (float)

        string[] lines = File.ReadAllLines(filename);
        string[] entries; 
        questions = new Question[lines.Length];
        for(int i=0; i < lines.Length; i++){
            entries = lines[i].Split(',');
            Debug.Log(entries[0] + "|" + entries[1] + "|" + entries[2] + "|" + entries[3] + "|" + entries[4] + "|" + entries[5] + "|" + entries[6]); 
            questions[i] = new Question(entries[0], new string[]{entries[2], entries[3], entries[4], entries[5]}, int.Parse(entries[1]), float.Parse(entries[6]));
        }
    }

    void displayQuestion(Question q){
        questionText.text = "\n\n" + q.question;
        option1.text = q.options[0];
        option2.text = q.options[1];
        option3.text = q.options[2];
        option4.text = q.options[3];
    }


    public void answer(int answer){
        
        string ansData; 
        currQuestion = questions[questionIdx];
        Debug.Log(currQuestion.question + " " + currQuestion.answer + " " + currQuestion.difficulty); 
        if(answer == currQuestion.answer){
            ansData = "1" + "," + currQuestion.difficulty + "\n"; 
            Debug.Log("Correct");
            result.text = "Correct!"; 
        }
        else{
            ansData = "0" + "," + currQuestion.difficulty + "\n"; 
            Debug.Log("Incorrect");
            result.text = "Incorrect"; 
        }
        // File.AppendAllText(QASaveFilename, ansData);
        saveData(ansData);
        changeQuestion(); 
    }

    void saveData(string ansData){
        QAWriter = new StreamWriter(QASaveFile, true);
        QAWriter.WriteLine(ansData);
        QAWriter.Close();
    }

    void changeQuestion(){
        if(questionIdx < questions.Length-1){
            questionIdx++;
            currQuestion = questions[questionIdx];
            Debug.Log(currQuestion.question + " " + currQuestion.answer + " " + currQuestion.difficulty); 
            displayQuestion(currQuestion);
        }
        else{ 
            Debug.Log("Done");
            SceneManager.LoadScene("Results");
        }   
    }
    void OnApplicationQuit(){
        if(QAWriter != null)
            QAWriter.Close();
    }
}


class Question{
    public string question; 
    public string[] options; 
    public int answer; 
    public float difficulty;

    public Question(string q, string[] o, int a, float d){
        question = q; 
        options = o; 
        answer = a; 
        difficulty = d;
    }
}
