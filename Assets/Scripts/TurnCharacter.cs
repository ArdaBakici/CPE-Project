using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCharacter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    
    }

    void onEnable(){
        transform.RotateAround(transform.position, transform.up, 96f);
    }

    //5.28-0.114
    // 0.218
    //0.114
    // -0.104 544.489 545.13
    // Update is called once per frame
    void Update()
    {
        
    }
}
