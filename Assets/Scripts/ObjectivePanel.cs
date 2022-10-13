using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivePanel : MonoBehaviour {

    public Manager manager;
    public Button nextButton;
    public Button quitButton;

    [Header("Events")]
    public GameEvent onGameStateChanged;
    
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
        onGameStateChanged.Raise(this, Manager.GameState.Transition);
        // manager.hud.transform.GetChild(8).gameObject.SetActive(true);
        manager.player.GetComponent<Inventory>().SetDisplayUI(false);
        manager.player.GetComponent<Gear>().SetDisplayUI(false);
        manager.game_rules.ProgressGameMode();
        // gameObject.SetActive(false);
    }

    void OnQuitButtonPressed(){
        Debug.Log("Show the main menu?");
    }
}
