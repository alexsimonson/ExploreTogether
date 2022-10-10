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
    void Awake(){
        enemy_prefab = Resources.Load("Prefabs/Enemy", typeof(GameObject)) as GameObject;
        hudPrefab = Resources.Load("Prefabs/HUD", typeof(GameObject)) as GameObject;
        playerPrefab = Resources.Load("Prefabs/Player", typeof(GameObject)) as GameObject;
        game_mode_prefab = Resources.Load("Prefabs/WaveSurvivalGM", typeof(GameObject)) as GameObject;
        maze_generator_prefab = Resources.Load("Prefabs/MazeGenerator", typeof(GameObject)) as GameObject;
        item_bank = Resources.LoadAll<Item>("Items");
    }

    // Start is called before the first frame update
    void Start(){
        player = Instantiate(playerPrefab, playerSpawnPoint, Quaternion.identity);
        player.name = "Player";
        hud = Instantiate(hudPrefab);
        hud.name = "HUD";
        Setup();
    }

    public void Setup(bool backup=false){
        if(backup){
            // hud.transform.GetChild(3).gameObject.GetComponent<InventoryUI>().DrawInventoryUI();
        }
        maze = Instantiate(maze_generator_prefab);
        maze.GetComponent<Maze>().manager = gameObject.GetComponent<Manager>();
        // player.GetComponent<Inventory>().AddStartingItems();
        if(backup){
            // redraw the inventory/gear hud with this data
            // foreach(GameObject slot in game_rules.GetPlayerInventoryBackup()){
            //     player.GetComponent<Inventory>().AddItem(slot.GetComponent<SlotContainer>().inventorySlot.GetComponent<InventorySlot>().item);
            // }

            // also set these after player isntantiate later
            // player.GetComponent<Inventory>().slots = game_rules.GetPlayerInventoryBackup();
            // player.GetComponent<Gear>().slots = game_rules.GetPlayerGearBackup();
        }

        game_mode = Instantiate(game_mode_prefab, new Vector3(0, 0, 0), Quaternion.identity);
        game_rules = game_mode.GetComponent<IGameMode>();
        // these functions require both player and hud instantiated before they can work, and their functionality is required for the game to work properly
        // player.GetComponent<Inventory>().Initialize();
        // player.GetComponent<Gear>().Initialize();
        
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
