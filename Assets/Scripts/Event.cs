using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour
{
    // Start is called before the first frame update
    public float time;
    public string eventName;
    void Start()
    {
        
    }

    void OnEnable()
    {
        time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
