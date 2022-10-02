using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour{
    public GearUI playerGearUI;
    public EquipmentSlot weaponSlot;
    public bool revoke_combat = false;
    public GameObject playerCamera;
    public GameObject crosshair;
    public GameObject audioSource;

    void Start(){
        playerGearUI = GameObject.Find("Canvas").transform.GetChild(5).gameObject.GetComponent<GearUI>();
        weaponSlot = playerGearUI.inventorySlots[9].GetComponent<SlotContainer>().inventorySlot.GetComponent<EquipmentSlot>();
        playerCamera = gameObject.transform.GetChild(0).gameObject;
        crosshair = GameObject.Find("Canvas").transform.GetChild(6).gameObject;
        audioSource = gameObject.transform.GetChild(1).gameObject;
    }

    void Update(){
        DrawGunAim();
        PlayerInput();
    }

    void PlayerInput(){
        if(revoke_combat){
            return;
        }
        if(weaponSlot.item==null){
            return;
        }
        if(Input.GetMouseButtonDown(0)){
            Weapon weapon = weaponSlot.item as Weapon;
            if(weapon==null){
                return;
            }
            weapon.Attack(gameObject);
        }
        if(Input.GetKeyDown("r")){
            Gun gun = weaponSlot.item as Gun;
            if(gun==null){
                return;
            }
            gun.Reload(gameObject);
        }
    }

    public void DrawGunAim(){
        if(HoldingGun() && !revoke_combat){
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.TransformDirection(Vector3.forward) * 1000, Color.green, Time.deltaTime);
            crosshair.SetActive(true);
        }else{
            crosshair.SetActive(false);
        }
    }

    public bool HoldingGun(){
        Weapon weapon = weaponSlot.item as Weapon;
        if(weapon==null){
            return false;
        }
        if(weapon.style==Weapon.Style.Gun){
            return true;
        }
        return false;
    }

    public void RevokeCombat(){
        revoke_combat = true;
    }

    public void AllowCombat(){
        revoke_combat = false;
    }
}
