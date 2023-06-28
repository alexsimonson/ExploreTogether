using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ExploreTogether {
    [CreateAssetMenu(fileName = "New Melee", menuName = "Melee")]
    public class Melee : Weapon {
        
        public Melee(){
            type = Type.Weapon;
            style = Style.Melee;
        }

        public override void Attack(GameObject owner){
            Stab(owner);
        }

        public override void Secondary(GameObject owner){
            Debug.Log("We should block?");
        }
    }
}
