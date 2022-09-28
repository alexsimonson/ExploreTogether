using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour {

    private Inventory inventory;
    public List<Item> interactWith;
    private GameObject interactionText;

    void Awake(){
        interactionText = GameObject.Find("Canvas").transform.GetChild(4).gameObject;
        interactionText.SetActive(false);
    }
    // Update is called once per frame
    void Update(){
        DisplayInteractionUI();
        PlayerInput();
    }

    void PlayerInput(){
        if(Input.GetKeyDown("f")){
            Debug.Log("Interaction");
            if(interactWith[0]!=null){
                Debug.Log("could interact with this item");
                interactWith[0].Interaction();
            }
        }
    }

    public void CanInteractWith(Item interactable){
        interactWith.Add(interactable);
    }

    public void CanNotInteractWith(Item interactable){
        interactWith.Remove(interactable);
    }

    private void DisplayInteractionUI(){
        if(interactWith.Count > 0 && !interactionText.activeSelf){
            interactionText.SetActive(true);
        }else if(interactWith.Count <= 0 && interactionText.activeSelf){
            interactionText.SetActive(false);
        }
    }
}
