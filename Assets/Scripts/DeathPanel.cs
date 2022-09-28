using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathPanel : MonoBehaviour{
    private Button restartButton;
    // Start is called before the first frame update
    void Start(){
        restartButton = gameObject.transform.Find("RestartButton").gameObject.GetComponent<Button>();
        restartButton.onClick.AddListener(OnRestartButtonPressed);
    }

    void OnRestartButtonPressed(){
        // this should head over to the game mode script and restart that shit
        GameObject player = GameObject.Find("Player");
        player.GetComponent<PlayerHealth>().ResetHealth();
        GameObject.Find("GameMode").GetComponent<WaveSurvival>().ResetGameMode();
        gameObject.SetActive(false);
        player.GetComponent<Inventory>().ResetUI();
        Cursor.visible = false;
    }
}
