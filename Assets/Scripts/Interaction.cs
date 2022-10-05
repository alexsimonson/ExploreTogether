using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour {

    public enum Type{
        Item,
        Resource,
        Spawner
    }

    public Type interaction_type;
    public Item interaction_reward;
    public void InteractWith(GameObject interacting){
        if(interaction_type==Type.Item){
            // we should pick up this item into our inventory
            if(interaction_reward){
                interacting.GetComponent<Inventory>().AddItem(interaction_reward);
            }else{
                Debug.Log("No item to reward lol");
            }
        }else if(interaction_type==Type.Resource){
            // we should harvest the resource
        }else if(interaction_type==Type.Spawner){
            interacting.GetComponent<Inventory>().AddItem(gameObject.GetComponent<ItemSpawner>().GenerateItem());
        }
    }
}