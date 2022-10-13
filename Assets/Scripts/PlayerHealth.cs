using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : Health{
    private GameObject canvas;

    [Header("Events")]
    public GameEvent onPlayerHealthChanged;

    public override void Start(){
        base.Start();
        canvas = GameObject.Find("HUD");
        onPlayerHealthChanged.Raise(this, currentHealth);
    }
    public override void DealDamage(int damage){
        base.DealDamage(damage);
        onPlayerHealthChanged.Raise(this, currentHealth);
    }
    public override void Death(bool shouldDestroy=true){
        base.Death(false);
        gameObject.GetComponent<PlayerLook>().RevokeLook();
        gameObject.GetComponent<PlayerMovement>().RevokeMovement();
        canvas.transform.GetChild(2).gameObject.SetActive(true);
        Cursor.visible = true;
    }

	public override void ResetHealth(){
		base.ResetHealth();
        onPlayerHealthChanged.Raise(this, currentHealth);
	}
}
