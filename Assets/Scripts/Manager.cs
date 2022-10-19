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


    // holding player inventory here for now during rehaul
    public Inventory player_inventory;
    public Gear player_gear;

    // eventually convert this to SO enum
    public enum GameState{
        Menu,
        Transition,
        Alive,
        Dead,
        Win
    }

    public GameState current_game_state;
    
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
        SetGameState(Manager.GameState.Transition);
        hud = Instantiate(hudPrefab);
        hud.name = "HUD";
        hud.transform.GetChild(8).gameObject.SetActive(true);
        player = Instantiate(playerPrefab, playerSpawnPoint, Quaternion.identity);
        player.name = "Player";
        player_inventory = ScriptableObject.CreateInstance("Inventory") as Inventory;
        // player_inventory
        player_gear = ScriptableObject.CreateInstance("Gear") as Gear;
        player_gear.Initialize();
        // maze = Instantiate(maze_generator_prefab);
        // maze.GetComponent<Maze>().manager = gameObject.GetComponent<Manager>();
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
        hud.transform.GetChild(8).gameObject.SetActive(false);
        player.transform.position = playerSpawnPoint;
        game_rules.Initialize();
        game_rules.SpawnEnemies();
        player.GetComponent<PlayerMovement>().AllowMovement();
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

    public void UpdateGameState(Component sender, object data){
        if(data is GameState){
            GameState _state = (GameState) data;
            SetGameState(_state);
            HandlePanels(_state);
        }
    }

    private void SetGameState(GameState _state){
        current_game_state = _state;
    }

    // this logic probably makes more sense in the individual panels themselves
    private void HandlePanels(GameState _state){
        if(_state==GameState.Dead){
            // show the death panel
            hud.transform.GetChild(2).gameObject.SetActive(true);
            // hide other competing panels
            hud.transform.GetChild(7).gameObject.SetActive(false);
            hud.transform.GetChild(8).gameObject.SetActive(false);
        }else if(_state==GameState.Alive){
            // this will be called after a transition period, set all of these to false
            hud.transform.GetChild(2).gameObject.SetActive(false);
            hud.transform.GetChild(7).gameObject.SetActive(false);
            hud.transform.GetChild(8).gameObject.SetActive(false);
        }else if(_state==GameState.Win){
            // show the objective panel
            hud.transform.GetChild(7).gameObject.SetActive(true);
            // hide other competing panels
            hud.transform.GetChild(2).gameObject.SetActive(false);
            hud.transform.GetChild(8).gameObject.SetActive(false);
        }else if(_state==GameState.Transition){
            // show the transition panel
            hud.transform.GetChild(8).gameObject.SetActive(true);
            // hide other competing panels
            hud.transform.GetChild(2).gameObject.SetActive(false);
            hud.transform.GetChild(7).gameObject.SetActive(false);
        }else if(_state==GameState.Menu){
            Debug.Log("Still need to setup a main menu");   // eventually this will just load the main menu scene
        }
    }
}
