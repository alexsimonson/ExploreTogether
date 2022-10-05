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

    public void InteractWith(GameObject interacting){
        if(interaction_type==Type.Item){
            // we should pick up this item into our inventory
            gameObject.GetComponent<ItemSpawn>().Interaction(interacting);
        }else if(interaction_type==Type.Resource){
            // we should harvest the resource
            gameObject.GetComponent<Resource>().Interaction(interacting);
        }else if(interaction_type==Type.Spawner){
            gameObject.GetComponent<ItemSpawner>().Interaction(interacting);
        }
    }
}