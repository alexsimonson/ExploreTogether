using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotContainer : MonoBehaviour, IDropHandler {

    public GameObject inventorySlot;
    public InventoryUI inventoryUI;
    public GearUI gearUI;
    public int index;
    public bool isEquipmentSlot;

    void Start(){
        inventoryUI = GameObject.Find("Canvas").transform.GetChild(3).gameObject.GetComponent<InventoryUI>();
        gearUI = GameObject.Find("Canvas").transform.GetChild(5).gameObject.GetComponent<GearUI>();
        if(inventorySlot.GetComponent<EquipmentSlot>()!=null){
            // this is an equipment slot
            isEquipmentSlot = true;
        }else{
            isEquipmentSlot = false;
        }
    }

    public void OnDrop(PointerEventData eventData){
        Debug.Log("OnDrop FROM SLOT CONTAINER");

        if(eventData.pointerDrag==null){
            Debug.Log("There is nothing to drag");
            return;
        }

        bool draggingEquipment = false;
        if(eventData.pointerDrag.GetComponent<EquipmentSlot>()!=null){
            draggingEquipment = true;
        }
        
        if(draggingEquipment){
            if(eventData.pointerDrag.GetComponent<EquipmentSlot>().item==null){
                Debug.Log("There is no item in the equipment slot");
                return;
            }
        }else{
            if(eventData.pointerDrag.GetComponent<InventorySlot>().item==null){
                Debug.Log("There is no item in the inventory slot");
                return;
            }
        }

        if(isEquipmentSlot){
            Debug.Log("Run this code when I drop inside an equipment slot");
            EquipItem(eventData);
        }else{
            Debug.Log("Run this code because I dropped inside an item slot");
            if(draggingEquipment){
                UnEquipItem(eventData);
            }else{
                SwapItem(eventData);
            }
        }

    }

    void SwapItem(PointerEventData eventData){
        // at this point in time, inventorySlot represents the slot where our mouse let go of the button
        // this logic is necessary for preventing a bug at this time
        Item originalItem = inventorySlot.GetComponent<InventorySlot>().item;   // I need this value when the bool is false
        int originalStackSize = inventorySlot.GetComponent<InventorySlot>().stack_size;   // I need this value when the bool is false
        bool shouldBeEmpty = originalItem==null ? true : false; 

        Transform pointerDragParent = eventData.pointerDrag.transform.parent;
        GameObject pointerDragSlotContainer = pointerDragParent.gameObject;
        GameObject dragSlot = pointerDragParent.GetComponent<SlotContainer>().inventorySlot;
        int drag_index = pointerDragParent.GetComponent<SlotContainer>().index;
        GameObject tempSlot = inventorySlot;

        inventorySlot.GetComponent<InventorySlot>().item = dragSlot.GetComponent<InventorySlot>().item;
        inventorySlot.GetComponent<InventorySlot>().stack_size = dragSlot.GetComponent<InventorySlot>().stack_size;
        inventorySlot.transform.SetParent(gameObject.transform, false);
        inventorySlot.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        
        dragSlot.GetComponent<InventorySlot>().item = shouldBeEmpty ? null : originalItem;
        dragSlot.GetComponent<InventorySlot>().stack_size = shouldBeEmpty ? 0 : originalStackSize;
        dragSlot.transform.SetParent(pointerDragParent, false);
        dragSlot.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        
        inventoryUI.UpdateSlot(index, inventorySlot.GetComponent<InventorySlot>().item, inventorySlot);
        if(shouldBeEmpty){
            inventoryUI.UpdateSlot(drag_index, null, dragSlot);
        }else{
            inventoryUI.UpdateSlot(drag_index, originalItem, dragSlot);
        }
    }

    void EquipItem(PointerEventData eventData){
        // same code to start as SwapItem, but I will be adjusting things as necessary
        // at this point in time, inventorySlot represents the slot where our mouse let go of the button
        // this logic is necessary for preventing a bug at this time
        Item originalItem = inventorySlot.GetComponent<EquipmentSlot>().item;   // I need this value when the bool is false
        int originalStackSize = inventorySlot.GetComponent<EquipmentSlot>().stack_size;   // I need this value when the bool is false
        bool shouldBeEmpty = originalItem==null ? true : false; 

        Transform pointerDragParent = eventData.pointerDrag.transform.parent;
        GameObject pointerDragSlotContainer = pointerDragParent.gameObject;
        GameObject dragSlot = pointerDragParent.GetComponent<SlotContainer>().inventorySlot;
        int drag_index = pointerDragParent.GetComponent<SlotContainer>().index;
        GameObject tempSlot = inventorySlot;
        Debug.Log("Item type in question: " + dragSlot.GetComponent<InventorySlot>().item.GetType().ToString());
        if(dragSlot.GetComponent<InventorySlot>().item.GetType().ToString()=="Equipment"){
            inventorySlot.GetComponent<EquipmentSlot>().item = (Equipment)dragSlot.GetComponent<InventorySlot>().item;
            inventorySlot.GetComponent<EquipmentSlot>().stack_size = dragSlot.GetComponent<InventorySlot>().stack_size;
            inventorySlot.transform.SetParent(gameObject.transform, false);
            inventorySlot.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
            
            dragSlot.GetComponent<InventorySlot>().item = shouldBeEmpty ? null : originalItem;
            dragSlot.GetComponent<InventorySlot>().stack_size = shouldBeEmpty ? 0 : originalStackSize;
            dragSlot.transform.SetParent(pointerDragParent, false);
            dragSlot.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
            
            gearUI.UpdateSlot(index, inventorySlot.GetComponent<EquipmentSlot>().item, inventorySlot);
            if(shouldBeEmpty){
                Debug.Log("Inventory slot should be empty");
                inventoryUI.UpdateSlot(drag_index, null, dragSlot);
            }else{
                Debug.Log("Replacing inventory slot with original equipped item");
                inventoryUI.UpdateSlot(drag_index, (Item)originalItem, dragSlot);
            }
        }else{
            Debug.Log("Failed to equip");
            return;
        }
    }

    void UnEquipItem(PointerEventData eventData){
        // same code to start as SwapItem, but I will be adjusting things as necessary
        // at this point in time, inventorySlot represents the slot where our mouse let go of the button
        // this logic is necessary for preventing a bug at this time
        Item originalItem = inventorySlot.GetComponent<InventorySlot>().item;   // I need this value when the bool is false
        int originalStackSize = inventorySlot.GetComponent<InventorySlot>().stack_size;   // I need this value when the bool is false
        bool shouldBeEmpty = originalItem==null ? true : false; 

        Transform pointerDragParent = eventData.pointerDrag.transform.parent;
        GameObject pointerDragSlotContainer = pointerDragParent.gameObject;
        GameObject dragSlot = pointerDragParent.GetComponent<SlotContainer>().inventorySlot;
        int drag_index = pointerDragParent.GetComponent<SlotContainer>().index;
        GameObject tempSlot = inventorySlot;
        Debug.Log("Item type in question: " + dragSlot.GetComponent<EquipmentSlot>().item.GetType().ToString());
        if(dragSlot.GetComponent<EquipmentSlot>().item.GetType().ToString()=="Equipment"){
            inventorySlot.GetComponent<InventorySlot>().item = (Item)dragSlot.GetComponent<EquipmentSlot>().item;
            inventorySlot.GetComponent<InventorySlot>().stack_size = dragSlot.GetComponent<EquipmentSlot>().stack_size;
            inventorySlot.transform.SetParent(gameObject.transform, false);
            inventorySlot.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
            
            dragSlot.GetComponent<EquipmentSlot>().item = shouldBeEmpty ? null : (Equipment)originalItem;
            dragSlot.GetComponent<EquipmentSlot>().stack_size = shouldBeEmpty ? 0 : originalStackSize;
            dragSlot.transform.SetParent(pointerDragParent, false);
            dragSlot.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
            
            gearUI.UpdateSlot(index, inventorySlot.GetComponent<InventorySlot>().item, inventorySlot);
            if(shouldBeEmpty){
                Debug.Log("Inventory slot should be empty");
                inventoryUI.UpdateSlot(drag_index, null, dragSlot);
            }else{
                Debug.Log("Replacing inventory slot with original equipped item");
                inventoryUI.UpdateSlot(drag_index, (Equipment)originalItem, dragSlot);
            }
        }else{
            Debug.Log("Failed to equip");
            return;
        }
    }
}
