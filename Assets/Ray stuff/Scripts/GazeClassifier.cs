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

    public string path = "Assets/Data/";
    public string gazeFileName = "gazeData.csv";
    public string responseFileName = "responseData.csv";

    string gazefile;
    string responsefile;
    StreamWriter gazeWriter;
    StreamWriter responseWriter;
    void Start()
    {
        cameraTf = GetComponentInChildren<Camera>().GetComponent<Transform>(); 
        
        gazefile = path + gazeFileName;
        responsefile = path + responseFileName; 

        if(File.Exists(gazefile)){
            File.Delete(gazefile);
        }
        if(File.Exists(responsefile)){
            File.Delete(responsefile);
        }
        // File.Create(gazefile);
        // File.Create(responsefile);
        
        gazeWriter = new StreamWriter(gazefile, true);
        responseWriter = new StreamWriter(responsefile, true);
 

    }

    // Update is called once per frame
    void Update()
    {
        gazeAngles();
        responseTimes();
    }

    void gazeAngles(){

        gazeDir = cameraTf.forward; 
        // Debug.Log("" + gazeDir.x + " " + gazeDir.y + " "+ gazeDir.z); 
        float dX = gazeDir.x/gazeDir.z; 
        float dY = gazeDir.y/gazeDir.z; 
        float thetaX = Vector3.Angle(new Vector3(0,0,1), new Vector3(gazeDir.x, 0, gazeDir.z));  
        float thetaY = Vector3.Angle(new Vector3(0,0,1), new Vector3(0, gazeDir.y, Math.Abs(gazeDir.z)));  

        // double xDeg = (thetaX*180)/Math.PI; 
        // double yDeg = (thetaY*180)/Math.PI; 
        double xDeg = thetaX; 
        double yDeg = thetaY; 
        // Debug.Log(xDeg + " " + yDeg); 
        xText.text = "X Degrees: " + Math.Round(xDeg, 2); 
        yText.text = "Y Degrees: " + Math.Round(yDeg, 2);  

        saveGazeData(thetaX, thetaY);

    }

    void responseTimes(){
        RaycastHit hit; 
        
        Ray forward = new Ray(cameraTf.position, cameraTf.forward); 

        if(Physics.Raycast(forward, out hit)){ 
        
            Debug.Log(hit.collider.gameObject.tag); 

            if(hit.collider.gameObject.tag == "Event"){
                float resTime = Time.time - hit.collider.gameObject.GetComponent<Event>().time; 
                rTime.text = "Last Respnse time: " + Math.Round(resTime, 2); 
                Destroy(hit.collider.gameObject); 
                saveResponseData(resTime);
            }
        }

    }

    void OnApplicationQuit(){
        gazeWriter.Close();
        responseWriter.Close();
    }

    void saveGazeData(float x, float y){
        string gazeData = "" + x + "," + y + "\n"; 

        gazeWriter.WriteLine(gazeData);
        // File.AppendAllText(gazefile, gazeData); 
    }
    void saveResponseData(float resTime){
        string responseTime = "" + resTime + "\n"; 
        responseWriter.WriteLine(responseTime);

        // File.AppendAllText(responsefile, responseTime); 
    }

}
