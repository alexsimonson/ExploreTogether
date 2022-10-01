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

    public Weapon(){
        type = Type.Weapon;
    }

    public virtual void Attack(GameObject owner){
        Debug.Log("This shouldn't run");
    }
}
