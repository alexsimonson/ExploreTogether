using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : Health{
    
    [Header("Events")]
    public GameEvent onPlayerHealthChanged;
    public GameEvent onGameStateChanged;

    public override void Start(){
        base.Start();
        onPlayerHealthChanged.Raise(this, currentHealth);
    }
    public override void DealDamage(int damage){
        base.DealDamage(damage);
        onPlayerHealthChanged.Raise(this, currentHealth);
    }
    public override void Death(bool shouldDestroy=true){
        base.Death(false);
        onGameStateChanged.Raise(this, Manager.GameState.Dead);
        gameObject.GetComponent<PlayerLook>().RevokeLook();
        gameObject.GetComponent<PlayerMovement>().RevokeMovement();
        Cursor.visible = true;
    }

	public override void ResetHealth(){
		base.ResetHealth();
        onPlayerHealthChanged.Raise(this, currentHealth);
	}
}
