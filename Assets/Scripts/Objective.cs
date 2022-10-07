using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour, IInteraction {

    public Item goal;
    public void Interaction(GameObject interacting){
        // check the players inventory for the dungeon pass
        int found_item = interacting.GetComponent<Inventory>().CheckInventoryForItem(goal);
        if(found_item >= 0){
            Debug.Log("We found the item, we can finish the game now.");
        }else{
            Debug.Log("We did not find the item.  Keep searching.");
        }
    }

    public string InteractionName(){
        return "Dungeon End";
    }
}
