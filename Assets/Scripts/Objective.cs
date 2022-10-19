using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour, IInteraction {
    public Item goal;

    Manager manager;

    [Header("Events")]
    public GameEvent onGameStateChanged;


    void Start(){
        manager = GameObject.Find("Manager").GetComponent<Manager>();
    }    
    public void Interaction(GameObject interacting){
        // check the players inventory for the dungeon pass
        int found_item_index = manager.player_inventory.CheckInventoryForItem(goal);
        if(found_item_index >= 0){
            // we should remove the dungeon key from the index
            // manager.player.GetComponent<PlayerLook>().RevokeLook();
            // manager.player.GetComponent<PlayerCombat>().RevokeCombat();
            // raise the event for game state change
            onGameStateChanged.Raise(this, Manager.GameState.Win);
            manager.player_inventory.RemoveItem(found_item_index); // this doesn't update the UI, but it's ok for now...
            Cursor.visible = true;
            return;
        }
    }

    public string InteractionName(){
        return "Dungeon End";
    }
}
