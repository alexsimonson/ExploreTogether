using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour, IInteraction {

    public Item item;
    private MeshRenderer mesh_renderer;
    Manager manager;
    
    void Start(){
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        mesh_renderer = gameObject.GetComponent<MeshRenderer>();
    }

    public void Interaction(GameObject interacting){
        if(item){
            manager.player_inventory.AddItem(item);
            Destroy(gameObject);
        }else{
            Debug.Log("No item to reward lol");
        }
    }

    public string InteractionName(){
        return item.name;
    }
}
