using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour {
    public Inventory inventory;
    public GameObject content;
    public GameObject slotContainerPrefab;
    public GameObject slotButtonPrefab;
    public bool isPlayerInventory;

    public List<GameObject> inventorySlots = new List<GameObject>();
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
        for(var slot_index=0;slot_index<inventory.slots.Length;slot_index++){
            if(inventory.slots[slot_index]==null){
                inventory.slots[slot_index] = Instantiate(slotContainerPrefab, content.transform.position, content.transform.rotation);
                inventory.slots[slot_index].GetComponent<SlotContainer>().index = slot_index;
            }
            // we need to create an inventory slot representation on the gui
            inventory.slots[slot_index].transform.SetParent(content.transform, false);
            inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot = Instantiate(slotButtonPrefab, inventory.slots[slot_index].GetComponent<SlotContainer>().transform.position, inventory.slots[slot_index].GetComponent<SlotContainer>().transform.rotation);
            inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.SetParent(inventory.slots[slot_index].transform, false);
            inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    public void UpdateSlot(int slot_index, Item new_item, GameObject test=null){
        Color newColor = inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().color;
        if(test==null){
            inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(0).GetComponent<Text>().text = new_item.name;
            inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().sprite = new_item.icon;
            newColor.a = 1;
            inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().color = newColor;
            inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(2).GetComponent<Text>().text = inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().stack_size.ToString();
        }else{
            if(new_item==null){
                test.transform.GetChild(0).GetComponent<Text>().text = null;
                test.transform.GetChild(1).GetComponent<Image>().sprite = null;
                newColor.a = 0;
                inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().color = newColor;
                inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(2).GetComponent<Text>().text = null;
            }else{
                Debug.Log("Test name: " + test.GetComponent<InventorySlot>().item.name);
                test.transform.GetChild(0).GetComponent<Text>().text = new_item.name;
                test.transform.GetChild(1).GetComponent<Image>().sprite = new_item.icon;
                newColor.a = 1;
                inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().color = newColor;
                inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(2).GetComponent<Text>().text = inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().stack_size.ToString();
            }
        }
    }

    public void EmptySlot(int slot_index){
        Color newColor = inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().color;
        newColor.a = 0;
        inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(0).GetComponent<Text>().text = null;
        inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().sprite = null;
        inventory.slots[slot_index].GetComponent<SlotContainer>().inventorySlot.transform.GetChild(1).GetComponent<Image>().color = newColor;
        inventory.RemoveItem(slot_index);
    }
}
