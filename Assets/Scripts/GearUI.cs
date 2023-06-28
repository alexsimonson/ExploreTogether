using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ExploreTogether {
    public class GearUI : InventoryUI {

        [Header("Events")]
        public GameEvent onWeaponChanged;   // this doesn't apply to the InventoryUI

        public Gear watching_gear;
        public bool storage_ui = true;

        void Start(){
            hudView = gameObject.transform.GetChild(0).gameObject;
            content = hudView.transform.GetChild(0).gameObject;
            manager = GameObject.Find("Manager").GetComponent<Manager>();
            if(storage_ui){
                // this should be set elsewhere
            }else{
                watching_gear = manager.player_gear;
            }
            DrawInventoryUI();
        }

        public override void DrawInventoryUI(){
            // we need to dynamically generate gear slots based on the existing gear
            if(watching_gear==null){
                Debug.Log("No gear set");
                return;
            }
            if(inventorySlots==null){
                Debug.Log("No gear slots");
                return;
            }
            // since the gear slots won't be determined or drawn dynamically, this is less important
        }

        public override void UpdateSlot(Component sender, object data){
            if(data.GetType().ToString()!="ExploreTogether.ItemSlot"){
                Debug.LogError("Invalid update data type: " +data.GetType().ToString());
                return;
            }
            Debug.Log("Data type: " + data.GetType().ToString());
            ItemSlot slot = (ItemSlot)data;
            if(slot.item!=null){
                Debug.Log("slot item type: " + slot.item.GetType().ToString());
                Equipment equipped_equipment = slot.item as Equipment;
                if(equipped_equipment!=null){
                    if(equipped_equipment.type==Equipment.Type.Weapon){
                        Weapon equipped_weapon = (Weapon)equipped_equipment;
                        if(equipped_weapon!=null){
                            onWeaponChanged.Raise(this, equipped_weapon);
                        }
                    }
                }
            }
            
            Color newColor = inventorySlots[slot.index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().color;

            // ideally only this code should run in every instance
            if(slot.item==null){
                newColor.a = 0;
                inventorySlots[slot.index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(0).GetComponent<Text>().text = null;
                inventorySlots[slot.index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().sprite = null;
                inventorySlots[slot.index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(2).GetComponent<Text>().text = null;
            }else{
                newColor.a = 1;
                inventorySlots[slot.index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(0).GetComponent<Text>().text = slot.item.name;
                inventorySlots[slot.index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().sprite = slot.item.icon;
                inventorySlots[slot.index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(2).GetComponent<Text>().text = slot.stack_size.ToString();
            }
            inventorySlots[slot.index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().color = newColor;
            watching_gear.slots[slot.index].item = slot.item;
        }

        public override void EmptySlot(int slot_index){
            // I need to make this actually just reset to an empty slot, instead of default item...
            inventorySlots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(0).GetComponent<Text>().text = null;
            inventorySlots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().sprite = null;
        }

        public override void ToggleUI(Component sender, object data){
            hudView.SetActive((bool)data);
        }

        public void SetWatchingGearByReference(ref Gear gear){
            watching_gear = gear;
        }

        // this function should very simply loop through all items and re-render them
        public void UpdateGearSlots(){
            foreach(ItemSlot slot in watching_gear.slots){
                HandleSlotUpdate(slot);
            }
        }
    }
}
