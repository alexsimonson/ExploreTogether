using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExploreTogether {
    public class PlayerCombat : MonoBehaviour{
        public GearUI playerGearUI;
        public EquipmentSlot weaponSlot;
        public bool revoke_combat = false;
        public GameObject playerCamera;
        public GameObject crosshair;
        public GameObject audioSource;
        Manager manager;
        
        public GameObject prefab_weapon_slot;
        public GameObject prefab_wand;
        public GameObject prefab_xbow;
        public GameObject prefab_sword;

        void Start(){
            manager = GameObject.Find("Manager").GetComponent<Manager>();
            playerGearUI = manager.hud.transform.GetChild(5).gameObject.GetComponent<GearUI>();
            weaponSlot = playerGearUI.inventorySlots[9].GetComponent<SlotContainer>().inventorySlot.GetComponent<EquipmentSlot>();
            playerCamera = manager.player.transform.GetChild(0).gameObject;
            crosshair = manager.hud.transform.GetChild(6).gameObject;
            audioSource = gameObject.transform.GetChild(1).gameObject;
            audioSource.GetComponent<AudioSource>().volume = .2f;
            DrawWeaponHeld();
        }

        void Update(){
            PlayerInput();
        }

        public void DrawWeaponHeld(GameObject _prefab=null){
            prefab_wand.SetActive(false);
            prefab_xbow.SetActive(false);
            prefab_sword.SetActive(false);
            if(_prefab==null){
                return;
            }
            _prefab.SetActive(true);
        }

        void PlayerInput(){
            if(revoke_combat){
                return;
            }
            if(weaponSlot==null) return;
            if(weaponSlot.item==null){
                return;
            }
            Weapon weapon = weaponSlot.item as Weapon;
            if(weapon==null){
                return;
            }
            if(Input.GetMouseButtonDown(0)){
                weapon.Attack(gameObject);
            }
            if(Input.GetMouseButtonDown(1)){
                weapon.Secondary(gameObject);   
            }else if(Input.GetMouseButtonUp(1)){
                weapon.Secondary(gameObject);
            }
            if(Input.GetKeyDown("r")){
                Gun gun = weaponSlot.item as Gun;
                if(gun==null) return;
                gun.Reload(gameObject);
            }
            
            if ((Input.GetKeyDown("y"))){
                Debug.Log("Show me the inventory");
                manager.player_inventory.ListInventory();
                manager.player_gear.ListInventory();
            }
        }

        public void DrawGunAim(bool _isActive){
            if(crosshair!=null){
                crosshair.SetActive(_isActive);
                // if(HoldingGun() && !revoke_combat){
                //     crosshair.SetActive(true);
                // }else{
                //     crosshair.SetActive(false);
                // }
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

        public void ToggleCombat(Component sender, object data){
            revoke_combat = (bool)data;
        }

        public void UpdateWeaponHeld(Component sender, object data){
            Debug.Log("UPDATING PLAYER COMBAT WEAPON INFO");
            Weapon equipped_weapon = (Weapon)data;
            // eventually this should be overhauled to just directly take the model from the item
            if(equipped_weapon==null){
                DrawWeaponHeld();
                return;
            }else if(equipped_weapon.style==Weapon.Style.Melee){
                DrawWeaponHeld(prefab_sword);
            }else if(equipped_weapon.style==Weapon.Style.Gun){
                DrawWeaponHeld(prefab_xbow);
            }else if(equipped_weapon.style==Weapon.Style.Magic){
                DrawWeaponHeld(prefab_wand);
            }
            // might as well do this here
            if(equipped_weapon.style==Weapon.Style.Gun){
                DrawGunAim(true);
            }else{
                DrawGunAim(false);
            }
        }
    }
}
