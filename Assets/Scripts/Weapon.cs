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

    public Weapon(){
        type = Type.Weapon;
    }

    public virtual void Attack(GameObject owner){
        Debug.Log("This shouldn't run");
    }

    // this is just the right click option
    public virtual void Secondary(GameObject owner){
        Debug.Log("Secondary");
    }
}
