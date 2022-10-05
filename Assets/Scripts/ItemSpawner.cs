using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour {

    Item[] item_bank;
    public Item item_offered;
    
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

    public void Interaction(GameObject interacting){
        item_offered = gameObject.GetComponent<ItemSpawner>().GenerateItem();
        interacting.GetComponent<Inventory>().AddItem(item_offered);
    }
}
