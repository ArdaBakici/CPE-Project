using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class QuizResult : MonoBehaviour
{
    TextMeshProUGUI resultText;
    public string QASaveFilename = "QASave.csv";
    // Start is called before the first frame update
    void Start()
    {
        resultText = GetComponent<TextMeshProUGUI>();
        string QASaveFile = Application.persistentDataPath + QASaveFilename;
        string printTxt = "\n";
        string[] lines = File.ReadAllLines(QASaveFile);
        string[] entries; 
        for(int i=0; i < lines.Length; i++){
            entries = lines[i].Split(',');
            printTxt += "Q" + (i+1) + ": " + (entries[0] == "1" ? "True" : "False") + "\t"; 
        }
        resultText.text = printTxt;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
