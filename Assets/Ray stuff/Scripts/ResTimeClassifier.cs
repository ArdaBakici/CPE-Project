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
<<<<<<<< HEAD:Assets/Scripts/ResTimeClassifier.cs
    StreamWriter responseWriter;
========
>>>>>>>> 3c6362a72b05a489049017006f19d21c588c32a3:Assets/Ray stuff/Scripts/ResTimeClassifier.cs

    void Start()
    {
        responsefile = path + responseFileName;
        if (File.Exists(responsefile))
        {
            File.Delete(responsefile);
        }
<<<<<<<< HEAD:Assets/Scripts/ResTimeClassifier.cs
========

>>>>>>>> 3c6362a72b05a489049017006f19d21c588c32a3:Assets/Ray stuff/Scripts/ResTimeClassifier.cs
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
<<<<<<<< HEAD:Assets/Scripts/ResTimeClassifier.cs
        responseWriter = new StreamWriter(responsefile, true);
        string responseTime = name + "," + resTime;
========
        string responseTime = "" + resTime + "\n";

        StreamWriter responseWriter = new StreamWriter(responsefile, true);
>>>>>>>> 3c6362a72b05a489049017006f19d21c588c32a3:Assets/Ray stuff/Scripts/ResTimeClassifier.cs
        responseWriter.WriteLine(responseTime);
        responseWriter.Close();
        // File.AppendAllText(responsefile, responseTime); 
    }

}