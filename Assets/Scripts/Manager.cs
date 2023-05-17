using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

public class Manager : MonoBehaviour {

    public GameObject hudPrefab;
    public GameMode game_mode;
    public GameObject playerPrefab;

    public GameObject player;
    public GameObject hud;


    public GameObject map;

    public Item[] item_bank;

    public bool pause_functionality = false;

    // public Vector3 playerSpawnPoint = new Vector3(0, 1.5f, 0);
    public Vector3 playerSpawnPoint = new Vector3(0, 2, 0);

    // holding player inventory here for now during rehaul
    public Inventory player_inventory;
    public Gear player_gear;

    public GameMode.Mode lobby_mode; // rename this later, but decoupling refactor will be utilizing this name

    public bool transition_period = false;

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
        item_bank = Resources.LoadAll<Item>("Items");
        hudPrefab = Resources.Load("Prefabs/HUD", typeof(GameObject)) as GameObject;
        playerPrefab = Resources.Load("Prefabs/Player", typeof(GameObject)) as GameObject;
        // we should load the game mode prefab based on the enum set
        game_mode = LoadGameMode();
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
        Setup();
    }

    public GameMode LoadGameMode(){
        Debug.Log("Lobby game mode: " + lobby_mode.ToString());
        if(lobby_mode==GameMode.Mode.Demo){
            return ScriptableObject.CreateInstance("Demo") as GameMode;
        }else if(lobby_mode==GameMode.Mode.DungeonCrawler){
            return ScriptableObject.CreateInstance("DungeonCrawler") as GameMode;
        }else if(lobby_mode==GameMode.Mode.WaveSurvival){
            return ScriptableObject.CreateInstance("WaveSurvival") as GameMode;
        }
        Debug.Log("Error: GameMode not set.  Please set a game mode to continue properly.");
        return null;
    }

    public void Setup(){
        game_mode.SpawnMap();
    }

    public void HandleRound(){
        game_mode.SetupNextRound();
        game_mode.Initialize();
        StartCoroutine(BeginTransitionPeriod());
    }

    public void MapSetupCallback(){
        // start the game mode
        Debug.Log("Map setup callback?");
        hud.transform.GetChild(8).gameObject.SetActive(false);
        player.transform.position = playerSpawnPoint;
        // commenting this out for now, as the race condition is stumping me
        game_mode.Initialize();  // this is causing an index race condition
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

    public void BeginTransition_SO(int wait_time=5){
        StartCoroutine(BeginTransitionPeriod(wait_time));
    }

    public IEnumerator BeginTransitionPeriod(int wait_time=5){
        transition_period = true;
        Debug.Log("Transition period started");
        yield return new WaitForSeconds(wait_time);
        Debug.Log("Transition period ending.");
        transition_period = false;
    }
}
