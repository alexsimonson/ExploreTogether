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

    private void OnTriggerEnter(Collider other){
        if(other.tag=="Player"){
            GenerateItem();
            other.gameObject.GetComponent<PlayerInteraction>().CanInteractWith(item_offered);
        }
    }

    private void OnTriggerExit(Collider other){
        if(other.tag=="Player"){
            other.gameObject.GetComponent<PlayerInteraction>().CanNotInteractWith(item_offered);
        }
    }

    private void GenerateItem(){
        int rnd_index = Random.Range(0, item_bank.Length);
        item_offered = item_bank[rnd_index];
    }
}
