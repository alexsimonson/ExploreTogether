using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GearUI : InventoryUI {

    // Start is called before the first frame update
    void Awake(){
        content = gameObject.transform.GetChild(0).GetChild(0).gameObject;
        if(isPlayerInventory){
            inventory = GameObject.Find("Player").GetComponent<Inventory>();
        }
        DrawInventoryUI();
    }

    void DrawInventoryUI(){
        // we need to dynamically generate inventory slots based on the existing inventory
        if(inventory==null){
            Debug.Log("No inventory set");
            return;
        }
        if(inventory.slots==null){
            Debug.Log("No inventory slots");
            return;
        }
        // since the gear slots won't be determined or drawn dynamically, this is less important
    }

    public void UpdateSlot(int slot_index, Item new_item, GameObject test=null){
        Color newColor = inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().color;
        if(test==null){
            inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(0).GetComponent<Text>().text = new_item.name;
            inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().sprite = new_item.icon;
            newColor.a = 1;
            inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().color = newColor;
            inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(2).GetComponent<Text>().text = inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.GetComponent<EquipmentSlot>().stack_size.ToString();
        }else{
            if(new_item==null){
                test.transform.GetChild(0).GetComponent<Text>().text = null;
                test.transform.GetChild(1).GetComponent<Image>().sprite = null;
                newColor.a = 0;
                inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().color = newColor;
                inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(2).GetComponent<Text>().text = null;
            }else{
                test.transform.GetChild(0).GetComponent<Text>().text = new_item.name;
                test.transform.GetChild(1).GetComponent<Image>().sprite = new_item.icon;
                newColor.a = 1;
                inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().color = newColor;
                // inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(2).GetComponent<Text>().text = inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.GetComponent<EquipmentSlot>().stack_size.ToString();
                inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(2).GetComponent<Text>().text = null;
            }
        }
    }

    public void EmptySlot(int slot_index){
        // I need to make this actually just reset to an empty slot, instead of default item...
        inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(0).GetComponent<Text>().text = null;
        inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().sprite = null;
    }
}
