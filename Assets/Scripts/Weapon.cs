using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : Equipment {

    public Weapon(){
        type = Type.Weapon;
    }

    public void Attack(){
        Debug.Log("Attacking");
    }
}
