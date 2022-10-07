using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    public GameObject hudPrefab;
    public GameObject game_mode_prefab;
    public GameObject maze_generator_prefab;
    public GameObject playerPrefab;
    public GameObject enemy_prefab;



    public GameObject player;
    public GameObject hud;
    public GameObject game_mode;
    public IGameMode game_rules;


    public GameObject maze;

    void Awake(){
        enemy_prefab = Resources.Load("Prefabs/Enemy", typeof(GameObject)) as GameObject;
        hudPrefab = Resources.Load("Prefabs/HUD", typeof(GameObject)) as GameObject;
        playerPrefab = Resources.Load("Prefabs/Player", typeof(GameObject)) as GameObject;
        game_mode_prefab = Resources.Load("Prefabs/WaveSurvivalGM", typeof(GameObject)) as GameObject;
        maze_generator_prefab = Resources.Load("Prefabs/MazeGenerator", typeof(GameObject)) as GameObject;
    }

    // Start is called before the first frame update
    void Start(){
        maze = Instantiate(maze_generator_prefab);
        maze.GetComponent<Maze>().manager = gameObject.GetComponent<Manager>();
        hud = Instantiate(hudPrefab);
        hud.name = "HUD";

        game_mode = Instantiate(game_mode_prefab, new Vector3(0, 0, 0), Quaternion.identity);
        game_rules = game_mode.GetComponent<IGameMode>();
        
        player = Instantiate(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        player.name = "Player";

        
    }

    void HandleRound(){
        game_rules.SetupNextRound();
        game_rules.Initialize();
        StartCoroutine(game_rules.BeginTransitionPeriod());
    }

    public void MazeGenerated(){
        // start the game mode
        game_rules.Initialize();
        game_rules.SpawnEnemies();
    }
}
