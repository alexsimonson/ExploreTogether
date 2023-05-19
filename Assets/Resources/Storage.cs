using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour, IInteraction {
    
    public new string name;
    Manager manager;
    public Inventory storage_inventory;


    void Start(){
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        storage_inventory = ScriptableObject.CreateInstance("Inventory") as Inventory;
    }

    public void Interaction(GameObject interactingWith){
        // we should display the storage inventory to the player
        // this doesn't exist yet
        Debug.Log("Storage interaction");
    }

    public string InteractionName(){
        return name;
    }
}
