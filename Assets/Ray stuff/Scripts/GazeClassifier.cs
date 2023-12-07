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
        // DrawLine(cameraTf.position, cameraTf.position + cameraTf.forward*10, Color.white, 2);
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
        // DrawLine(cameraTf.position, cameraTf.position + cameraTf.forward*10, Color.white, 2);

        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity)){ 
            Debug.Log(hit.collider.gameObject.tag); 
            whatHit.text = hit.collider.gameObject.tag; 

            if(hit.collider.gameObject.tag == "Event"){
                float resTime = Time.time - hit.collider.gameObject.GetComponent<Event>().time; 
                rTime.text = "Last Respnse time: " + Math.Round(resTime, 2); 
                Destroy(hit.collider.gameObject); 
                saveResponseData(resTime);
            }
        }
        
        }

    
    

    // void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    // {
        
    //     LineRenderer.startColor = color;
    //     LineRenderer.endColor = color;
 
    //     // set width of the renderer
    //     LineRenderer.startWidth = 0.3f;
    //     LineRenderer.endWidth = 0.3f;
 
    //     // set the position
    //     LineRenderer.SetPosition(0, start);
    //     LineRenderer.SetPosition(1, end);
    // // GameObject myLine = new GameObject();
    // // myLine.transform.position = start;
    // // myLine.AddComponent();
    // // LineRenderer lr = myLine.GetComponent();
    // // lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
    // // lr.SetColors(color, color);
    // // lr.SetWidth(0.1f, 0.1f);
    // // lr.SetPosition(0, start);
    // // lr.SetPosition(1, end);
    // // GameObject.Destroy(myLine, duration);
    // }

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
