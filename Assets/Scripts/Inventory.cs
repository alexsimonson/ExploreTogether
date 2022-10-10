using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, IInventory {

    public int max_slots;  // number of items an inventory can hold before full
    public GameObject[] slots;
    public GameObject RelevantScrollView;
    public Manager manager;

    public Melee sword_test;
    public Gun gun_test;
    public Item dungeon_pass;

    void Awake(){
        if(max_slots==0){
            max_slots=28;
        }
        slots = new GameObject[max_slots];
    }

    void Start(){
        Debug.Log("Inventory Start has run...");
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        sword_test = Resources.Load("Items/Sword", typeof(Melee)) as Melee;
        gun_test = Resources.Load("Items/Pistol", typeof(Gun)) as Gun;
        dungeon_pass = Resources.Load("Items/Dungeon Pass", typeof(Item)) as Item;
        ListInventory();
    }

    void Update(){
        PlayerInput();
    }

    public void Initialize(){
        RelevantScrollView = manager.hud.transform.GetChild(3).gameObject;
        RelevantScrollView.SetActive(false);
        AddItem(dungeon_pass);
    }

    public void AddItem(Item new_item){
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

    public void RemoveItem(int index){
        Item removedItem = slots[index].GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().item;
        slots[index].GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().item = null;
        GameObject dropped_item = Instantiate(Resources.Load("Prefabs/Item", typeof(GameObject)) as GameObject, gameObject.transform.position, Quaternion.identity);
        dropped_item.GetComponent<ItemSpawn>().item = removedItem;
        // let's instantiate this item on the ground
    }

    int FindEmptySlot(){
        for(int slot_index=0;slot_index<slots.Length;slot_index++){
            Debug.Log("slots @ index " + slot_index.ToString() + " below");
            Debug.Log(slots[slot_index]);
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

    public int CheckInventoryForItem(Item find_item){
        for(int slot_index=0;slot_index<slots.Length;slot_index++){
            if(slots[slot_index]!=null && slots[slot_index].GetComponent<SlotContainer>().inventorySlot!=null){
                if(slots[slot_index].GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().item!=null){
                    if(slots[slot_index].GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().item.id==find_item.id){
                        Debug.Log("Slot stack size: " + slots[slot_index].GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().stack_size.ToString());
                        Debug.Log(find_item.name + " max stack size: " + find_item.max_stack_size.ToString());
                        return slot_index;
                    }
                }
            }
        }
        return -1;
    }

    void ListInventory(){
        foreach(GameObject slot in slots){
            if(slot!=null && slot.GetComponent<SlotContainer>().inventorySlot!=null && slot.GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().item!=null){
                Debug.Log("Slot item: " + slot.GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().item.name);
            }
        }
    }

    void PlayerInput(){
        if ((Input.GetKeyDown("e"))){
            RelevantScrollView.SetActive(!RelevantScrollView.activeSelf);
            Cursor.visible = RelevantScrollView.activeSelf;
            HandlePlayerRights();
        }
        if((Input.GetKeyDown("g"))){
            RelevantScrollView.GetComponent<InventoryUI>().EmptySlot(0);
        }
    }

    void HandlePlayerRights(){
        if(RelevantScrollView.activeSelf){
            gameObject.GetComponent<PlayerLook>().RevokeLook();
            gameObject.GetComponent<PlayerCombat>().RevokeCombat();
        }else{
            gameObject.GetComponent<PlayerLook>().AllowLook();
            gameObject.GetComponent<PlayerCombat>().AllowCombat();
        }
    }

    public void ResetUI(){
        RelevantScrollView.SetActive(false);
        HandlePlayerRights();
    }

    public void AddStartingItems(){
        AddItem(sword_test);
        AddItem(gun_test);
        AddItem(dungeon_pass);
    }
}
