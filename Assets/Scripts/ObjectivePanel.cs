using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivePanel : MonoBehaviour {

    public Manager manager;
    public Button nextButton;
    public Button quitButton;
    // Start is called before the first frame update
    void Start(){
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        nextButton = gameObject.transform.GetChild(1).gameObject.GetComponent<Button>();
        quitButton = gameObject.transform.GetChild(2).gameObject.GetComponent<Button>();
        nextButton.onClick.AddListener(OnNextButtonPressed);
        quitButton.onClick.AddListener(OnQuitButtonPressed);
    }

    void OnNextButtonPressed(){
        // this should head over to the game mode script and restart that shit
        Debug.Log("Generate another dungeon");
        manager.game_rules.ResetGameMode();
        gameObject.SetActive(false);
    }

    void OnQuitButtonPressed(){
        Debug.Log("Show the main menu?");
    }
}
