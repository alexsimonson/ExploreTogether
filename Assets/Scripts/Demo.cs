using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demo : GameMode, IGameMode {

    public Melee sword_test;
    public Gun gun_test;
    public Item dungeon_pass;
    public Magic blood_wand;
    public Magic ice_wand;

    public GameObject maze_generator_prefab;

    void Awake(){
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        sword_test = Resources.Load("Items/Sword", typeof(Melee)) as Melee;
        gun_test = Resources.Load("Items/Pistol", typeof(Gun)) as Gun;
        dungeon_pass = Resources.Load("Items/Dungeon Pass", typeof(Item)) as Item;
        blood_wand = Resources.Load("Items/Blood Wand", typeof(Magic)) as Magic;
        ice_wand = Resources.Load("Items/Ice Wand", typeof(Magic)) as Magic;
        maze_generator_prefab = Resources.Load("Prefabs/Map", typeof(GameObject)) as GameObject;
    }

    void Start(){
        mode = Mode.Demo;
    }

    public override bool TransitionPeriod(){
        return transition_period;
    }

    public override void Initialize(){
        Debug.Log("Init demo shit");
        manager.player_inventory.AddItem(sword_test);
        manager.player_inventory.AddItem(gun_test);
        manager.player_inventory.AddItem(blood_wand);
        manager.player_inventory.AddItem(ice_wand);
    }

    public override void SetupNextRound(){
        return;
    }

    public override bool ShouldSpawnEnemy(){
        return false;
    }

    public override void ResetGameMode(){
        return;
    }

    public override GameObject[] GetPlayerInventoryBackup(){
        return playerInventoryBackup;
    }

    public override GameObject[] GetPlayerGearBackup(){
        return playerGearBackup;
    }

    public override void ProgressGameMode(){
        return;
    }

    public override void SpawnMap(){
        manager.map = Instantiate(maze_generator_prefab);
        // I have no idea why I'm setting below... or how it's obtaining this correctly...
        // I'm going to "correct" it and hope for the best
        manager.map.GetComponent<Map>().manager = manager;    // still works... so let's just do this
    }

    public override List<KeyValuePair<string, int>> GetScoreData(){
        return null;
    }

    public override int GetUnspentScore(){
        return score;
    }

}