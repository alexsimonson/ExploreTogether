using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonCrawler : GameMode, IGameMode {
    bool killable_spawners = false;
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
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        maze_generator_prefab = Resources.Load("Prefabs/MazeGenerator", typeof(GameObject)) as GameObject;
        sword_test = Resources.Load("Items/Sword", typeof(Melee)) as Melee;
        gun_test = Resources.Load("Items/Pistol", typeof(Gun)) as Gun;
        dungeon_pass = Resources.Load("Items/Dungeon Pass", typeof(Item)) as Item;
    }

    public void Start(){
        mode = Mode.DungeonCrawler;
    }

    // I don't think this function is used within dungeon crawler
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
        // DetectWaveEnd();
    }

    public void EnemyKilledListener(Component sender, object data){
        enemies_currently_spawned -= 1;
        enemies_eliminated_this_round += 1;
        Debug.Log("enemies_eliminated_this_round: " + enemies_eliminated_this_round.ToString());
        if(manager.game_mode.ShouldSpawnEnemy()){
            // pick a random spawn point and then call spawn enemy
            if(killable_spawners){
                enemy_spawners = GameObject.FindGameObjectsWithTag("Respawn");
            }
            // we could add code to remove the closest spawner to the player from the array and then choose randomly of those
            int rnd_index = Random.Range(0, enemy_spawners.Length);
            enemy_spawners[rnd_index].GetComponent<RespawnPoint>().SpawnEnemy();
        }
        // DetectWaveEnd();
    }

    public override void SpawnMap(){
        manager.maze = Instantiate(maze_generator_prefab);
        // I have no idea why I'm setting below... or how it's obtaining this correctly...
        // I'm going to "correct" it and hope for the best
        manager.maze.GetComponent<Maze>().manager = manager;    // still works... so let's just do this
    }

    // Start is called before the first frame update
    public override void Initialize(){
        manager.player_inventory = ScriptableObject.CreateInstance("Inventory") as Inventory;
        // manager.player_inventory.AddItem(sword_test);
        manager.player_inventory.AddItem(gun_test);
        // manager.player_inventory.AddItem(dungeon_pass);
        EndRound();
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
        manager.BeginTransition_SO();
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

    public override List<KeyValuePair<string, int>> GetScoreData(){
        Debug.Log("Get score data");
        List<KeyValuePair<string, int>> listi = new List<KeyValuePair<string, int>>();
        return listi;
    }
    public override int GetUnspentScore(){
        return score;
    }
}