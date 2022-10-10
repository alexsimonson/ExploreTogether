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
    public bool transition_period = false;
    public GameObject enemy_prefab;

    public GameObject[] respawns;
    public GameObject playerRespawns;
    public GameObject enemyRespawns;

    public Manager manager;

    public GameObject[] playerInventoryBackup;
    public GameObject[] playerGearBackup;

    // Start is called before the first frame update
    public override void Initialize(){
        manager = GameObject.Find("Manager").GetComponent<Manager>(); 
        manager.player.GetComponent<Inventory>().Initialize();
        manager.player.GetComponent<Gear>().Initialize();
        // enemy_prefab = Resources.Load("Prefabs/Enemy", typeof(GameObject)) as GameObject;
        respawns = GameObject.FindGameObjectsWithTag("Respawn");
        FilterRespawns();
        EndRound();
    }

    public override void SpawnEnemies(){
        while(enemies_currently_spawned < enemies_spawned_max && enemies_spawned_this_round < enemies_spawned_this_round_max){
            SpawnEnemy();
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

    void SpawnEnemy(){
        Instantiate(enemy_prefab, new Vector3(enemyRespawns.transform.position.x, enemyRespawns.transform.position.y + 1.5f, enemyRespawns.transform.position.z), enemyRespawns.transform.rotation);
        enemies_currently_spawned += 1;
        enemies_spawned_this_round += 1;
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
        // manager.player.transform.position = playerRespawns.transform.position;
        manager.player.transform.position = manager.playerSpawnPoint;
        EndRound();
    }

    public override void ProgressGameMode(){
        // only difference is we save the player's equipment and inventory heading into the next dungeon.  we also shouldn't reset the game stats to 0... so not entirely one change :)

        // these values should be saved for next round
        // playerInventoryBackup = manager.player.GetComponent<Inventory>().slots;
        // playerGearBackup = manager.player.GetComponent<Gear>().slots;
        manager.DestroyNonEssentialGameObjects();
        manager.Setup(true);
        EndRound();
        // ResetGameMode();
    }

    private void FilterRespawns(){
        foreach(GameObject respawn in respawns){
            if(respawn.GetComponent<RespawnPoint>().isPlayerSpawn){
                playerRespawns = respawn;
            }else{
                Debug.Log("Setting enemy respawn");
                enemyRespawns = respawn;
            }
        }
    }

    public override bool TransitionPeriod(){
        return transition_period;
    }
}