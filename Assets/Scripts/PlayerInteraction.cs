using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ExploreTogether {
    public class PlayerInteraction : MonoBehaviour {

        private Inventory inventory;
        public GameObject interactWith;
        private GameObject interactionText;
        public List<Item> itemInteraction;  // temporary to avoid functionality loss
        Manager manager;
        public GameObject camera;

        void Start(){
            // maybe this should be inside of the manager?
            manager = GameObject.Find("Manager").GetComponent<Manager>();
            interactionText = manager.hud.transform.GetChild(4).gameObject;
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
                    ChangeInteractionWithText();
                }
            }
        }

        private void InteractRaycast() {
            RaycastHit hit;
            if (camera != null) {
                if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 2)) {
                    Transform giveInteractionTo = IsInLayerRecursively(hit.transform, 7);
                    if (giveInteractionTo!=null) {
                        GiveInteractWith(giveInteractionTo.gameObject);
                        return;
                    }
                }
            }
            ClearInteractWith();
        }

        private Transform IsInLayerRecursively(Transform transform, int layer) {
            if (transform.gameObject.layer == layer) {
                return transform;
            }

            foreach (Transform child in transform) {
                if (IsInLayerRecursively(child, layer)) {
                    return child;
                }
            }

            return null;
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
            if(interactionText!=null){
                if(interactWith!=null && !interactionText.activeSelf){
                    ChangeInteractionWithText();
                    interactionText.SetActive(true);
                }else if(interactWith==null && interactionText.activeSelf){
                    interactionText.SetActive(false);
                }
            }
        }

        public void ChangeInteractionWithText(){
            if(interactWith!=null && interactWith.GetComponent<IInteraction>()!=null){
                string interacting_with_text = interactWith.GetComponent<IInteraction>().InteractionName();
                interactionText.GetComponent<Text>().text = "Press F to Interact with " + interacting_with_text;
            }
        }
    }
}
