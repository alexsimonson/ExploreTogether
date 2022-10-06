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
        Shoot(owner);
    }

    public void Reload(GameObject owner){
        owner.GetComponent<PlayerCombat>().audioSource.GetComponent<AudioSource>().PlayOneShot(reloadSound);
        Debug.Log("Reloading the fucking gun");
    }
}
