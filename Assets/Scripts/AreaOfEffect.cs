using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AreaOfEffect : MonoBehaviour{
    public bool areaToEffect;
    private Collider[] effected;
    private int damage = 10;

    void Effect(Collider afflicted){
        if(areaToEffect){
            afflicted.gameObject.GetComponent<Health>().DealDamage(damage);
        }
    }

    void OnTriggerStay(Collider other){
        if(other.gameObject.layer==9){
            Effect(other);
        }
    }
}
