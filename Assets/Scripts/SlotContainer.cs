using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotContainer : MonoBehaviour, IDropHandler {

    public GameObject inventorySlot;
    private InventoryUI inventoryUI;
    public int index;

    void Start(){
        inventoryUI = GameObject.Find("Canvas").transform.GetChild(3).gameObject.GetComponent<InventoryUI>();
    }

    public void OnDrop(PointerEventData eventData){
        Debug.Log("OnDrop FROM SLOT CONTAINER");

        if(eventData.pointerDrag==null){
            Debug.Log("There is nothing to drag");
            return;
        }
        if(eventData.pointerDrag.GetComponent<InventorySlot>()==null){
            Debug.Log("Dragged object is not an inventory slot");
            return;
        }
        if(eventData.pointerDrag.GetComponent<InventorySlot>().item==null){
            Debug.Log("There is no item in the item slot");
            return;
        }

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
}
