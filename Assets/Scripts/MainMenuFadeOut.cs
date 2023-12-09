using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class MainMenuFadeOut : MonoBehaviour
{
    [SerializeField]
    public PlayableDirector director;

    [SerializeField]
    public GameObject mainMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame(){
        director.Play();
        mainMenu.SetActive(false);
    }

    public void ExitGame(){
        Application.Quit();
    }
}
