using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExploreTogether {
    public class DungeonEntrance : MonoBehaviour, IInteraction {

        Manager manager;

        [Header("Events")]
        public GameEvent onGameStateChanged;

        void Start(){
            manager = GameObject.Find("Manager").GetComponent<Manager>();
        }    

        public void Interaction(GameObject interacting){
            // check the players inventory for the dungeon pass
            if(manager.current_game_state==Manager.GameState.Hub){
                // we should enter new dungeon
                // this dungeon should have generated upon exiting...
                onGameStateChanged.Raise(this, Manager.GameState.Alive);
            }else{
                // we should enter the hub
                onGameStateChanged.Raise(this, Manager.GameState.Hub);
            }
            manager.player.GetComponent<PlayerMovement>().DebugJump();
        }

        public string InteractionName(){
            // should be different depending on being in hub vs. dungeon
            return "Dungeon Entrance";
        }
    }
}
