using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : Health{
    private GameObject canvas;
    public override void Start(){
        base.Start();
        canvas = GameObject.Find("Canvas");
        SetHealthUI();
    }
    public override void DealDamage(int damage){
        base.DealDamage(damage);
        SetHealthUI();
    }
    public override void Death(bool shouldDestroy=true){
        base.Death(false);
        gameObject.GetComponent<PlayerLook>().RevokeLook();
        gameObject.GetComponent<PlayerMovement>().RevokeMovement();
        canvas.transform.GetChild(2).gameObject.SetActive(true);
        Cursor.visible = true;
    }
    private void SetHealthUI(){
        canvas.transform.GetChild(1).gameObject.GetComponent<Text>().text = currentHealth.ToString() + " Health";
    }

	public void ResetHealth(){
		currentHealth = maxHealth;
        SetHealthUI();
        gameObject.GetComponent<PlayerLook>().AllowLook();
        gameObject.GetComponent<PlayerMovement>().AllowMovement();
	}
}
