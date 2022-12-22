using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	public int maxHealth = 100;
	public int currentHealth = 100;
    private int layer;
	private Manager manager;

	[Header("Events")]
    public GameEvent onEnemyKilled;
	public GameEvent onSpawnerKilled;
	public GameEvent onPlayerKilled;

    public virtual void Start(){
		manager = GameObject.Find("Manager").GetComponent<Manager>();
        layer = gameObject.layer;    
    }

	public virtual void DealDamage(int damage){
		currentHealth -= damage;
		checkifDead();
	}

	public virtual void Death(bool shouldDestroy=true){
        HandleScore();
		if(shouldDestroy) Destroy(gameObject);
	}

	public virtual void HealthAmount(int heal){
		currentHealth += heal;	// we want to cap this at the maxHealth
		if(currentHealth > maxHealth) currentHealth = maxHealth;
	}

	public virtual void ResetHealth(){
		currentHealth = maxHealth;
	}

	private void checkifDead(){
		if (currentHealth <= 0){
			currentHealth = 0;
			Death();
		}
	}

	// this needs overhauled with an event
    private void HandleScore(){
        if(layer==9){
			// we can handle differently via tags
			if(gameObject.tag=="Respawn"){
				// handle respawn points removed
				onSpawnerKilled.Raise(this, null);
			}else{
				// for now we only have two types
				onEnemyKilled.Raise(this, null);
			}
        }
    }

}
