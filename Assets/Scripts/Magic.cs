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
    }
}
