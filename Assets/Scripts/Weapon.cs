using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : Equipment {

    public enum Style{
        Melee,
        Gun,
        Magic
    }

    public Style style;
    public AudioClip attackSound;
    public int damage = 10;
    
    public Weapon(){
        type = Type.Weapon;
        name = "weapon";
    }

    public virtual void Attack(GameObject owner){
        Debug.Log("This shouldn't run");
    }

    // this is just the right click option
    public virtual void Secondary(GameObject owner){
        Debug.Log("Secondary");
    }

    public void Stab(GameObject owner){
        Debug.Log("Getting up close and personal");
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
    }

    public void Shoot(GameObject owner){
        RaycastHit hit;

        int layerMask = 1 << 9;
        if(Physics.Raycast(owner.GetComponent<PlayerCombat>().playerCamera.transform.position, owner.GetComponent<PlayerCombat>().playerCamera.transform.forward * 1000, out hit, Mathf.Infinity, layerMask)){
            Debug.DrawRay(owner.GetComponent<PlayerCombat>().playerCamera.transform.position, owner.GetComponent<PlayerCombat>().playerCamera.transform.forward * hit.distance, Color.red, 2.0f, false);
            if(hit.transform.gameObject.GetComponent<Health>()!=null){
                hit.transform.gameObject.GetComponent<Health>().DealDamage(100, hit);
            }
        }else{
            Debug.DrawRay(owner.GetComponent<PlayerCombat>().playerCamera.transform.position, owner.GetComponent<PlayerCombat>().playerCamera.transform.forward * 1000, Color.green, 2.0f, false);
        }
        owner.GetComponent<PlayerCombat>().audioSource.GetComponent<AudioSource>().PlayOneShot(attackSound);
    }
}
