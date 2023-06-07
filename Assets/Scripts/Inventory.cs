using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : ScriptableObject, IInventory {

    // THE INVENTORY IS STRICTLY DATA
    public int max_slots;  // number of items an inventory can hold before full
    public ItemSlot[] slots;

    // THE INVENTORY IS RESPONSIBLE FOR RAISING EVENTS RELATED TO THE INVENTORY
    [Header("Events")]
    public GameEvent onInventoryChanged;
    public GameEvent onStorageChanged;

    public GameObject RelevantScrollView;   // UI
    public Manager manager; // used to get the UI

    void Awake(){
        if(max_slots==0){
            max_slots=28;
        }
        slots = new ItemSlot[max_slots];
        // this initialization is required, otherwise the array slots themselves are NULL, and not an ItemSlot...
        for(int i=0;i<max_slots;i++){
            slots[i] = ScriptableObject.CreateInstance("ItemSlot") as ItemSlot;
            slots[i].item = null;
            slots[i].stack_size = 0;
            slots[i].index = i;
        }
        onInventoryChanged = Resources.Load("Events/InventoryChanged", typeof(GameEvent)) as GameEvent;
        onStorageChanged = Resources.Load("Events/StorageChanged", typeof(GameEvent)) as GameEvent;
        manager = GameObject.Find("Manager").GetComponent<Manager>();
    }

    public virtual void Initialize(){
        // RelevantScrollView = manager.hud.transform.GetChild(3).gameObject;
    }

    public virtual void AddItem(Item new_item, bool isStorage=false){
        if(new_item==null) return;
        if(new_item.stack){
            // we should FindItemInSlot
            int slot_index = FindItemInSlot(new_item);
            if(slot_index >= 0){
                ItemSlot item_slot = ScriptableObject.CreateInstance("ItemSlot") as ItemSlot;
                item_slot.index = slot_index;
                item_slot.item = new_item;
                item_slot.stack_size = slots[slot_index].stack_size;
                slots[slot_index].stack_size++;
                if(isStorage){
                    onStorageChanged.Raise(null, item_slot);
                }else{
                    // will sending in this have issues?  this refers to a scriptable object...  other examples I was using a mono
                    onInventoryChanged.Raise(null, item_slot);
                }
                return;
            }
        }
        // default back to non-stacking behavior
        int empty_slot_index = FindEmptySlot();
        if(empty_slot_index >= 0){
            // we should add to this slot
            slots[empty_slot_index].item = new_item;
            slots[empty_slot_index].stack_size = 1;
            ItemSlot item_slot = ScriptableObject.CreateInstance("ItemSlot") as ItemSlot;
            item_slot.index = empty_slot_index;
            item_slot.item = new_item;
            item_slot.stack_size = slots[empty_slot_index].stack_size;
            if(isStorage){
                onStorageChanged.Raise(null, item_slot);
            }else{    
                // this data needs passed to the UI, so it can update
                onInventoryChanged.Raise(null, item_slot);
            }
            return;
        }
        return;
    }

    public void RemoveItem(int index, bool shouldDrop=false){
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

    public void DropItem(int index){
        RemoveItem(index, true);
    }

    // remove all items from inventory
    public virtual void Clear(){
        for(int i=0;i<max_slots;i++){
            slots[i].item = null;
            slots[i].stack_size = 0;
            ItemSlot item_slot = ScriptableObject.CreateInstance("ItemSlot") as ItemSlot;
            item_slot = slots[i];
            onInventoryChanged.Raise(null, item_slot);
        }
    }

    int FindEmptySlot(){
        for(int slot_index=0;slot_index<slots.Length;slot_index++){
            if(slots[slot_index]!=null){
                if(slots[slot_index].item==null) return slot_index;
            }
        }
        return -1;
    }

    int FindItemInSlot(Item find_item){
        for(int slot_index=0;slot_index<slots.Length;slot_index++){
            if(slots[slot_index]!=null){
                if(slots[slot_index].item!=null){
                    Debug.Log("Find item id: " + find_item.id.ToString());
                    Debug.Log("slot item id: " + slots[slot_index].item.id.ToString());
                    if(slots[slot_index].item.id==find_item.id && slots[slot_index].stack_size < find_item.max_stack_size) return slot_index;
                }
            }
        }
        return -1;
    }

    public int CheckInventoryForItem(Item find_item){
        for(int slot_index=0;slot_index<slots.Length;slot_index++){
            if(slots[slot_index].item.id==find_item.id) return slot_index;
        }
        return -1;
    }

    public virtual void ListInventory(){
        foreach(ItemSlot slot in slots){
            if(slot.item!=null){
                Debug.Log("Slot item: " + slot.item.name);
            }
        }
    }
}
