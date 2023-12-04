using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataProcessing : MonoBehaviour
{
    // Start is called before the first frame update


    public string path = "Assets/Data/";
    public string gazeFileName = "gazeData.csv";
    public string responseFileName = "responseData.csv";
    
    public int[,] gazeBoxes = {{15, 15},
                                {50, 30},
                                {180, 180}}; 

    public float[] boxScores = {10, 5, 0};


    public float responseScoreFactor = 1; // per second
    public float responseTimeout = 5; // seconds


    string gazefile;
    string responsefile;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    int[] parseGazeData(){
        
        string[] lines = System.IO.File.ReadAllLines(path + gazeFileName);
        float x, y; 
        int[] classifications = new int[lines.Length];; 
        
        for(int i=0; i < lines.Length; i++){
            string[] entries = lines[i].Split(',');
            x = float.Parse(entries[0]);
            y = float.Parse(entries[1]);
            classifications[i] = classifyGaze(x, y);
            
            Debug.Log(entries[0] + " " + entries[1] + " " + entries[2]); 
        }

        return classifications; 
    }
    int classifyGaze(float x, float y){
        int res = 0; 
 
        for(int i =0; i < gazeBoxes.Length; i++){
            if(x < gazeBoxes[i,0] && y < gazeBoxes[i,1] && x > -gazeBoxes[i,0] && y > -gazeBoxes[i,1]){
                res = i; 
                break; 
            }
        }
        return res; 
    }

    float processGazeData(){

        int[] classifications = parseGazeData();
        
        float avg = 0;
        foreach(int c in classifications){
            avg += boxScores[c]; 
        }
        avg = avg/classifications.Length;
        Debug.Log("Average: " + avg);

        return avg;
    }

    float[] getResponseScores(){
        
        string[] lines = System.IO.File.ReadAllLines(path + responseFileName);
        float[] responseScores = new float[lines.Length];;

        for(int i=0; i < lines.Length; i++){
            string[] entries = lines[i].Split(',');
            float responseTime = float.Parse(entries[0]);
            responseScores[i] = calcResponseScore(responseTime);

            Debug.Log(entries[0] + " " + entries[1]); 
        }
        return responseScores; 
    }

    float calcResponseScore(float responseTime){
        float score = 0; 
        if(responseTime > responseTimeout){
            score = 0; 
        }
        else{
            score = responseScoreFactor*responseTime; 
        }
        Debug.Log("Response Score: " + score);
        return score;
    }


    float processResponseTimes(){
        float[] scores = getResponseScores();

        float avg = 0;
        foreach(float s in scores){
            avg += s; 
        }   
        avg = avg/scores.Length;
        Debug.Log("Average: " + avg);
        return avg; 

    }


}
