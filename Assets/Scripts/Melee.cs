using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Melee", menuName = "Melee")]
public class Melee : Weapon {
    
    public Melee(){
        type = Type.Weapon;
        style = Style.Melee;
    }

    public override void Attack(GameObject owner){
        Debug.Log("Getting up close and personal");
        RaycastHit hit;

        int layerMask = 1 << 9;
        if(Physics.Raycast(owner.GetComponent<PlayerCombat>().playerCamera.transform.position, owner.GetComponent<PlayerCombat>().playerCamera.transform.forward * 1, out hit, 4, layerMask)){
            Debug.DrawRay(owner.GetComponent<PlayerCombat>().playerCamera.transform.position, owner.GetComponent<PlayerCombat>().playerCamera.transform.forward * hit.distance, Color.red, 2.0f, false);
            hit.transform.gameObject.GetComponent<Health>().DealDamage(100);
        }else{
            Debug.DrawRay(owner.GetComponent<PlayerCombat>().playerCamera.transform.position, owner.GetComponent<PlayerCombat>().playerCamera.transform.forward * 1, Color.green, 2.0f, false);
        }
        // owner.GetComponent<PlayerCombat>().audioSource.GetComponent<AudioSource>().PlayOneShot(attackSound);
    }

    public override void Secondary(GameObject owner){
        Debug.Log("We should block?");
    }
}
