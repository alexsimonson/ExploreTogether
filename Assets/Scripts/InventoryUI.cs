using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ExploreTogether {
    public class InventoryUI : MonoBehaviour {
        public GameObject hudView;
        public GameObject content;
        public GameObject slotContainerPrefab;
        public GameObject slotButtonPrefab;
        public GameObject[] inventorySlots;    // this is a conversion of the original slots variable from Inventory.cs
        public Manager manager;
        // this will help us decouple the player from this script
        public Inventory watching_inventory;

        public string debug_name;

        void Awake(){
            slotContainerPrefab = Resources.Load("Prefabs/SlotContainer", typeof(GameObject)) as GameObject;
            slotButtonPrefab = Resources.Load("Prefabs/InventorySlotButton", typeof(GameObject)) as GameObject;
            manager = GameObject.Find("Manager").GetComponent<Manager>();
            hudView = gameObject.transform.GetChild(0).gameObject;
            content = hudView.transform.GetChild(0).GetChild(0).gameObject;
        }

        void Start(){
            // if(watching_inventory==null) Debug.LogError("InventoryUI is missing reference to an inventory to display");
            // DrawInventoryUI(watching_inventory);    // this doesn't HAVE to be done here... right?
            Debug.Log("InventoryUI Has been started");
        }

        public void UpdateInventory(Component sender, object data){
            DrawInventoryUI();
        }

        // why is the inventory being passed in, when it should just draw the watching inventory
        public virtual void DrawInventoryUI(){
            // we need to dynamically generate inventory slots based on the existing inventory
            // if slots has not yet been instantiated, we should do that too
            // every time this function runs, we need to remove existing inventorySlots
            foreach(GameObject inventorySlot in inventorySlots){
                Destroy(inventorySlot);
            }

            inventorySlots = new GameObject[watching_inventory.max_slots];
            for(int slot_index=0;slot_index<watching_inventory.max_slots;slot_index++){
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
                inventorySlots[slot_index].GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().SetParentUI(gameObject);
            }
        }

        // this function is run by ALL inventories when one changes... how can I fix this?
        public virtual void UpdateSlot(Component sender, object data){

            if(data.GetType().ToString()!="ExploreTogether.ItemSlot"){
                Debug.LogError("Invalid update data type: " + data.GetType().ToString());
                return;
            }

            ItemSlot slot = (ItemSlot)data;
            // Debug.Log("Item name in update slot: " + slot.item.name);    // this can break functionality...
            
            HandleSlotUpdate(slot);
        }

        public void HandleSlotUpdate(ItemSlot slot)
        {
            Debug.Log("Handle slot update at index: " + slot.index.ToString());

            if (slot.index >= 0 && slot.index < inventorySlots.Length)
            {
                SlotContainer slotContainer = inventorySlots[slot.index].GetComponent<SlotContainer>();

                if (slotContainer != null && slotContainer.inventorySlot != null)
                {
                    Transform slotTransform = slotContainer.inventorySlot.transform;

                    if (slot.item == null)
                    {
                        SetChildText(slotTransform, 0, null);
                        SetChildImage(slotTransform, 1, null);
                        SetChildText(slotTransform, 2, null);
                    }
                    else
                    {
                        SetChildText(slotTransform, 0, slot.item.name);
                        SetChildImage(slotTransform, 1, slot.item.icon);
                        SetChildText(slotTransform, 2, slot.stack_size.ToString());
                    }

                    Color newColor = GetChildImage(slotTransform, 1).color;
                    newColor.a = (slot.item == null) ? 0 : 1;
                    GetChildImage(slotTransform, 1).color = newColor;

                    InventorySlot inventorySlotComponent = slotContainer.inventorySlot.GetComponent<InventorySlot>();

                    if (inventorySlotComponent != null)
                    {
                        inventorySlotComponent.item = slot.item;
                    }

                    if (watching_inventory != null && watching_inventory.slots[slot.index] != null)
                    {
                        watching_inventory.slots[slot.index].item = slot.item;
                    }
                }
            }
        }

        private void SetChildText(Transform parent, int childIndex, string text)
        {
            Text childText = parent.GetChild(childIndex).GetComponent<Text>();
            if (childText != null)
            {
                childText.text = text;
            }
        }

        private void SetChildImage(Transform parent, int childIndex, Sprite sprite)
        {
            Image childImage = parent.GetChild(childIndex).GetComponent<Image>();
            if (childImage != null)
            {
                childImage.sprite = sprite;
            }
        }

        private Image GetChildImage(Transform parent, int childIndex)
        {
            Image childImage = parent.GetChild(childIndex).GetComponent<Image>();
            return childImage;
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
            if((bool)data==false){
                // set manager value back to false
                manager.GetComponent<Manager>().storage_hud_visible_state = false;
            }
        }

        public virtual void HideUI(Component sender, object data){
            if((bool)data==false){
                hudView.SetActive(false);
            }
        }

        public virtual void SetWatchingInventoryByReference(ref Inventory inventory){
            watching_inventory = inventory;
        }

        // this function should very simply loop through all items and re-render them
        public void UpdateInventorySlots(){
            foreach(ItemSlot slot in watching_inventory.slots){
                HandleSlotUpdate(slot);
            }
        }
    }
}
