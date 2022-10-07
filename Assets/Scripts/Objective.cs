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
        int found_item = interacting.GetComponent<Inventory>().CheckInventoryForItem(goal);
        if(found_item >= 0){
            Debug.Log("We found the item, we can finish the game now.");
            objective_panel.SetActive(true);
        }else{
            Debug.Log("We did not find the item.  Keep searching.");
        }
    }

    public string InteractionName(){
        return "Dungeon End";
    }
}
