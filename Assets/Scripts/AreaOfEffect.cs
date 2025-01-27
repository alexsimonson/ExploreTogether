using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ExploreTogether {
    public class AreaOfEffect : MonoBehaviour{
        public bool areaToEffect;
        private Collider[] effected;
        public Magic weapon = null;

        void Effect(Collider afflicted){
            Debug.Log("COLLIDER AFFLICTED: " + afflicted.name);
            if(!areaToEffect){
                return;
            }
            if(afflicted.tag!="NPC"){
                return;
            }
            Debug.Log("WE FOUND THE CORRECT NPC FOR AFFLICTION");
            afflicted.gameObject.GetComponent<Health>().DealDamage(weapon.last_damage);
            if(weapon.magicStyle==Magic.MagicStyle.Frost){
                afflicted.gameObject.GetComponent<GenericNPC>().FreezeNPC(weapon.timer);
            }else if(weapon.magicStyle==Magic.MagicStyle.Blood){
                weapon.BloodMagic();
            }
        }

        void OnTriggerStay(Collider other){
            if(weapon==null) return;
            if(!weapon.is_dot) return;
            if(other.gameObject.layer==9){
                Effect(other);
            }
        }

        void OnTriggerEnter(Collider other){
            if(weapon==null) return;
            if(weapon.is_dot) return;
            if(other.gameObject.layer==9){
                Effect(other);
            }
        }
    }
}
