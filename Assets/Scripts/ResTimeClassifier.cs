using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class ResTimeClassifier : MonoBehaviour
{
    // Start is called before the first frame update
    public Text rTime;
    public string path = "Assets/Data/";
    public string responseFileName = "responseData.csv";
    string responsefile;
    StreamWriter responseWriter;

    void Start()
    {
        responsefile = path + responseFileName;
        if (File.Exists(responsefile))
        {
            File.Delete(responsefile);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnTriggerEnter(Collider collision)
    {   
        Debug.Log("Collision detected");
        if(collision.gameObject.tag == "Event")
        {
            float resTime = Time.time - collision.gameObject.GetComponent<Event>().time;
            string eventName = collision.gameObject.GetComponent<Event>().eventName;
            rTime.text = "Last Response time: " + Math.Round(resTime, 2);

            Debug.Log("Response time: " + resTime); 

            Destroy(collision.gameObject);

            saveResponseData(resTime, eventName);
        }


    }
    void saveResponseData(float resTime, string name)
    {
        responseWriter = new StreamWriter(responsefile, true);
        string responseTime = name + "," + resTime;
        responseWriter.WriteLine(responseTime);
        responseWriter.Close();
        // File.AppendAllText(responsefile, responseTime); 
    }

}