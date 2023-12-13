using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; 


public class GazeClassifier : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 gazeDir; 
    Transform cameraTf; 

    public Text xText; 
    public Text yText; 

    public Text rTime; 
    public Text whatHit; 

    public string gazeFileName = "gazeData.csv";
    public string responseFileName = "responseData.csv";

    string gazefile;
    string responsefile;
    // StreamWriter gazeWriter;
    StreamWriter responseWriter;

    void Start()
    {
        cameraTf = GetComponentInChildren<Camera>().GetComponent<Transform>(); 
        
        gazefile = Application.persistentDataPath + gazeFileName;

        if(File.Exists(gazefile)){ // Resetting File at the start of the game
            File.Delete(gazefile);
        }

    }

    // Update is called once per frame
    void Update()
    {
        gazeAngles();
    }

    void gazeAngles(){ // Calculates the angle of the Camera (w.r.t z-axis) and saves it t a file 
        gazeDir = cameraTf.forward; 

        // Calculate angles between z-axis and Forward vector of camera
        float thetaX = Vector3.Angle(new Vector3(0,0,1), new Vector3(gazeDir.x, 0, gazeDir.z));  
        float thetaY = Vector3.Angle(new Vector3(0,0,1), new Vector3(0, gazeDir.y, Math.Abs(gazeDir.z)));  
        
        // Fixing for scene: 
        thetaX = thetaX - 180;

        // Adding sign to the angle
        if(gazeDir.x < 0){
            thetaX = -thetaX; 
        }
        if(gazeDir.y < 0){
            thetaY = -thetaY; 
        }


        double xDeg = thetaX; 
        double yDeg = thetaY; 
        xText.text = "X Degrees: " + Math.Round(xDeg, 2); 
        yText.text = "Y Degrees: " + Math.Round(yDeg, 2);  
    
        saveGazeData(thetaX, thetaY);

    }

    void saveGazeData(float x, float y){ //  Saving Data to a file

        string gazeData = "" + x + "," + y + "\n"; 

        StreamWriter gazeWriter = new StreamWriter(gazefile, true); // Opening file stream
        gazeWriter.WriteLine(gazeData);
        gazeWriter.Close();

    }


}
