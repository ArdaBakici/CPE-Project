using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Object point;
    float time = 0;  
    // Start is called before the first frame update
    void Start()
    {
        Object temp =  Object.Instantiate(point, new Vector3(Random.Range(-5, 5), Random.Range(0, 10), 5), new Quaternion(0,0,0,0), this.transform);
    
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - time > 3){
            Object.Instantiate(point, new Vector3(Random.Range(-5, 5), Random.Range(0, 10), 5), new Quaternion(0,0,0,0), this.transform);
            time = Time.time; 

        }
        
    }
}
