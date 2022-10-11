using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class DeathPanel : MonoBehaviour{
    private Button restartButton;
    Manager manager;
    // Start is called before the first frame update
    void Start(){
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        restartButton = gameObject.transform.Find("RestartButton").gameObject.GetComponent<Button>();
        restartButton.onClick.AddListener(OnRestartButtonPressed);
    }

    void OnRestartButtonPressed(){
        // pop the transition panel
        manager.hud.transform.GetChild(8).gameObject.SetActive(true);
        SceneManager.LoadScene("Scenes/Maze");
        // manager.player.GetComponent<PlayerHealth>().ResetHealth();
        // manager.game_rules.ResetGameMode();
        // manager.player.GetComponent<Inventory>().Clear();
        // manager.player.GetComponent<Gear>().Clear();
        // manager.player.GetComponent<Inventory>().ResetUI();
        Cursor.visible = false;
        gameObject.SetActive(false);
    }
}
