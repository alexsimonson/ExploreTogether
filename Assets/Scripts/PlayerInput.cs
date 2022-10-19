using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour{
    
    [Header("Events")]
    public GameEvent onToggleInventory;

    public bool hud_visible_state = false;

    void Update(){
        if ((Input.GetKeyDown("e"))){
            // we should swap everything
            hud_visible_state = !hud_visible_state;
            onToggleInventory.Raise(this, hud_visible_state);
        }
    }

    public void HandleMouseState(Component sender, object data){
        Cursor.visible = (bool)data;
    }
}
