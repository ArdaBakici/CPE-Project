using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class DisTimeClassifier : MonoBehaviour
{
    // Start is called before the first frame update
    public string responseFileName = "responseData.csv";
    string responsefile;
    StreamWriter responseWriter;

    void Start()
    {
        responsefile = Application.persistentDataPath + responseFileName;
        if (File.Exists(responsefile)) //  Reseeting File at the start of the game
        {
            File.Delete(responsefile);
        }

        responseWriter = new StreamWriter(responsefile, true);
        string responseTime = "";
        responseWriter.WriteLine(responseTime);
        responseWriter.Close();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnTriggerEnter(Collider collision) // Called when Gaze Cube collides with any object
    {   
        // If Gaze Cube collides with any object with tag "Event", then calculate response time and save it to file
        
        if(collision.gameObject.tag == "Event")
        {
            float resTime = Time.time - collision.gameObject.GetComponent<Event>().time;
            string eventName = collision.gameObject.GetComponent<Event>().eventName;
            Debug.Log("Response time: " + resTime); 

            collision.gameObject.transform.parent.gameObject.SetActive(false); // Deactivates the Event object

            saveResponseData(resTime, eventName);
        }


    }
    void saveResponseData(float resTime, string name) // Saves event name and response time to file
    {
        responseWriter = new StreamWriter(responsefile, true);
        string responseTime = name + "," + resTime;
        responseWriter.WriteLine(responseTime);
        responseWriter.Close();
    }

}