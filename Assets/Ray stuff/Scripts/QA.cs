using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class QA : MonoBehaviour
{

    Question[] questions;

    public Text questionText;
    public Button option1, option2, option3, option4;

    Question currQuestion; 
    int questionIdx = 0;

    public string path = "Assets/Data/";
    public string QAFilename = "QA.csv";

    string QAfile;
    // Start is called before the first frame update
    void Start()
    {

        QAfile = path + QAFilename;
        if(!File.Exists(QAfile)){
            File.Create(QAfile);
        }

        parseQAFilename("Assets/Data/QA.csv");
        displayQuestion(questions[0]);
        currQuestion = questions[0];

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    


    void parseQAFilename(string filename){
        // File Stucture: 
        // Question, Answer (int), Option1, Option2, Option3, Option4, difficulty (float)

        string[] lines = System.IO.File.ReadAllLines(filename);
        string[] entries; 
        questions = new Question[lines.Length];
        for(int i=0; i < lines.Length; i++){
            entries = lines[i].Split(',');
            Debug.Log(entries[0] + " " + entries[1] + " " + entries[2] + " " + entries[3] + " " + entries[4] + " " + entries[5] + " " + entries[6]); 
            questions[i] = new Question(entries[0], new string[]{entries[2], entries[3], entries[4], entries[5]}, int.Parse(entries[1]), float.Parse(entries[6]));
        }
    }

    void displayQuestion(Question q){
        questionText.text = q.question;
        option1.GetComponentInChildren<TextMeshProUGUI>().text = q.options[0];
        option2.GetComponentInChildren<TextMeshProUGUI>().text = q.options[1];
        option3.GetComponentInChildren<TextMeshProUGUI>().text = q.options[2];
        option4.GetComponentInChildren<TextMeshProUGUI>().text = q.options[3];
    }

    public void onClick(int answer){
        
        string ansData; 
        if(answer == currQuestion.answer){
            ansData = "1" + "," + currQuestion.difficulty + "\n"; 
            Debug.Log("Correct");
        }
        else{
            ansData = "0" + "," + currQuestion.difficulty + "\n"; 
            Debug.Log("Incorrect");
        }
        File.AppendAllText(QAfile, ansData);
        changeQuestion(); 

    }

    void changeQuestion(){
        if(questionIdx < questions.Length-1){
            questionIdx++;
            currQuestion = questions[questionIdx];
            displayQuestion(currQuestion);
        }
        else{
            Debug.Log("Done");
        }   
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
