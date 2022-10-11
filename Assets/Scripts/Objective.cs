using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour, IInteraction {
    public Item goal;
    public Manager manager;
    public GameObject objective_panel;

    void Start(){
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        objective_panel = manager.hud.transform.GetChild(7).gameObject;
    }
    public void Interaction(GameObject interacting){
        // check the players inventory for the dungeon pass
        int found_item_index = interacting.GetComponent<Inventory>().CheckInventoryForItem(goal);
        if(found_item_index >= 0){
            // we should remove the dungeon key from the index
            // manager.player.GetComponent<PlayerLook>().RevokeLook();
            // manager.player.GetComponent<PlayerCombat>().RevokeCombat();
            interacting.GetComponent<Inventory>().RemoveItem(found_item_index); // this doesn't update the UI, but it's ok for now...
            objective_panel.SetActive(true);
            Cursor.visible = true;
            // interacting.GetComponent<Inventory>().HandlePlayerRights();
            return;
        }
    }

    public string InteractionName(){
        return "Dungeon End";
    }
}
