using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    public Item[] item_bank;

    public bool pause_functionality = false;

    public Vector3 playerSpawnPoint = new Vector3(0, 1.5f, 0);

    public Item dungeon_pass;
    
    void Awake(){
        enemy_prefab = Resources.Load("Prefabs/Enemy", typeof(GameObject)) as GameObject;
        hudPrefab = Resources.Load("Prefabs/HUD", typeof(GameObject)) as GameObject;
        playerPrefab = Resources.Load("Prefabs/Player", typeof(GameObject)) as GameObject;
        game_mode_prefab = Resources.Load("Prefabs/WaveSurvivalGM", typeof(GameObject)) as GameObject;
        maze_generator_prefab = Resources.Load("Prefabs/MazeGenerator", typeof(GameObject)) as GameObject;
        item_bank = Resources.LoadAll<Item>("Items");
        dungeon_pass = Resources.Load("Items/Dungeon Pass", typeof(Item)) as Item;
    }

    // Start is called before the first frame update
    void Start(){
        player = Instantiate(playerPrefab, playerSpawnPoint, Quaternion.identity);
        player.name = "Player";
        hud = Instantiate(hudPrefab);
        hud.name = "HUD";
        Setup();
    }

    public void Setup(){
        maze = Instantiate(maze_generator_prefab);
        maze.GetComponent<Maze>().manager = gameObject.GetComponent<Manager>();
        game_mode = Instantiate(game_mode_prefab, new Vector3(0, 0, 0), Quaternion.identity);
        game_rules = game_mode.GetComponent<IGameMode>();
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

    public Item GenerateItem(){
        Item generated = null;
        while(generated == null || generated.id==999){
            // we should generate a new item to try and return
            int rnd_index = Random.Range(0, item_bank.Length);
            generated = item_bank[rnd_index];
        }
        return generated;
    }

    public void DestroyNonEssentialGameObjects(){
        GameObject[] get_all = FindObjectsOfType<GameObject>() as GameObject[];
        foreach(GameObject obj in get_all){
            if(obj.tag!="essential" && obj.tag!="Player"){
                Destroy(obj);
            }
        }
    }
}
