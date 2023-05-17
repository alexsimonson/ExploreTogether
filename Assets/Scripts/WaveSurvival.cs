using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSurvival : GameMode, IGameMode {

    bool killable_spawners = false;

    int current_round = 0;
    int score = 0;
    static int enemies_spawned_this_round_max_default = 5;
    int enemies_spawned_this_round_max = enemies_spawned_this_round_max_default;  // how many enemies will spawn and be defeated before the round ends
    int enemies_currently_spawned_max = 10;
    int enemies_currently_spawned = 0;
    int enemies_spawned_this_round = 0;
    int enemies_eliminated_this_round = 0;
    int spawners_this_round = 1;
    int spawners_eliminated_this_round = 0;

    int total_enemies_eliminated = 0;
    int total_score = 0;

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
    public Magic blood_wand;
    public Magic ice_wand;

    // game mode overhaul
    public GameObject maze_generator_prefab;

    public void Awake(){
        enemy_prefab = Resources.Load("Prefabs/Enemy", typeof(GameObject)) as GameObject;
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        sword_test = Resources.Load("Items/Sword", typeof(Melee)) as Melee;
        gun_test = Resources.Load("Items/Pistol", typeof(Gun)) as Gun;
        dungeon_pass = Resources.Load("Items/Dungeon Pass", typeof(Item)) as Item;
        blood_wand = Resources.Load("Items/Blood Wand", typeof(Magic)) as Magic;
        ice_wand = Resources.Load("Items/Ice Wand", typeof(Magic)) as Magic;
        maze_generator_prefab = Resources.Load("Prefabs/Arena/Arena", typeof(GameObject)) as GameObject;
    }

    public void Start(){
        mode = Mode.WaveSurvival;
        enemy_spawners = GameObject.FindGameObjectsWithTag("Respawn");
    }

    // determine when the wave will end based on some data
    void DetectWaveEnd(){
        if(killable_spawners && spawners_eliminated_this_round!=spawners_this_round){
            return; // game mode is not over
        }
        if(enemies_currently_spawned>0){
            return; // game mode is not over
        }
        ProgressGameMode();
    }

    public void SpawnerKilledListener(Component sender, object data){
        if(killable_spawners){
            spawners_eliminated_this_round += 1;
            Debug.Log("spawners_eliminated_this_round: " + spawners_eliminated_this_round);
            DetectWaveEnd();
        }
    }

    public void EnemyKilledListener(Component sender, object data){
        enemies_currently_spawned -= 1;
        enemies_eliminated_this_round += 1;
        total_enemies_eliminated += 1;
        score += 10;
        total_score += 10;
        manager.hud.transform.GetChild(9).gameObject.GetComponent<Text>().text = "Score: " + score.ToString();
        if(manager.game_mode.ShouldSpawnEnemy()){
            // pick a random spawn point and then call spawn enemy
            if(killable_spawners){
                enemy_spawners = GameObject.FindGameObjectsWithTag("Respawn");
            }
            // we could add code to remove the closest spawner to the player from the array and then choose randomly of those
            int rnd_index = Random.Range(0, enemy_spawners.Length);
            enemy_spawners[rnd_index].GetComponent<RespawnPoint>().SpawnEnemy();
        }
        DetectWaveEnd();
    }

    public void PlayerKilledListener(Component sender, object data){
        if(data is int && (int)data == 0){
            List<KeyValuePair<string, int>> scoreData = GetScoreData();
            foreach(KeyValuePair<string, int> score in scoreData){
                Debug.Log("key: " + score.Key);
                Debug.Log("Value: " + score.Value.ToString());
            }
        }
    }

    public void ScoreSpentListener(Component sender, object data){
        Debug.Log("score spent listener");
        if(data is int){
            score -= (int)data;
            manager.hud.transform.GetChild(9).gameObject.GetComponent<Text>().text = "Score: " + score.ToString();
        }
    }

    public override void SpawnMap(){
        // we should just implement the basic wave survival scenario
        manager.maze = Instantiate(maze_generator_prefab);
        // handle the navmesh now
        // manager.maze.GetComponent<Maze>().GetArenaPieces();
        // manager.maze.GetComponent<Maze>().BuildMapNavigation();
    }

    public override void Initialize(){
        manager.player_inventory = ScriptableObject.CreateInstance("Inventory") as Inventory;
        // there's something about the loading that's preventing items from being loaded at this exact point in time...
        manager.player_inventory.AddItem(sword_test);
        manager.player_inventory.AddItem(gun_test);
        manager.player_inventory.AddItem(blood_wand);
        manager.player_inventory.AddItem(ice_wand);
        EndRound("Initialize");
    }

    public override bool ShouldSpawnEnemy(){
        // if(enemies_currently_spawned < enemies_currently_spawned_max && enemies_spawned_this_round < enemies_spawned_this_round_max){
        // right now this spawner will constantly spawn enemies unless there are too many spawned at once
        if(killable_spawners && spawners_eliminated_this_round == spawners_this_round){
            Debug.Log("There are no more spawners available");
            return false;
        }
        if(enemies_currently_spawned < enemies_currently_spawned_max && enemies_spawned_this_round < enemies_spawned_this_round_max){
            // increase data, then return true to spawn the enemy
            enemies_currently_spawned += 1;
            enemies_spawned_this_round += 1;
            Debug.Log("enemies_currently_spawned: " + enemies_currently_spawned.ToString());
            return true;
        }
        return false;
    }

    // this function will be ran after all enemies are defeated
    void EndRound(string test = "test"){
        Debug.Log("End round caLLED with " + test);
        manager.BeginTransition_SO();
        SetupNextRound();
    }

    public override void SetupNextRound(){
        current_round += 1;
        manager.hud.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Round " + current_round.ToString();
        IncreaseEnemies();
        if(current_round!=1){
            RewardRoundBonus();
        }
        ResetRoundDefaults();
    }

    // this algorithm will determine how many enemies are present in each round
    void IncreaseEnemies(){
        enemies_spawned_this_round_max += 1;
        Debug.Log("New max enemies spawned this round: " + enemies_spawned_this_round_max.ToString());
    }

    void RewardRoundBonus(){
        score += 1000;
        total_score += 1000;
        manager.hud.transform.GetChild(9).gameObject.GetComponent<Text>().text = "Score: " + score.ToString();
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

    public override void ResetGameMode(){
        // we should delete every single game object that isn't essential
        manager.DestroyNonEssentialGameObjects();
        manager.Setup();
        current_round = 0;
        total_enemies_eliminated = 0;
        total_score = 0;
        score = 0;
        enemies_spawned_this_round_max = enemies_spawned_this_round_max_default;
        manager.player.transform.position = manager.playerSpawnPoint;
        EndRound("Reset GameMode");
    }

    public override void ProgressGameMode(){
        // pop the transition panel
        // manager.DestroyNonEssentialGameObjects();
        // SpawnMap();
        EndRound("ProgressGameMode");
        while(ShouldSpawnEnemy()){
            if(killable_spawners){
                enemy_spawners = GameObject.FindGameObjectsWithTag("Respawn");
            }
            // we could add code to remove the closest spawner to the player from the array and then choose randomly of those
            int rnd_index = Random.Range(0, enemy_spawners.Length);
            Debug.Log("rnd_index: " + rnd_index.ToString());
            enemy_spawners[rnd_index].GetComponent<RespawnPoint>().SpawnEnemy();
        }
    }

    public override bool TransitionPeriod(){
        return transition_period;
    }    

    public override List<KeyValuePair<string, int>> GetScoreData(){
        List<KeyValuePair<string, int>> highscoreList = new List<KeyValuePair<string, int>>();
        highscoreList.Add(new KeyValuePair<string, int>("death_round", current_round));
        highscoreList.Add(new KeyValuePair<string, int>("rounds_survived", current_round - 1));
        highscoreList.Add(new KeyValuePair<string, int>("total_enemies_eliminated", total_enemies_eliminated));
        highscoreList.Add(new KeyValuePair<string, int>("total_score", total_score));
        highscoreList.Add(new KeyValuePair<string, int>("unspent_score", score));
        return highscoreList;
    }

    public override int GetUnspentScore(){
        return score;
    }
}