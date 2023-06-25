using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : Health{
    
    [Header("Events")]
    public GameEvent onPlayerHealthChanged;
    public GameEvent onGameStateChanged;
    public bool is_dead = false;

    public override void Start(){
        base.Start();
        onPlayerHealthChanged.Raise(this, currentHealth);
    }
    public override void DealDamage(int damage, RaycastHit? hit=null){
        if(is_dead) return;
        currentHealth -= damage;
        onPlayerHealthChanged.Raise(this, currentHealth);
    }
    public override void Death(bool shouldDestroy=true){
        base.Death(false);
        is_dead = true;
        onGameStateChanged.Raise(this, Manager.GameState.Dead);
        gameObject.GetComponent<PlayerLook>().RevokeLook();
        gameObject.GetComponent<PlayerMovement>().RevokeMovement();
        Cursor.visible = true;
    }

    public override void HealthAmount(int heal){
		currentHealth += heal;	// we want to cap this at the maxHealth
		if(currentHealth > maxHealth) currentHealth = maxHealth;
        onPlayerHealthChanged.Raise(this, currentHealth);
	}

	public override void ResetHealth(){
		base.ResetHealth();
        onPlayerHealthChanged.Raise(this, currentHealth);
	}
}
