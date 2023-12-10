using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnHit : MonoBehaviour
{
    AudioSource saw;
    bool oneTime = true;
    // Start is called before the first frame update
    void Start()
    {
        saw = GameObject.FindObjectOfType<AudioSource>();
    }

    IEnumerator PlaySounds() {
        yield return new WaitForSeconds(0.1f);
        saw.Play();
    }

    void OnCollisionEnter(Collision col)
    {
        if(oneTime){
            StartCoroutine(PlaySounds());
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
