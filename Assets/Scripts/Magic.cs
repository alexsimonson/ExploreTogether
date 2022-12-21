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

    public override void Attack(GameObject owner){
        Debug.Log("Casting the spell");
        owner.GetComponent<PlayerGhostAim>().HandlePortalPlacement();
        if(magicStyle==MagicStyle.Blood){
            BloodMagic(owner);
        }else if(magicStyle==MagicStyle.Frost){
            FrostMagic(owner);
        }else if(magicStyle==MagicStyle.Fire){
            FireMagic(owner);
        }else if(magicStyle==MagicStyle.Earth){
            EarthMagic(owner);
        }
    }

    public override void Secondary(GameObject owner){
        Debug.Log("We should aim");
        owner.GetComponent<PlayerGhostAim>().TogglePortal();
    }

    public void BloodMagic(GameObject owner){
        Debug.Log("Heal Here");
    }

    public void FrostMagic(GameObject owner){
        Debug.Log("Freeze Here");
    }

    public void FireMagic(GameObject owner){
        Debug.Log("Fire Here");
    }

    public void EarthMagic(GameObject owner){
        Debug.Log("Earth Here?");
    }
    // depending on type of magic, we do some magic?
    // how to assign blood/frost/etc.
}
