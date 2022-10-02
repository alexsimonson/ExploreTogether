using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
public class Gun : Weapon {
    
    public Gun(){
        type = Type.Weapon;
        style = Style.Gun;
    }

    public AudioClip reloadSound;

    void OnEnable(){
        attackSound = Resources.Load("Audio/gunshot", typeof(AudioClip)) as AudioClip;
        reloadSound = Resources.Load("Audio/reload", typeof(AudioClip)) as AudioClip;
    }

    public override void Attack(GameObject owner){
        RaycastHit hit;

        int layerMask = 1 << 9;
        if(Physics.Raycast(owner.GetComponent<PlayerCombat>().playerCamera.transform.position, owner.GetComponent<PlayerCombat>().playerCamera.transform.forward * 1000, out hit, Mathf.Infinity, layerMask)){
            Debug.DrawRay(owner.GetComponent<PlayerCombat>().playerCamera.transform.position, owner.GetComponent<PlayerCombat>().playerCamera.transform.forward * hit.distance, Color.red, 2.0f, false);
            hit.transform.gameObject.GetComponent<Health>().DealDamage(100);
        }else{
            Debug.DrawRay(owner.GetComponent<PlayerCombat>().playerCamera.transform.position, owner.GetComponent<PlayerCombat>().playerCamera.transform.forward * 1000, Color.green, 2.0f, false);
        }
        owner.GetComponent<PlayerCombat>().audioSource.GetComponent<AudioSource>().PlayOneShot(attackSound);
    }

    public void Reload(GameObject owner){
        owner.GetComponent<PlayerCombat>().audioSource.GetComponent<AudioSource>().PlayOneShot(reloadSound);
        Debug.Log("Reloading the fucking gun");
    }
}
