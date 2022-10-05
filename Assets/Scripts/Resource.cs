using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour {
    
    public new string name;
    public Item[] resources;   // the item obtained during a successful harvest
    bool single_use = true;
    int rng_size = 512;
    // osrs type solution for rng
    int min_bounds = 12;
    int max_bounds = 399;
    bool depleted = false;
    float respawn_time = 5f;

    public Material active;
    public Material empty;

    private MeshRenderer mesh_renderer;

    void Start(){
        mesh_renderer = gameObject.GetComponent<MeshRenderer>();
        mesh_renderer.material = active;
    }

    public void Interaction(GameObject interactingWith){
        int rnd_index = Random.Range(0, rng_size);
        if(depleted) return;
        if(rnd_index >= min_bounds && rnd_index <= max_bounds){
            Debug.Log("Reward them a resource");
            if(resources.Length<0){
                Debug.Log("Must add items as reward in order to interact with resources");
                return;
            }
            if(single_use){
                depleted = true;
                StartCoroutine(Respawn(5f));
            }
            interactingWith.GetComponent<Inventory>().AddItem(GiveResource());
        }else{
            Debug.Log("No resource for you");
        }
    }

    public Item GiveResource(){
        int rnd_index = Random.Range(0, resources.Length - 1);
        return resources[rnd_index];
    }

    IEnumerator Respawn(float respawn_time){
        mesh_renderer.material = empty;
        yield return new WaitForSeconds(respawn_time);
        depleted = false;
        mesh_renderer.material = active;
    }
}
