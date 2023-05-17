using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "New Tool", menuName = "Tool")]
public class Tool : Weapon {
    
    public Tool(){
        type = Type.Weapon;
        name = "tool";
        // style = Style.Melee; // not all tools are necessarily melee (nail gun???)
    }

    public override void Attack(GameObject owner){

        // tool functionality may be incredibly different depending on our desire
        Debug.Log("Getting up close and personal (tool time)");
        RaycastHit hit;

        int layerMask = 1 << 9;
        if(Physics.Raycast(owner.GetComponent<PlayerCombat>().playerCamera.transform.position, owner.GetComponent<PlayerCombat>().playerCamera.transform.forward * 1, out hit, 4, layerMask)){
            Debug.DrawRay(owner.GetComponent<PlayerCombat>().playerCamera.transform.position, owner.GetComponent<PlayerCombat>().playerCamera.transform.forward * hit.distance, Color.red, 2.0f, false);
            hit.transform.gameObject.GetComponent<NPC>().Knockback(owner.transform.forward);
            hit.transform.gameObject.GetComponent<Health>().DealDamage(20);
        }else{
            Debug.DrawRay(owner.GetComponent<PlayerCombat>().playerCamera.transform.position, owner.GetComponent<PlayerCombat>().playerCamera.transform.forward * 1, Color.green, 2.0f, false);
        }
        // owner.GetComponent<PlayerCombat>().audioSource.GetComponent<AudioSource>().PlayOneShot(attackSound);
        // if(style==Style.Melee){
        //     Stab();
        // }else if(style==Style.Gun){
        //     Shoot();
        // }
    }

    public override void Secondary(GameObject owner){
        Debug.Log("We should block?");
    }
}
