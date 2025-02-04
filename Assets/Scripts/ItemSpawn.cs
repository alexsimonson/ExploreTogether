using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExploreTogether {
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
                Debug.Log("Interaction is running from itemspawn.cs");
                manager.player_inventory.AddItem(item);
                // nasty, nasty loop instead of sending in the proper index from the list based on interaction
                // could be done but big overhaul needed and bigger brain
                manager.map.GetComponent<Maze>().FindAndRemoveItem(item.id, gameObject.transform.position);
                Destroy(gameObject);
            }else{
                Debug.Log("No item to reward lol");
            }
        }

        public string InteractionName(){
            return item.name;
        }
    }
}
