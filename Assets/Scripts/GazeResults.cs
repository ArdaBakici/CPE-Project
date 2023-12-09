using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
public class GazeResults : MonoBehaviour
{
    TextMeshProUGUI resultText;
    public string responseFileName = "responseData.csv";

    // Start is called before the first frame update
    void Start()
    {
        resultText = GetComponent<TextMeshProUGUI>();
        string responsefile = Application.persistentDataPath + responseFileName;
        string printTxt = "\n";
        string[] lines = File.ReadAllLines(responsefile);
        string[] entries; 
        for(int i=0; i < lines.Length; i++){
            entries = lines[i].Split(',');
            printTxt += "Distraction " + entries[0] + ": " + entries[1] + "\t"; 
        }
        resultText.text = printTxt;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
