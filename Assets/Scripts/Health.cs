using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	public int maxHealth = 100;
	public int currentHealth = 100;
    private int layer;
	private Manager manager;

    public virtual void Start(){
		manager = GameObject.Find("Manager").GetComponent<Manager>();
        layer = gameObject.layer;    
    }

	public virtual void DealDamage(int damage){
		currentHealth -= damage;
		checkifDead();
	}

	void checkifDead(){
		if (currentHealth <= 0){
			currentHealth = 0;
			Death();
		}
	}

	public virtual void Death(bool shouldDestroy=true){
        HandleScore();
		if(shouldDestroy) Destroy(gameObject);
	}

    void HandleScore(){
        if(layer==9){
			manager.game_rules.EnemyKilled();
        }
    }
}
