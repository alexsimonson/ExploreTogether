using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : Inventory, IInventory {

    GameObject content;
    void Awake(){
        max_slots=12;
        slots = new GameObject[max_slots];
    }

    // Start is called before the first frame update
    void Start(){
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        ListInventory();
    }


    // Update is called once per frame
    void Update(){
        PlayerInput();
    }
    
    public override void Initialize(){
        RelevantScrollView = manager.hud.transform.GetChild(5).gameObject;
        content = RelevantScrollView.transform.GetChild(0).GetChild(0).gameObject;
        RelevantScrollView.SetActive(false);
        int i=0;
        foreach(Transform child in content.transform){
            slots[i] = child.gameObject;
            slots[i].GetComponent<SlotContainer>().index = i;
            i++;
        }
    }

    public override void AddItem(Item new_item){
        if(new_item.stack){
            // we should FindItemInSlot
            int slot_index = FindItemInSlot(new_item);
            Debug.Log("Found slot index: " + slot_index);
            if(slot_index < 0){
                // there was no slot found containing this item, we should add to the next available slot
                AddItemCheck(new_item);
            }else{
                Debug.Log("We are increasing");
                slots[slot_index].GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().stack_size++;
                RelevantScrollView.GetComponent<InventoryUI>().UpdateSlot(slot_index, new_item);
            }
        }else{
            AddItemCheck(new_item);
        }
    }

    void AddItemCheck(Item new_item){
        int empty_slot_index = FindEmptySlot();
        if(empty_slot_index >= 0){
            // we should add to this slot
            slots[empty_slot_index].GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().item = new_item;
            slots[empty_slot_index].GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().stack_size = 1;
            // this data needs passed to the UI, so it can update
            RelevantScrollView.GetComponent<InventoryUI>().UpdateSlot(empty_slot_index, new_item);
        }else{
            Debug.Log("Inventory is full, cannot add item");
        }
    }

    int FindEmptySlot(){
        for(int slot_index=0;slot_index<slots.Length;slot_index++){
            if(slots[slot_index].GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().item==null) return slot_index;
        }
        return -1;
    }

    int FindItemInSlot(Item find_item){
        for(int slot_index=0;slot_index<slots.Length;slot_index++){
            if(slots[slot_index]!=null && slots[slot_index].GetComponent<SlotContainer>().inventorySlot!=null){
                if(slots[slot_index].GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().item!=null){
                    if(slots[slot_index].GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().item.id==find_item.id && slots[slot_index].GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().stack_size < find_item.max_stack_size){
                        Debug.Log("Slot stack size: " + slots[slot_index].GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().stack_size.ToString());
                        Debug.Log(find_item.name + " max stack size: " + find_item.max_stack_size.ToString());
                        return slot_index;
                    }
                }
            }
        }
        return -1;
    }

    public override void ListInventory(){
        foreach(GameObject slot in slots){
            if(slot!=null && slot.GetComponent<SlotContainer>().inventorySlot!=null && slot.GetComponent<SlotContainer>().inventorySlot.GetComponent<EquipmentSlot>().item!=null){
                Debug.Log("Slot item: " + slot.GetComponent<SlotContainer>().inventorySlot.GetComponent<EquipmentSlot>().item.name);
            }
        }
    }
}
