using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Magic", menuName = "Magic")]
public class Magic : Weapon {

    public enum MagicStyle{
        Blood,
        Frost,
        Fire,
        Earth
    }

    public MagicStyle magicStyle;

    public Magic(){
        type = Type.Weapon;
        style = Style.Magic;
        magicStyle = MagicStyle.Blood;
    }
    public int max_damage = 50;
    public float timer = 5f;
    public bool is_dot = false;
    public int last_damage = 0;
    public GameObject weapon_owner = null;

    public override void Attack(GameObject owner){
        weapon_owner = owner;
        last_damage = Random.Range(0, max_damage);
        owner.GetComponent<PlayerGhostAim>().HandlePortalPlacement(this);
        // if(magicStyle==MagicStyle.Blood){
        //     BloodMagic(owner);
        // }else if(magicStyle==MagicStyle.Frost){
        //     FrostMagic(owner);
        // }else if(magicStyle==MagicStyle.Fire){
        //     FireMagic(owner);
        // }else if(magicStyle==MagicStyle.Earth){
        //     EarthMagic(owner);
        // }
    }

    public override void Secondary(GameObject owner){
        Debug.Log("We should aim");
        owner.GetComponent<PlayerGhostAim>().TogglePortal();
    }

    public void BloodMagic(){
        weapon_owner.GetComponent<PlayerHealth>().HealthAmount(last_damage);
    }

    public void FrostMagic(){
        Debug.Log("Freeze Here");
    }

    public void FireMagic(){
        Debug.Log("Fire Here");
    }

    public void EarthMagic(){
        Debug.Log("Earth Here?");
    }
    // depending on type of magic, we do some magic?
    // how to assign blood/frost/etc.
}
