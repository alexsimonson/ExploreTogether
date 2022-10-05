using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour {

    Item[] item_bank;
    Item item_offered;
    
    void Awake(){
        item_bank = Resources.LoadAll<Item>("Items");
        Debug.Log(item_bank.Length);
    }

    void Start(){
        foreach(Item item in item_bank){
            Debug.Log("Item name: " + item.name);
        }
    }

    public Item GenerateItem(){
        int rnd_index = Random.Range(0, item_bank.Length);
        return item_bank[rnd_index];
    }

    public void Interaction(){
        Debug.Log("Interacting with the item spawner");
    }
}
