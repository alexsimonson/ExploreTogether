using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour, IInteraction {
    
    public new string name;
    public Item[] resources;   // the item obtained during a successful harvest
    public bool single_use = true;
    int rng_size = 512;
    // osrs type solution for rng
    int min_bounds = 12;
    int max_bounds = 399;
    bool depleted = false;
    float respawn_time = 5f;

    public Material active;
    public Material empty;

    private MeshRenderer mesh_renderer;
    Manager manager;

    public Tool.Role required_role;


    void Start(){
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        mesh_renderer = gameObject.GetComponent<MeshRenderer>();
        mesh_renderer.material = active;
    }

    public void Interaction(GameObject interactingWith){
        int rnd_index = Random.Range(0, rng_size);
        if(depleted) return;
        // the weapon slot is index 9
        if(!(manager.player_gear.slots[9].item is Tool)){
            Debug.Log("Incorrect tool for resource.");
            return;
        }
        var interacting_tool = manager.player_gear.slots[9].item as Tool;
        if(interacting_tool.role!=required_role){
            Debug.Log("Incorrect tool for resource.");
            return;
        }
        if(rnd_index >= min_bounds && rnd_index <= max_bounds){
            if(resources.Length<0){
                Debug.Log("Must add items as reward in order to interact with resources");
                return;
            }
            Debug.Log("Reward them a resource");
            manager.player_inventory.AddItem(GiveResource());
            if(single_use){
                Depleted();
            }else{
                ChanceDepletion();
            }
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

    void Depleted(){
        depleted = true;
        StartCoroutine(Respawn(respawn_time));
    }

    void ChanceDepletion(){
        int rnd_index = Random.Range(0, rng_size);
        int min_chance = 100;
        int max_chance = 200;
        if(rnd_index >= min_chance && rnd_index <= max_chance){
            Depleted();
        }
    }

    public string InteractionName(){
        return name;
    }
}
