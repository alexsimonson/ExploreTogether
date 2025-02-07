using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace ExploreTogether {
    public class DeathPanel : MonoBehaviour{
        public Button restartButton;
        private Button mainMenuButton;

        Manager manager;

        [Header("Events")]
        public GameEvent onGameStateChanged;
        
        void Start(){
            manager = GameObject.Find("Manager").GetComponent<Manager>();
            // restartButton = gameObject.transform.Find("RestartButton").gameObject.GetComponent<Button>();
            restartButton.onClick.AddListener(OnRestartButtonPressed);
            mainMenuButton = gameObject.transform.Find("MainMenuButton").gameObject.GetComponent<Button>();
            mainMenuButton.onClick.AddListener(OnMainMenuButtonPressed);
        }

        void OnRestartButtonPressed(){
            // pop the transition panel
            onGameStateChanged.Raise(this, Manager.GameState.Transition);
            manager.game_mode.ResetGameMode();
            Cursor.visible = false;
        }

        void OnMainMenuButtonPressed(){
            onGameStateChanged.Raise(this, Manager.GameState.Menu);
            manager.game_mode.DestroyGameInstance();
            Cursor.visible = true;
        }
    }
}
