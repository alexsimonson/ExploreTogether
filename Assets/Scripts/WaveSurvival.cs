using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSurvival : GameMode, IGameMode {

    int current_round = 0;
    int score = 0;
    static int enemies_spawned_this_round_max_default = 5;
    int enemies_spawned_this_round_max = enemies_spawned_this_round_max_default;  // how many enemies will spawn and be defeated before the round ends
    int enemies_spawned_max = 10;
    int enemies_currently_spawned = 0;
    int enemies_spawned_this_round = 0;
    int enemies_eliminated_this_round = 0;
    int spawners_this_round = 1;
    int spawners_eliminated_this_round = 0;

    // enemy spawners
    public GameObject[] enemy_spawners;


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
        enemy_prefab = Resources.Load("Prefabs/Enemy", typeof(GameObject)) as GameObject;
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        sword_test = Resources.Load("Items/Sword", typeof(Melee)) as Melee;
        gun_test = Resources.Load("Items/Pistol", typeof(Gun)) as Gun;
        dungeon_pass = Resources.Load("Items/Dungeon Pass", typeof(Item)) as Item;
    }

    public void Start(){
        mode = Mode.WaveSurvival;
        maze_generator_prefab = Resources.Load("Prefabs/Arena", typeof(GameObject)) as GameObject;
        enemy_spawners = GameObject.FindGameObjectsWithTag("Respawn");
    }

    // determine when the wave will end based on some data
    void DetectWaveEnd(){
        if(spawners_eliminated_this_round!=spawners_this_round){
            return; // game mode is not over
        }
        if(enemies_currently_spawned>0){
            return; // game mode is not over
        }
        ProgressGameMode();
    }

    public void SpawnerKilledListener(Component sender, object data){
        spawners_eliminated_this_round += 1;
        Debug.Log("spawners_eliminated_this_round: " + spawners_eliminated_this_round);
        DetectWaveEnd();
    }

    public void EnemyKilledListener(Component sender, object data){
        EnemyKilled();
        if(manager.game_rules.ShouldSpawnEnemy()){
            // pick a random spawn point and then call spawn enemy
            enemy_spawners = GameObject.FindGameObjectsWithTag("Respawn");
            int rnd_index = Random.Range(0, enemy_spawners.Length);
            enemy_spawners[rnd_index].GetComponent<RespawnPoint>().SpawnEnemy();
        }
        DetectWaveEnd();
    }

    public override void SpawnMap(){
        // we should just implement the basic wave survival scenario
        InstantiateMap();
        manager.MapSetupCallback();
    }

    public override void Initialize(){
        manager.player_inventory = ScriptableObject.CreateInstance("Inventory") as Inventory;
        // there's something about the loading that's preventing items from being loaded at this exact point in time...
        // manager.player_inventory.AddItem(sword_test);
        // manager.player_inventory.AddItem(gun_test);
        // manager.player_inventory.AddItem(dungeon_pass);
        EndRound("Initialize");
    }

    // there's a null reference here that should go away soon, it is not the game mode's responsibility for spawning enemies
    // though it should keep track of necessary data that will be used to potentially control the spawns
    public override void SpawnEnemies(){
        // Debug.Log("Running the spawn enemies script");
        // while(enemies_currently_spawned < enemies_spawned_max && enemies_spawned_this_round < enemies_spawned_this_round_max){
        //     manager.maze.GetComponent<Maze>().SpawnEnemy();
        //     enemies_currently_spawned += 1;
        //     enemies_spawned_this_round += 1;
        // }
        // if(enemies_currently_spawned==enemies_spawned_max){
        //     // Debug.Log("Awaiting enemy death before spawning more");
        // }
        // if(enemies_spawned_this_round==enemies_spawned_this_round_max){
        //     // Debug.Log("No more enemies will be spawned this round.");
        // }
        // if(enemies_eliminated_this_round==enemies_spawned_this_round_max){
        //     EndRound();
        // }
    }

    public override bool ShouldSpawnEnemy(){
        // if(enemies_currently_spawned < enemies_spawned_max && enemies_spawned_this_round < enemies_spawned_this_round_max){
        // right now this spawner will constantly spawn enemies unless there are too many spawned at once
        if(spawners_eliminated_this_round == spawners_this_round){
            Debug.Log("There are no more spawners available");
            return false;
        }
        if(enemies_currently_spawned < enemies_spawned_max){
            // increase data, then return true to spawn the enemy
            enemies_currently_spawned += 1;
            enemies_spawned_this_round += 1;
            Debug.Log("enemies_currently_spawned: " + enemies_currently_spawned.ToString());
            Debug.Log("enemies_spawned_this_round: " + enemies_spawned_this_round.ToString());
            return true;
        }
        return false;
    }

    // this function will be ran after all enemies are defeated
    void EndRound(string test = "test"){
        Debug.Log("End round caLLED with " + test);
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
        spawners_eliminated_this_round = 0;
    }

    public override IEnumerator BeginTransitionPeriod(){
        transition_period = true;
        Debug.Log("Transition period started");
        yield return new WaitForSeconds(1);
        Debug.Log("Transition period ending.");
        transition_period = false;
        // band aid
        if(current_round==1){
            manager.player_inventory.AddItem(sword_test);
            manager.player_inventory.AddItem(gun_test);
            manager.player_inventory.AddItem(dungeon_pass);
        }
    }

    public override void EnemyKilled(){
        enemies_currently_spawned -= 1;
        enemies_eliminated_this_round += 1;
        Debug.Log("enemies_eliminated_this_round: " + enemies_eliminated_this_round.ToString());
    }

    public override void ResetGameMode(){
        // we should delete every single game object that isn't essential
        manager.DestroyNonEssentialGameObjects();
        manager.Setup();
        current_round = 0;
        enemies_spawned_this_round_max = enemies_spawned_this_round_max_default;
        manager.player.transform.position = manager.playerSpawnPoint;
        EndRound("Reset GameMode");
    }

    public override void ProgressGameMode(){
        // pop the transition panel
        manager.DestroyNonEssentialGameObjects();
        InstantiateMap();
        EndRound("ProgressGameMode");
    }

    public void InstantiateMap(){
        // we should just implement the basic wave survival scenario
        manager.maze = Instantiate(maze_generator_prefab);
        // manager.MapSetupCallback();
    }

    public override bool TransitionPeriod(){
        return transition_period;
    }    
}