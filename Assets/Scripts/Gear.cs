using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExploreTogether {
    public class Gear : Inventory, IInventory {

        GameObject content;
        void Awake(){
            max_slots=12;
            slots = new ItemSlot[max_slots];
            // this initialization is required, otherwise the array slots themselves are NULL, and not an ItemSlot...
            for(int i=0;i<max_slots;i++){
                slots[i] = ScriptableObject.CreateInstance("ItemSlot") as ItemSlot;
                slots[i].item = null;
                slots[i].stack_size = 0;
                slots[i].index = i;
            }
            onInventoryChanged = Resources.Load("Events/EquipmentChanged", typeof(GameEvent)) as GameEvent;
            onStorageChanged = Resources.Load("Events/StorageChanged", typeof(GameEvent)) as GameEvent;
            manager = GameObject.Find("Manager").GetComponent<Manager>();
        }

        // Start is called before the first frame update
        void Start(){
            ListInventory();
        }
        
        public override void Initialize(){
            
        }

        public override void AddItem(Item new_item, bool isStorage=false){
            if(new_item.stack){
                // we should FindItemInSlot
                int slot_index = FindItemInSlot(new_item);
                if(slot_index < 0){
                    // there was no slot found containing this item, we should add to the next available slot
                    AddItemCheck(new_item);
                }else{
                    slots[slot_index].stack_size++;
                    // this data needs passed to the UI, so it can update
                }
            }else{
                AddItemCheck(new_item);
            }
        }

        public override void RemoveItem(int index, bool shouldDrop=false){
            Item removedItem = slots[index].item;
            slots[index].item = null;
            slots[index].stack_size = 0;
            ItemSlot item_slot = ScriptableObject.CreateInstance("ItemSlot") as ItemSlot;
            item_slot = slots[index];
            if(shouldDrop){
                GameObject dropped_item = Instantiate(Resources.Load("Prefabs/Item", typeof(GameObject)) as GameObject, manager.player.transform.position, Quaternion.identity);
                dropped_item.GetComponent<ItemSpawn>().item = removedItem;
            }
            onInventoryChanged.Raise(null, item_slot);
        }

        void AddItemCheck(Item new_item){
            int empty_slot_index = FindEmptySlot();
            if(empty_slot_index >= 0){
                // we should add to this slot
                slots[empty_slot_index].item = new_item;
                slots[empty_slot_index].stack_size = 1;
                // this data needs passed to the UI, so it can update
                // manager.hud.transform.GetChild(5).gameObject.GetComponent<InventoryUI>().UpdateSlot(empty_slot_index, new_item);
            }else{
                Debug.Log("Inventory is full, cannot add item");
            }
        }

        int FindEmptySlot(){
            for(int slot_index=0;slot_index<slots.Length;slot_index++){
                if(slots[slot_index].item==null) return slot_index;
            }
            return -1;
        }

        int FindItemInSlot(Item find_item){
            for(int slot_index=0;slot_index<slots.Length;slot_index++){
                if(slots[slot_index].item.id==find_item.id && slots[slot_index].stack_size < find_item.max_stack_size) return slot_index;
            }
            return -1;
        }

        public override void ListInventory(){
            Debug.Log("~~~~~~~~~~~~LISTING THE GEAR BELOW~~~~~~~~~~~");
            foreach(ItemSlot slot in slots){
                if(slot!=null && slot.item!=null){
                    Debug.Log("Slot item: " + slot.item.name);
                }
            }
            Debug.Log("~~~~~~~~~~~~LISTING THE GEAR ABOVE~~~~~~~~~~~");
        }
    }
}
