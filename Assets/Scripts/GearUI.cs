using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GearUI : InventoryUI {

    void Start(){
        hudView = gameObject.transform.GetChild(0).gameObject;
        content = hudView.transform.GetChild(0).gameObject;
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        DrawInventoryUI(manager.player_gear);
    }

    public override void DrawInventoryUI(Inventory inventory){
        // inventorySlots = new GameObject[inventory.max_slots];

        // for(int slot_index=0;slot_index<inventory.slots.Length;slot_index++){
        //     if(inventorySlots[slot_index]==null){
        //         GameObject slot_container = Instantiate(slotContainerPrefab, content.transform.position, content.transform.rotation);
        //         slot_container.transform.SetParent(content.transform, false);
        //         inventorySlots[slot_index] = slot_container;
        //         inventorySlots[slot_index].GetComponent<SlotContainer>().index = slot_index;
        //     }
        // }
        // we need to dynamically generate gear slots based on the existing gear
        if(inventory==null){
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
        if(data.GetType().ToString()!="ItemSlot"){
            Debug.LogError("Invalid update data type");
            return;
        }
        ItemSlot slot = (ItemSlot)data;
        Equipment equipped_equipment = (Equipment)slot.item;
        if(equipped_equipment!=null){
            if(equipped_equipment.type==Equipment.Type.Weapon){
                Weapon equipped_weapon = (Weapon)equipped_equipment;
                if(equipped_weapon!=null){
                    Debug.Log("Fix here");
                    // equipped_weapon.UpdateWeaponUI();
                }
                
                // we have equipped something and we should change the title
                // Gun equipped_gun = (Gun)equipped_weapon;
                // if(equipped_gun==null){
                //     gameObject.GetComponent<Text>().text = equipped_weapon.name;
                // }else{
                //     gameObject.GetComponent<Text>().text = equipped_gun.name + "\n" + equipped_gun.magazineRounds + " | " + equipped_gun.bulletCount;
                // }
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
    }

    public override void EmptySlot(int slot_index){
        // I need to make this actually just reset to an empty slot, instead of default item...
        inventorySlots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(0).GetComponent<Text>().text = null;
        inventorySlots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().sprite = null;
    }

    public override void ToggleUI(Component sender, object data){
        hudView.SetActive((bool)data);
    }
}
