using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonCrawler : GameMode, IGameMode {

    int current_round = 0;
    int score = 0;
    static int enemies_spawned_this_round_max_default = 5;
    int enemies_spawned_this_round_max = enemies_spawned_this_round_max_default;  // how many enemies will spawn and be defeated before the round ends
    int enemies_spawned_max = 10;
    int enemies_currently_spawned = 0;
    int enemies_spawned_this_round = 0;
    int enemies_eliminated_this_round = 0;
    public bool transition_period = false;
    public GameObject enemy_prefab;

    public Manager manager;

    public GameObject[] playerInventoryBackup;
    public GameObject[] playerGearBackup;

    public Melee sword_test;
    public Gun gun_test;
    public Item dungeon_pass;

    // game mode overhaul
    public GameObject maze_generator_prefab;

    public void Awake(){
        manager = GameObject.Find("Manager").GetComponent<Manager>();
    }

    public void Start(){
        mode = Mode.DungeonCrawler;
        maze_generator_prefab = Resources.Load("Prefabs/MazeGenerator", typeof(GameObject)) as GameObject;
        sword_test = Resources.Load("Items/Sword", typeof(Melee)) as Melee;
        gun_test = Resources.Load("Items/Pistol", typeof(Gun)) as Gun;
        dungeon_pass = Resources.Load("Items/Dungeon Pass", typeof(Item)) as Item;
    }

    public override void SpawnMap(){
        manager.maze = Instantiate(maze_generator_prefab);
        manager.maze.GetComponent<Maze>().manager = gameObject.GetComponent<Manager>();
    }

    // Start is called before the first frame update
    public override void Initialize(){
        manager.player_inventory = ScriptableObject.CreateInstance("Inventory") as Inventory;
        // manager.player_inventory.AddItem(sword_test);
        manager.player_inventory.AddItem(gun_test);
        // manager.player_inventory.AddItem(dungeon_pass);
        EndRound();
    }

    public override void SpawnEnemies(){
        while(enemies_currently_spawned < enemies_spawned_max && enemies_spawned_this_round < enemies_spawned_this_round_max){
            manager.maze.GetComponent<Maze>().SpawnEnemy();
            enemies_currently_spawned += 1;
            enemies_spawned_this_round += 1;
        }
        if(enemies_currently_spawned==enemies_spawned_max){
            // Debug.Log("Awaiting enemy death before spawning more");
        }
        if(enemies_spawned_this_round==enemies_spawned_this_round_max){
            // Debug.Log("No more enemies will be spawned this round.");
        }
        if(enemies_eliminated_this_round==enemies_spawned_this_round_max){
            EndRound();
        }
    }

    public override bool ShouldSpawnEnemy(){
        if(enemies_currently_spawned < enemies_spawned_max && enemies_spawned_this_round < enemies_spawned_this_round_max){
            // increase data, then return true to spawn the enemy
            enemies_currently_spawned += 1;
            enemies_spawned_this_round += 1;
            return true;
        }
        return false;
    }

    // this function will be ran after all enemies are defeated
    void EndRound(){
        StartCoroutine(BeginTransitionPeriod());
        SetupNextRound();
    }

    public override void SetupNextRound(){
        current_round += 1;
        manager.hud.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Round " + current_round.ToString();
        IncreaseEnemies();
        RewardRoundBonus();
        ResetRoundDefaults();
    }

    // this algorithm will determine how many enemies are present in each round
    void IncreaseEnemies(){
        enemies_spawned_this_round_max += 1;
    }

    void RewardRoundBonus(){
        score += 1000;
    }

    public override GameObject[] GetPlayerInventoryBackup(){
        return playerInventoryBackup;
    }
    public override GameObject[] GetPlayerGearBackup(){
        return playerGearBackup;
    }

    void ResetRoundDefaults(){
        enemies_eliminated_this_round = 0;
        enemies_currently_spawned = 0;
        enemies_spawned_this_round = 0;
    }

    public override IEnumerator BeginTransitionPeriod(){
        transition_period = true;
        Debug.Log("Transition period started");
        yield return new WaitForSeconds(5);
        Debug.Log("Transition period ending.");
        transition_period = false;
    }

    public override void EnemyKilled(){
        enemies_currently_spawned -= 1;
        enemies_eliminated_this_round += 1;
    }

    public override void ResetGameMode(){
        // we should delete every single game object that isn't essential
        manager.DestroyNonEssentialGameObjects();
        manager.Setup();
        current_round = 0;
        enemies_spawned_this_round_max = enemies_spawned_this_round_max_default;
        manager.player.transform.position = manager.playerSpawnPoint;
        EndRound();
    }

    public override void ProgressGameMode(){
        // pop the transition panel
        // manager.hud.transform.GetChild(8).gameObject.SetActive(true);
        manager.DestroyNonEssentialGameObjects();
        manager.Setup();
        // manager.player.GetComponent<PlayerLook>().AllowLook();
        // manager.player.GetComponent<PlayerMovement>().AllowMovement();
        EndRound();
    }

    public override bool TransitionPeriod(){
        return transition_period;
    }    
}