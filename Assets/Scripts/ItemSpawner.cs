using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour, IInteraction {

    Item[] item_bank;
    public Item item_offered;
    
    void Awake(){
        item_bank = Resources.LoadAll<Item>("Items");
        item_offered = GenerateItem();
    }

    public Item GenerateItem(){
        int rnd_index = Random.Range(0, item_bank.Length);
        return item_bank[rnd_index];
    }

    public void Interaction(GameObject interacting){
        interacting.GetComponent<Inventory>().AddItem(item_offered);
        item_offered = gameObject.GetComponent<ItemSpawner>().GenerateItem();
    }

    public string InteractionName(){
        return item_offered.name;
    }
}
