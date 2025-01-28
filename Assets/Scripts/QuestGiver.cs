using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExploreTogether {
    public class QuestGiver : MonoBehaviour, IInteraction {
        
        public new string name;
        Manager manager;

        // private Animator animator;

        void Start(){
            
        }

        public void Interaction(GameObject interactingWith){
            Debug.Log("Interaction via ??? interacting With: " + interactingWith.name);
        }

        public string InteractionName(){
            return name;
        }

        public void Initialize(){
            Debug.Log("Initializing quest giver");
        }
    }
}
