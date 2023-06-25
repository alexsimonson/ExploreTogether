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
    public int storage_slots;
    private Animator animator;

    void Start(){
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        animator = gameObject.GetComponent<Animator>();
        onStorageAccessed = Resources.Load("Events/StorageAccessed", typeof(GameEvent)) as GameEvent;
        storage_inventory = ScriptableObject.CreateInstance("Inventory") as Inventory;
        storage_inventory.max_slots = storage_slots;
        Initialize();
    }

    public void Interaction(GameObject interactingWith){
        if(storage_hud==null){
            storage_hud = manager.hud.transform.GetChild(11).gameObject;
        }
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

            // the HUD should match with playerInput variable here...
            if(temp_state==true){
                // we should set the inventory as we're about to utilize it
                manager.hud.transform.GetChild(11).gameObject.GetComponent<InventoryUI>().SetWatchingInventoryByReference(ref storage_inventory);
                manager.hud.transform.GetChild(11).gameObject.GetComponent<InventoryUI>().DrawInventoryUI();
                onStorageAccessed.Raise(this, manager.GetComponent<Manager>().storage_hud_visible_state);
                // handle open animation
                Debug.Log("Open the trigger");
                animator.SetTrigger("TrOpen");
            }else{
                // handle close animation
                Debug.Log("Close the trigger");
                animator.SetTrigger("TrClose");
            }
            
            playerInput.ToggleHUD(temp_state);
            // handle animation
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
    }

    public string InteractionName(){
        return name;
    }

    public void Initialize(){
        Debug.Log("Initializing storage inventory with some test items");

        // let's spawn a bunch of random items to storage_inventory
        // this will be for number of items
        int minRange = 0;
        int maxRange = storage_inventory.max_slots;
        int randomNumItemsInt = Random.Range(minRange, maxRange + 1);
        for(int i=0;i<randomNumItemsInt;i++){
            var new_item = manager.item_bank[Random.Range(minRange, manager.item_bank.Length)];
            storage_inventory.AddItem(new_item, true);
        }
    }
}
