using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour {
    public GameObject hudView;
    public GameObject content;
    public GameObject slotContainerPrefab;
    public GameObject slotButtonPrefab;
    public GameObject[] inventorySlots;    // this is a conversion of the original slots variable from Inventory.cs
    public Manager manager;

    void Awake(){
        slotContainerPrefab = Resources.Load("Prefabs/SlotContainer", typeof(GameObject)) as GameObject;
        slotButtonPrefab = Resources.Load("Prefabs/InventorySlotButton", typeof(GameObject)) as GameObject;
        manager = GameObject.Find("Manager").GetComponent<Manager>();
    }

    void Start(){
        hudView = gameObject.transform.GetChild(0).gameObject;
        content = hudView.transform.GetChild(0).GetChild(0).gameObject;
        DrawInventoryUI(manager.player_inventory);
        Debug.Log("InventoryUI Has been started");
    }

    public virtual void DrawInventoryUI(Inventory inventory){
        // we need to dynamically generate inventory slots based on the existing inventory
        // if slots has not yet been instantiated, we should do that too
        inventorySlots = new GameObject[inventory.max_slots];
        for(int slot_index=0;slot_index<inventory.slots.Length;slot_index++){
            if(inventorySlots[slot_index]==null){
                GameObject slot_container = Instantiate(slotContainerPrefab, content.transform.position, content.transform.rotation);
                slot_container.transform.SetParent(content.transform, false);
                inventorySlots[slot_index] = slot_container;
                inventorySlots[slot_index].GetComponent<SlotContainer>().index = slot_index;
            }
            // we need to create an inventory slot representation on the gui
            // inventorySlots[slot_index].transform.SetParent(content.transform, false);
            inventorySlots[slot_index].GetComponent<SlotContainer>().inventorySlot = Instantiate(slotButtonPrefab, inventorySlots[slot_index].GetComponent<SlotContainer>().transform.position, inventorySlots[slot_index].GetComponent<SlotContainer>().transform.rotation);
            inventorySlots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.SetParent(inventorySlots[slot_index].transform, false);
            inventorySlots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    public virtual void UpdateSlot(Component sender, object data){
        if(data.GetType().ToString()!="ItemSlot"){
            Debug.LogError("Invalid update data type");
            return;
        }

        ItemSlot slot = (ItemSlot)data;
        
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

        // we also need to change the underlying value of the InventorySlotButton, which honestly is so fucked up because there's too many places this is stored
        inventorySlots[slot.index].GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().item = slot.item;
    }

    // this function may be outdated with events update :)
    public virtual void EmptySlot(int slot_index){
        Color newColor = inventorySlots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().color;
        newColor.a = 0;
        inventorySlots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(0).GetComponent<Text>().text = null;
        inventorySlots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().sprite = null;
        inventorySlots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().color = newColor;
    }

    public virtual void ToggleUI(Component sender, object data){
        hudView.SetActive((bool)data);
    }
}
