using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour {

    private Inventory inventory;
    public GameObject interactWith;
    private GameObject interactionText;
    public List<Item> itemInteraction;  // temporary to avoid functionality loss

    void Awake(){
        interactionText = GameObject.Find("Canvas").transform.GetChild(4).gameObject;
        interactionText.SetActive(false);
    }
    // Update is called once per frame
    void Update(){
        InteractRaycast();
        DisplayInteractionUI();
        PlayerInput();
    }

    void PlayerInput(){
        if(Input.GetKeyDown("f")){
            if(interactWith){
                interactWith.GetComponent<IInteraction>().Interaction(gameObject);
            }
        }
    }

    private void InteractRaycast(){
        RaycastHit hit;

        if(Physics.Raycast(gameObject.GetComponent<PlayerCombat>().playerCamera.transform.position, gameObject.GetComponent<PlayerCombat>().playerCamera.transform.forward * 1, out hit, 2) && hit.transform.gameObject.layer==7){
            GiveInteractWith(hit.transform.gameObject);
        }else{
            ClearInteractWith();
        }
    }

    public void CanInteractWith(Item interactable){
        itemInteraction.Add(interactable);
    }

    public void GiveInteractWith(GameObject interactable){
        interactWith = interactable;
    }

    public void ClearInteractWith(){
        interactWith = null;
    }

    public void CanNotInteractWith(Item interactable){
        itemInteraction.Remove(interactable);
    }

    private void DisplayInteractionUI(){
        if(interactWith!=null && !interactionText.activeSelf){
            ChangeInteractionWithText();
            interactionText.SetActive(true);
        }else if(interactWith==null && interactionText.activeSelf){
            interactionText.SetActive(false);
        }
    }

    public void ChangeInteractionWithText(){
        Interaction.Type interactWithType = interactWith.GetComponent<Interaction>().interaction_type;
        string interacting_with_text = null;
        if(interactWithType == Interaction.Type.Item){
            interacting_with_text = interactWith.GetComponent<ItemSpawn>().item.name;
        }else if(interactWithType == Interaction.Type.Resource){
            interacting_with_text = interactWith.GetComponent<Resource>().name;
        }else if(interactWithType == Interaction.Type.Spawner){
            interacting_with_text = "Item Spawner";
        }
        interactionText.GetComponent<Text>().text = "Press F to Interact with " + interacting_with_text;
    }
}
