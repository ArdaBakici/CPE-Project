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
        // responsefile = path + responseFileName; 

        if(File.Exists(gazefile)){
            File.Delete(gazefile);
        }
        // if(File.Exists(responsefile)){
        //     File.Delete(responsefile);
        // }
        // File.Create(gazefile);
        // File.Create(responsefile);
        
        // gazeWriter = new StreamWriter(gazefile, true);
        // responseWriter = new StreamWriter(responsefile, true);

 

    }

    // Update is called once per frame
    void Update()
    {
        gazeAngles();
    }

    void gazeAngles(){
        // DrawLine(cameraTf.position, cameraTf.position + cameraTf.forward*10, Color.white, 2);
        gazeDir = cameraTf.forward; 
        // Debug.Log("" + gazeDir.x + " " + gazeDir.y + " "+ gazeDir.z); 

        float thetaX = Vector3.Angle(new Vector3(0,0,1), new Vector3(gazeDir.x, 0, gazeDir.z));  
        float thetaY = Vector3.Angle(new Vector3(0,0,1), new Vector3(0, gazeDir.y, Math.Abs(gazeDir.z)));  
        
        // Fixing for scene: 
        thetaX = thetaX - 180;

        if(gazeDir.x < 0){
            thetaX = -thetaX; 
        }
        if(gazeDir.y < 0){
            thetaY = -thetaY; 
        }


        // double xDeg = (thetaX*180)/Math.PI; 
        // double yDeg = (thetaY*180)/Math.PI; 
        double xDeg = thetaX; 
        double yDeg = thetaY; 
        // Debug.Log(xDeg + " " + yDeg); 
        xText.text = "X Degrees: " + Math.Round(xDeg, 2); 
        yText.text = "Y Degrees: " + Math.Round(yDeg, 2);  
    

        
        saveGazeData(thetaX, thetaY);

    }

    void saveGazeData(float x, float y){
        string gazeData = "" + x + "," + y + "\n"; 
        StreamWriter gazeWriter = new StreamWriter(gazefile, true);
        gazeWriter.WriteLine(gazeData);
        gazeWriter.Close();
        // File.AppendAllText(gazefile, gazeData); 
    }


}
