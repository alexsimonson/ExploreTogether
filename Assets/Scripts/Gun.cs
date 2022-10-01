using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
public class Gun : Weapon {
    
    public Gun(){
        type = Type.Weapon;
        style = Style.Gun;
    }

    public override void Attack(GameObject owner){
        RaycastHit hit;

        int layerMask = 1 << 9;
        if(Physics.Raycast(owner.transform.position, owner.transform.forward * 1000, out hit, Mathf.Infinity, layerMask)){
            Debug.DrawRay(owner.transform.position, owner.transform.forward * hit.distance, Color.red, 2.0f, false);
            hit.transform.gameObject.GetComponent<Health>().DealDamage(100);
        }else{
            Debug.DrawRay(owner.transform.position, owner.transform.forward * 1000, Color.green, 2.0f, false);
        }
    }
}
