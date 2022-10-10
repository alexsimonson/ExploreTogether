using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionPanel : MonoBehaviour {

    GameObject transition_panel;
    Text transition_panel_text;
    int i=0;
    bool updating = false;
    
    void Start(){
        transition_panel = GameObject.Find("Manager").GetComponent<Manager>().hud.transform.GetChild(8).gameObject;
        transition_panel_text = transition_panel.transform.GetChild(0).gameObject.GetComponent<Text>();
        gameObject.SetActive(false);
    }

    
    void Update(){
        Debug.Log("RUNNING UPDATE TRANSITION PANEL");
        if(!gameObject.activeSelf) return;
        if(updating) return;
        StartCoroutine(DynamicText());
    }

    IEnumerator DynamicText(){
        updating = true;
        transition_panel_text.text = "Loading dungeon";
        string append = "";
        for(int j=0;j<i;j++){
            append += ".";
        }
        transition_panel_text.text += append;
        i++;
        if(i>3){
            i=0;
        }
        yield return new WaitForSeconds(.1f);
        updating = false;
    }
}
