using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Resource")]
public class Resource : ScriptableObject {
    
    public new string name;
    public Item resource;   // the item obtained during a successful harvest
    bool single_use = true;
    int rng_size = 512;
    // osrs type solution for rng
    int min_bounds = 12;
    int max_bounds = 399;

    public void Interaction(GameObject interactingWith){
        int rnd_index = Random.Range(0, rng_size);
        if(rnd_index >= min_bounds && rnd_index <= max_bounds){
            Debug.Log("Reward them a resource");
            // interactingWith.GetComponent
        }else{
            Debug.Log("No resource for you");
        }
    }
}
