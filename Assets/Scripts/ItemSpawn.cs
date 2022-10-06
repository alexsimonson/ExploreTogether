using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour, IInteraction {

    public Item item;
    private MeshRenderer mesh_renderer;
    
    void Start(){
        mesh_renderer = gameObject.GetComponent<MeshRenderer>();
    }

    public void Interaction(GameObject interacting){
        Debug.Log("Interacting with item spawn");
        if(item){
            interacting.GetComponent<Inventory>().AddItem(item);
            Destroy(gameObject);
        }else{
            Debug.Log("No item to reward lol");
        }
    }

    public string InteractionName(){
        return item.name;
    }
}
