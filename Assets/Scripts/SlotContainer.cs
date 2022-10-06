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
    private static string[] valid_equipment_types = {"Equipment", "Weapon", "Gun", "Magic", "Melee"};
    private List<string> valid_equipment_types_list = new List<string>(valid_equipment_types);

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

        if(eventData.pointerDrag==null){
            return;
        }

        bool draggingEquipment = false;
        if(eventData.pointerDrag.GetComponent<EquipmentSlot>()!=null){
            draggingEquipment = true;
        }
        
        if(draggingEquipment){
            if(eventData.pointerDrag.GetComponent<EquipmentSlot>().item==null){
                return;
            }
        }else{
            if(eventData.pointerDrag.GetComponent<InventorySlot>().item==null){
                return;
            }
        }

        if(isEquipmentSlot){
            if(draggingEquipment){
                // SwapEquipment(eventData);
            }else{
                EquipItem(eventData);
            }
        }else{
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
        string dragSlotType = dragSlot.GetComponent<InventorySlot>().item.GetType().ToString();
        if(valid_equipment_types_list.Contains(dragSlotType)){
            // before we just go through with it, we need to ensure this item is of the correct type
            // we will need the type we are looking for, from this dropped on slot
            Equipment testThis = (Equipment)dragSlot.GetComponent<InventorySlot>().item;
            if(dragSlotType=="Weapon"){
                testThis = (Weapon)dragSlot.GetComponent<InventorySlot>().item;
            }
            string dragSlotItemType = testThis.type.ToString();
            string dropSlotItemType = inventorySlot.GetComponent<EquipmentSlot>().type.ToString();
            if(dropSlotItemType==dragSlotItemType){
                // this means that the equipment slot types match and we should go through with equipping the item
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
                    inventoryUI.UpdateSlot(drag_index, null, dragSlot);
                }else{
                    inventoryUI.UpdateSlot(drag_index, (Item)originalItem, dragSlot);
                }
            }else{
                return;
            }
        }else{
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
        string dragSlotType = dragSlot.GetComponent<EquipmentSlot>().item.GetType().ToString();
        if(valid_equipment_types_list.Contains(dragSlotType)){
            // need to check if the originalItem is of the Equipment Type or not
            if(originalItem!=null && !valid_equipment_types_list.Contains(originalItem.GetType().ToString())){            
                return;
            }

            // before we go further, we need to ensure that the item potentially being swapped is of the correct type
            // originalItem is not null, then we should check the type
            if(!shouldBeEmpty){
                // this means that we must ensure the originalItem is of the correct Equipment type
                Equipment castOriginalItemEquipment = originalItem as Equipment;
                if(castOriginalItemEquipment!=null){
                    if(dragSlotType=="Weapon"){
                        Weapon castOriginalItemWeapon = originalItem as Weapon;
                        if(castOriginalItemWeapon!=null){
                        }else{
                        }
                    }else{
                        if(castOriginalItemEquipment.type==dragSlot.GetComponent<EquipmentSlot>().type){
                        }else{
                            return;
                        }
                    }
                }
            }
            inventorySlot.GetComponent<InventorySlot>().item = (Item)dragSlot.GetComponent<EquipmentSlot>().item;
            inventorySlot.GetComponent<InventorySlot>().stack_size = dragSlot.GetComponent<EquipmentSlot>().stack_size;
            inventorySlot.transform.SetParent(gameObject.transform, false);
            inventorySlot.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
            
            dragSlot.GetComponent<EquipmentSlot>().item = shouldBeEmpty ? null : (Equipment)originalItem;
            dragSlot.GetComponent<EquipmentSlot>().stack_size = shouldBeEmpty ? 0 : originalStackSize;
            dragSlot.transform.SetParent(pointerDragParent, false);
            dragSlot.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
            

            inventoryUI.UpdateSlot(index, inventorySlot.GetComponent<InventorySlot>().item, inventorySlot);
            if(shouldBeEmpty){
                gearUI.UpdateSlot(drag_index, null, dragSlot);
            }else{
                gearUI.UpdateSlot(drag_index, (Equipment)originalItem, dragSlot);
            }
        }else{
            return;
        }
    }

    // this function may simply never be necessary with type requirements...
    void SwapEquipment(PointerEventData eventData){
        // at this point in time, inventorySlot represents the slot where our mouse let go of the button
        // this logic is necessary for preventing a bug at this time
        Equipment originalItem = inventorySlot.GetComponent<EquipmentSlot>().item;   // I need this value when the bool is false
        int originalStackSize = inventorySlot.GetComponent<EquipmentSlot>().stack_size;   // I need this value when the bool is false
        bool shouldBeEmpty = originalItem==null ? true : false; 

        Transform pointerDragParent = eventData.pointerDrag.transform.parent;
        GameObject pointerDragSlotContainer = pointerDragParent.gameObject;
        GameObject dragSlot = pointerDragParent.GetComponent<SlotContainer>().inventorySlot;
        int drag_index = pointerDragParent.GetComponent<SlotContainer>().index;
        GameObject tempSlot = inventorySlot;

        inventorySlot.GetComponent<EquipmentSlot>().item = dragSlot.GetComponent<EquipmentSlot>().item;
        inventorySlot.GetComponent<EquipmentSlot>().stack_size = dragSlot.GetComponent<EquipmentSlot>().stack_size;
        inventorySlot.transform.SetParent(gameObject.transform, false);
        inventorySlot.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        
        dragSlot.GetComponent<EquipmentSlot>().item = shouldBeEmpty ? null : originalItem;
        dragSlot.GetComponent<EquipmentSlot>().stack_size = shouldBeEmpty ? 0 : originalStackSize;
        dragSlot.transform.SetParent(pointerDragParent, false);
        dragSlot.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        
        gearUI.UpdateSlot(index, inventorySlot.GetComponent<EquipmentSlot>().item, inventorySlot);
        if(shouldBeEmpty){
            // something with this logic is causing an item slot to be messed up
            gearUI.UpdateSlot(drag_index, null, dragSlot);
        }else{
            gearUI.UpdateSlot(drag_index, originalItem, dragSlot);
        }
    }

    public void DropItem(){
        inventoryUI.EmptySlot(index);
    }
}
