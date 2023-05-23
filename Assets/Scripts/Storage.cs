using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour, IInteraction {
    
    public new string name;
    Manager manager;
    public Inventory storage_inventory;

    [Header("Events")]
    public GameEvent onStorageAccessed;

    public GameObject storage_hud;

    void Start(){
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        storage_hud = manager.hud.transform.GetChild(11).gameObject;
        storage_inventory = ScriptableObject.CreateInstance("Inventory") as Inventory;
    }

    public void Interaction(GameObject interactingWith){
        // we should display the storage inventory to the player
        if(interactingWith.tag=="Player"){
            PlayerInput playerInput = interactingWith.GetComponent<PlayerInput>();
            bool? temp_state = null;
            // this branch will determine temp_state
            if(playerInput.hud_visible_state==true && manager.GetComponent<Manager>().storage_hud_visible_state==true){
                Debug.Log("Hide hud when both are showing");
                // we want to display the hud here
                temp_state = false;
                ToggleStorageHUD(temp_state);
            }else if(playerInput.hud_visible_state==true){
                Debug.Log("show hud when only player inventory is open");
                // we want to display the hud here
                temp_state = true;
                ToggleStorageHUD(temp_state);
            }else if(playerInput.hud_visible_state== false && manager.GetComponent<Manager>().storage_hud_visible_state==false){
                temp_state = true;
                ToggleStorageHUD(temp_state);       
            }else{
                // always hide hud otherwise
                temp_state = false;
                ToggleStorageHUD(temp_state);
            }

            // temp_state is set
            if(temp_state==true){
            }

            // the HUD should match with playerInput variable here...
            if(temp_state==true){
                // we should set the inventory as we're about to utilize it
                manager.hud.transform.GetChild(11).gameObject.GetComponent<InventoryUI>().SetWatchingInventoryByReference(ref storage_inventory);
                manager.hud.transform.GetChild(11).gameObject.GetComponent<InventoryUI>().DrawInventoryUI(storage_inventory);
                // storage_hud.GetComponent<InventoryUI>().DrawInventoryUI(storage_hud.GetComponent<InventoryUI>().watching_inventory);
            }
            playerInput.ToggleHUD(temp_state);
        }
    }

    // ? after bool allows for null assignment, which is perfect for this function
    public void ToggleStorageHUD(bool? _state=null){
        if(_state==null){
            // toggle
            manager.GetComponent<Manager>().storage_hud_visible_state = !manager.GetComponent<Manager>().storage_hud_visible_state;
        }else{
            // otherwise use value as passed in
            manager.GetComponent<Manager>().storage_hud_visible_state = (bool)_state;
        }
        onStorageAccessed.Raise(this, manager.GetComponent<Manager>().storage_hud_visible_state);
    }

    public string InteractionName(){
        return name;
    }
}
