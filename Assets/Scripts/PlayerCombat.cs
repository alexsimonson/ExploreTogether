using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour{
    public GearUI playerGearUI;
    public EquipmentSlot weaponSlot;
    public bool revoke_combat = false;

    void Start(){
        playerGearUI = GameObject.Find("Canvas").transform.GetChild(5).gameObject.GetComponent<GearUI>();
        weaponSlot = playerGearUI.inventorySlots[9].GetComponent<SlotContainer>().inventorySlot.GetComponent<EquipmentSlot>();
    }

    void Update(){
        PlayerInput();
    }

    void PlayerInput(){
        if(revoke_combat){
            return;
        }
        if(Input.GetMouseButtonDown(0)){
            if(weaponSlot.item==null){
                return;
            }else{
                Weapon weapon = weaponSlot.item as Weapon;
                if(weapon==null){
                    return;
                }
                weapon.Attack(gameObject);
            }
        }
    }

    public void RevokeCombat(){
        revoke_combat = true;
    }

    public void AllowCombat(){
        revoke_combat = false;
    }
}
