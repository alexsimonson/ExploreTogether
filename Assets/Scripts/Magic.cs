using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Magic", menuName = "Magic")]
public class Magic : Weapon {
    
    public Magic(){
        type = Type.Weapon;
        style = Style.Magic;
    }

    public override void Attack(GameObject owner){
        Debug.Log("Casting the spell");
        owner.GetComponent<PlayerGhostAim>().HandlePortalPlacement();
    }

    public override void Secondary(GameObject owner){
        Debug.Log("We should aim");
        owner.GetComponent<PlayerGhostAim>().TogglePortal();
    }
}
