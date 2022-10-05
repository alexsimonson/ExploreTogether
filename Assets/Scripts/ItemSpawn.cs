using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour {

    public Item item;
    public GameObject itemPrefab;

    public void Interaction(GameObject interactingWith){
        Debug.Log("Interacting with item spawn");
        interactingWith.GetComponent<PlayerInteraction>().GiveInteractWith(gameObject);
    }
}
