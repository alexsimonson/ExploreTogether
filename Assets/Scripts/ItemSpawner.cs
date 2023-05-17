using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour, IInteraction {

    public Item item_offered;

    Manager manager;

    bool random_box = true;

    [Header("Events")]
    public GameEvent onScoreSpent;
    int cost = 4000;

    void Start(){
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        item_offered = manager.GenerateItem();
    }

    public void Interaction(GameObject interacting){
        if(random_box==true){
            var score = manager.game_mode.GetUnspentScore();
            if(score<cost){
                Debug.Log("Not enough money");
                return;
            }
            onScoreSpent.Raise(this, cost);
            Debug.Log("charge the man with an event");
        }
        manager.player_inventory.AddItem(item_offered);
        item_offered = manager.GenerateItem();
    }

    public string InteractionName(){
        if(random_box) return "Random Box (" + cost.ToString() + " Score)";
        return item_offered.name;
    }
}
