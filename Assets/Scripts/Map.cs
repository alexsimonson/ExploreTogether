using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ExploreTogether {
    public class Map : MonoBehaviour {

        public Manager manager;
        bool controlled_render = true;

        void Start(){
            manager = GameObject.Find("Manager").GetComponent<Manager>();
        }

        // Start is called before the first frame update
        void Update(){
            if(controlled_render){
                controlled_render = false;
                manager.MapSetupCallback();
            }
        }
    }
}
