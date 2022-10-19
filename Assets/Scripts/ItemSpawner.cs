using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour, IInteraction {

    public Item item_offered;

    Manager manager;

    void Start(){
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        item_offered = manager.GenerateItem();
    }

    public void Interaction(GameObject interacting){
        manager.player_inventory.AddItem(item_offered);
        item_offered = manager.GenerateItem();
    }

    public string InteractionName(){
        return item_offered.name;
    }
}
