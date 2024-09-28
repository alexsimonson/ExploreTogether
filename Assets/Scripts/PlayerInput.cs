using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExploreTogether {
    public class PlayerInput : MonoBehaviour{
        
        [Header("Events")]
        public GameEvent onToggleInventory;

        public bool hud_visible_state = false;

        void Update(){
            if ((Input.GetKeyDown("e"))){
                // we should swap everything
                ToggleHUD();
            }

            if(hud_visible_state){
                // UI quality of life controls
                if(Input.GetKey(KeyCode.LeftControl)){
                    // modifier pressed
                    if(Input.GetMouseButtonDown(0)){
                        Debug.Log("Testing drop item or something");
                    }

                }else{
                    // input as normal
                }
            }
        }

        // ? after bool allows for null assignment, which is perfect for this function
        public void ToggleHUD(bool? _state=null){
            if(_state==null){
                // toggle
                hud_visible_state = !hud_visible_state;
            }else{
                // otherwise use value as passed in
                hud_visible_state = (bool)_state;
            }
            onToggleInventory.Raise(this, hud_visible_state);
        }

        public void HandleMouseState(Component sender, object data){
            Cursor.visible = (bool)data;
        }
    }
}
