using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExploreTogether {
    [CreateAssetMenu(fileName = "New Game Mode", menuName = "Game Mode")]
    public abstract class GameMode : ScriptableObject, IGameMode {
        
        public enum Mode{
            WaveSurvival,
            DungeonCrawler,
            Demo
        }

        public Manager manager;
        public Mode mode;
        public int score = 0;
        public bool transition_period;
        public GameObject[] playerInventoryBackup;
        public GameObject[] playerGearBackup;

        public abstract bool TransitionPeriod();
        public abstract void Initialize();
        public abstract void SetupNextRound();
        public abstract bool ShouldSpawnEnemy();
        public abstract void ResetGameMode();
        public abstract GameObject[] GetPlayerInventoryBackup();
        public abstract GameObject[] GetPlayerGearBackup();
        public abstract void ProgressGameMode();
        public abstract void SpawnMap(bool isLoading=false);
        public abstract List<KeyValuePair<string, int>> GetScoreData();
        public abstract int GetUnspentScore();
        // adding these in from Maze.cs
        // just need to track the data of spawned items for save/load
        private List<KeyValuePair<int, Vector3>> spawned_items = new List<KeyValuePair<int, Vector3>>();
        // track enemies for save/load
        private List<KeyValuePair<string, Transform>> spawned_enemies = new List<KeyValuePair<string, Transform>>();
        private List<KeyValuePair<string, Transform>> spawned_resources = new List<KeyValuePair<string, Transform>>();
        public GameObject itemSpawn=null;

        public Dictionary<string, GameObject> loaded_resources = new Dictionary<string, GameObject>();

        public bool SpawnItem(int _item_id, Vector3? _position=null){
            if(itemSpawn==null){
                itemSpawn = Resources.Load("Prefabs/Item", typeof(GameObject)) as GameObject;
            }
            // we need to adjust the items position by a minimal amount to allow the AI to pass during navigation.
            // providing random offset for x and z positions
            int xOffset = Random.Range(0, 1)==0 ? -1 : 1;
            int zOffset = Random.Range(0, 1)==0 ? -1 : 1;
            

            if(manager.item_bank.ContainsKey(_item_id)==false){
                Debug.LogError("Invalid item id during SpawnItem");
                return false;
            }
            itemSpawn.GetComponent<ItemSpawn>().item = manager.item_bank[_item_id];  // this should be passed in
            Vector3 adjustItemSpawnPoint;
            
            if(_position!=null){
                adjustItemSpawnPoint = _position.Value;
            }else{
                adjustItemSpawnPoint = RandomSpawnPointInGenerated(5);  // adjust padding to be more lenient than other things
            }
            GameObject itemSpawned = Instantiate(itemSpawn, adjustItemSpawnPoint, Quaternion.identity);
            // after instantiation, track for export
            // GameObject itemSpawned = Instantiate(itemSpawn, new Vector3(generated_nodes[random_index].mazePosition.x * prefabSize, generated_nodes[random_index].mazePosition.y * prefabSize + 1.5f, generated_nodes[random_index].mazePosition.z * prefabSize), Quaternion.identity);
            itemSpawned.transform.SetParent(manager.map.transform); // where do we instantiate this shit???
            itemSpawned.name = "Item: " + itemSpawn.GetComponent<ItemSpawn>().item.name.ToString();
            spawned_items.Add(new KeyValuePair<int, Vector3>(itemSpawn.GetComponent<ItemSpawn>().item.id, adjustItemSpawnPoint));
            return true;
        }

        public Vector3 RandomSpawnPointInGenerated(int _padding=10){
            int _node_index = Random.Range(_padding, manager.map.GetComponent<Maze>().generated_nodes.Count - _padding);
            return new Vector3(manager.map.GetComponent<Maze>().generated_nodes[_node_index].mazePosition.x * manager.map.GetComponent<Maze>().prefabSize, manager.map.GetComponent<Maze>().generated_nodes[_node_index].mazePosition.y * manager.map.GetComponent<Maze>().prefabSize + 1.5f, manager.map.GetComponent<Maze>().generated_nodes[_node_index].mazePosition.z * manager.map.GetComponent<Maze>().prefabSize);
        }

        public Vector3 SpawnPointAtIndex(int _index){
            return new Vector3(manager.map.GetComponent<Maze>().generated_nodes[_index].mazePosition.x * manager.map.GetComponent<Maze>().prefabSize, manager.map.GetComponent<Maze>().generated_nodes[_index].mazePosition.y * manager.map.GetComponent<Maze>().prefabSize + 1.5f, manager.map.GetComponent<Maze>().generated_nodes[_index].mazePosition.z * manager.map.GetComponent<Maze>().prefabSize);
        }

        public void SpawnResource(string _resource_location, Vector3? _position=null, string _name="spawned prefab"){
            GameObject _resource_prefab;
            if(loaded_resources.ContainsKey(_resource_location)){
                _resource_prefab = loaded_resources[_resource_location];
            }else{
                _resource_prefab = Resources.Load(_resource_location, typeof(GameObject)) as GameObject;
                loaded_resources.Add(_resource_location, _resource_prefab);
            }
            Vector3 _resource_spawn_point;
            if(_position==null){
                // random spawn point based on index
                _resource_spawn_point = RandomSpawnPointInGenerated();
            }else{
                _resource_spawn_point = _position.Value;
            }
            GameObject spawned_prefab = Instantiate(_resource_prefab, _resource_spawn_point, Quaternion.identity);
            spawned_prefab.transform.SetParent(manager.map.transform);  // where do we instantiate this shit???
            spawned_prefab.name = _name;
            spawned_resources.Add(new KeyValuePair<string, Transform>(_resource_location, spawned_prefab.transform));
        }

        public void SpawnEnemy(GameObject _enemy_prefab, Vector3? _position=null, string _name="spawned_enemy_name"){
            // ensure the mazePosition is n-depth away from the player (we need a function for this)
            GameObject enemy = null;
            Vector3 enemySpawnPoint;
            if(_position==null){
                // random spawn point based on index
                enemySpawnPoint = RandomSpawnPointInGenerated();
            }else{
                enemySpawnPoint = _position.Value;
            }
            enemy = Instantiate(_enemy_prefab, enemySpawnPoint, Quaternion.identity);
            // enemy.transform.SetParent(manager.map.transform);    // where do we instantiate this shit???
            enemy.name = "Enemy";
            spawned_enemies.Add(new KeyValuePair<string, Transform>("Prefabs/ModernMan", enemy.transform));
        }

        public List<MazeSpawnedItemSerializable> ExportSpawnedItems(){
            List<MazeSpawnedItemSerializable> export_spawned_items = new List<MazeSpawnedItemSerializable>();
            foreach(KeyValuePair<int, Vector3> spawned_item in spawned_items){
                export_spawned_items.Add(new MazeSpawnedItemSerializable(spawned_item.Key, spawned_item.Value));
            }
            return export_spawned_items;
        }

        public bool ImportSavedItems(List<MazeSpawnedItemSerializable> _load_spawned_items){
            if(_load_spawned_items==null || _load_spawned_items.Count==0){
                return false;
            }
            manager.game_mode.spawned_items.Clear();
            for(int i=0;i<_load_spawned_items.Count;i++){
                // really... this function should call the Instantiation of the items...
                // manager.game_mode.spawned_items.Add(_load_spawned_items[i].ToKeyValue()); // this will happen within the spawn item function...
                SpawnItem(_load_spawned_items[i].item_id, _load_spawned_items[i].maze_position);
            }
            return true;
        }

        public List<MazeSpawnedObjectSerializable> ExportSpawnedEnemies(){
            List<MazeSpawnedObjectSerializable> export_spawned_enemies = new List<MazeSpawnedObjectSerializable>();
            foreach(KeyValuePair<string, Transform> spawned_enemy in spawned_enemies){
                export_spawned_enemies.Add(new MazeSpawnedObjectSerializable(spawned_enemy.Key, spawned_enemy.Value.position, spawned_enemy.Value.gameObject.name));
            }
            return export_spawned_enemies;
        }

        public bool ImportSavedEnemies(List<MazeSpawnedObjectSerializable> _load_spawned_enemies){
            if(_load_spawned_enemies==null || _load_spawned_enemies.Count==0){
                return false;
            }
            manager.game_mode.spawned_enemies.Clear();
            for(int i=0;i<_load_spawned_enemies.Count;i++){
                GameObject _enemy_prefab;
                if(loaded_resources.ContainsKey(_load_spawned_enemies[i].object_prefab)){
                    _enemy_prefab = loaded_resources[_load_spawned_enemies[i].object_prefab];
                }else{
                    _enemy_prefab = Resources.Load(_load_spawned_enemies[i].object_prefab, typeof(GameObject)) as GameObject;
                    loaded_resources.Add(_load_spawned_enemies[i].object_prefab, _enemy_prefab);
                }
                SpawnEnemy(_enemy_prefab, _load_spawned_enemies[i].maze_position);
            }
            return true;
        }

        public List<MazeSpawnedObjectSerializable> ExportSpawnedResources(){
            List<MazeSpawnedObjectSerializable> export_spawned_resources = new List<MazeSpawnedObjectSerializable>();
            foreach(KeyValuePair<string, Transform> spawned_resource in spawned_resources){
                export_spawned_resources.Add(new MazeSpawnedObjectSerializable(spawned_resource.Key, spawned_resource.Value.position, spawned_resource.Value.gameObject.name));
            }
            return export_spawned_resources;
        }

        public bool ImportSavedResources(List<MazeSpawnedObjectSerializable> _load_spawned_resources){
            if(_load_spawned_resources==null || _load_spawned_resources.Count==0){
                Debug.LogError("Failed to import saved resources");
                return false;
            }
            manager.game_mode.spawned_resources.Clear();
            for(int i=0;i<_load_spawned_resources.Count;i++){
                SpawnResource(_load_spawned_resources[i].object_prefab, _load_spawned_resources[i].maze_position, _load_spawned_resources[i].object_name);
            }
            return true;
        }

        public bool FindAndRemoveItem(int _item_id, Vector3 _position){
            for(int i=0;i<spawned_items.Count;i++){
                if(spawned_items[i].Key!=_item_id || spawned_items[i].Value!=_position){
                    continue;   // not the right item...
                }
                spawned_items.RemoveAt(i);
                return true;
            }
            Debug.LogError("Failed to find and remove item");
            return false;
        }

        public bool AddSpawnedItem(int _item_id, Vector3 _drop_position){
            if(_item_id==-1 || _drop_position==null){
                Debug.LogError("Invalid item add attempt");
                return false;
            }
            manager.game_mode.spawned_items.Add(new KeyValuePair<int, Vector3>(_item_id, _drop_position));
            return true;
        }

        public bool FindAndRemoveEnemy(Transform _remove_transform){
            for(int i=0;i<spawned_enemies.Count;i++){
                if(spawned_enemies[i].Value.position!=_remove_transform.position){
                    continue;   // not the right enemy...
                }
                spawned_enemies.RemoveAt(i);
                return true;
            }
            Debug.LogError("Failed to find and remove enemy");
            return false;
        }
    }
}