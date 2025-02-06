using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using System.Threading.Tasks;


namespace ExploreTogether {
    
    public class Maze : MonoBehaviour {

        public GameObject corner;
        public GameObject hall;
        public GameObject tJunc;
        public GameObject xJunc;
        public GameObject cJunc;
        public GameObject itemSpawn;
        public GameObject enemyPrefab;
        public GameObject torchPrefab;
        public GameObject treePrefab;

        private int prefabSize = 5;

        private List<Vector3> neighbors_visited = new List<Vector3>();
        private Stack<GeneratedNode> generating_stack = new Stack<GeneratedNode>();

        public List<GeneratedNode> generated_nodes = new List<GeneratedNode>();

        public List<GeneratedNode> xJunctions = new List<GeneratedNode>();

        public int max_width;
        public int max_height;
        public int max_dungeon_length = 1000;
        public int total_count = 0;

        bool controlled_render = false;
        bool controlled_generation = true;
        public bool maze_generated = false;
        public bool needs_generation = true;

        public Manager manager;

        public List<NavMeshSurface> surfaces = new List<NavMeshSurface>();
        public NavigationBuilder nav_builder;

        // just need to track the data of spawned items for save/load
        private List<KeyValuePair<int, Vector3>> spawned_items = new List<KeyValuePair<int, Vector3>>();
        // track enemies for save/load
        private List<KeyValuePair<string, Transform>> spawned_enemies = new List<KeyValuePair<string, Transform>>();
        private List<KeyValuePair<string, Transform>> spawned_resources = new List<KeyValuePair<string, Transform>>();



        void Awake(){
            corner = Resources.Load("Prefabs/Dungeon/Corner", typeof(GameObject)) as GameObject;
            hall = Resources.Load("Prefabs/Dungeon/Hall", typeof(GameObject)) as GameObject;
            tJunc = Resources.Load("Prefabs/Dungeon/T-Junction", typeof(GameObject)) as GameObject;
            xJunc = Resources.Load("Prefabs/Dungeon/X-Junction", typeof(GameObject)) as GameObject;
            cJunc = Resources.Load("Prefabs/Dungeon/C-Junction", typeof(GameObject)) as GameObject;
            itemSpawn = Resources.Load("Prefabs/Item", typeof(GameObject)) as GameObject;
            enemyPrefab = Resources.Load("Prefabs/ModernMan", typeof(GameObject)) as GameObject;
            torchPrefab = Resources.Load("Prefabs/Torch Particle", typeof(GameObject)) as GameObject;
            treePrefab = Resources.Load("Prefabs/TreeResource", typeof(GameObject)) as GameObject;
        }

        void Start(){
            manager = GameObject.Find("Manager").GetComponent<Manager>();
            nav_builder = gameObject.GetComponent<NavigationBuilder>();
        }

        // Start is called before the first frame update
        void Update(){
            // we should only generate a new maze if we do not have a list of generated nodes
            // && generated_nodes.Count==0
            if(controlled_generation && needs_generation){
                controlled_generation = false;
                GenerateRandomMaze();
            }

            if(controlled_generation && !needs_generation){
                controlled_generation = false;
                RenderGrid();
                BuildMapNavigation();
                if(ImportSavedItems(manager.chosen_character_data.spawned_items)==false){
                    Debug.LogError("We failed to import spawned items");
                }
                if(ImportSavedEnemies(manager.chosen_character_data.enemy_positions)==false){
                    Debug.LogError("We failed to import spawned enemies");
                }
                if(ImportSavedResources(manager.chosen_character_data.spawned_resources)==false){
                    Debug.LogError("We failed to import saved resources");
                }
            }

            // Debug.Log("Testing generated nodes length: " + generated_nodes.Count.ToString());
            if(controlled_render){
                controlled_render = false;
                StartCoroutine(ControlledRender());
            }
        }

        void GenerateRandomMaze(){
            // given starting point 0, 0, find empty neighbors
            Vector3 start_pos = new Vector3(0, 0, 0);
            GridRecursiveBacktrackGenerator(start_pos, null, true);
            if(!controlled_generation){
                RenderGrid();
            }
            maze_generated = true;
            BuildMapNavigation();
            FillMazeWithItems();
            // when the maze is generated we should alert the manager if there is one
            if(manager==null) return;
            manager.MapSetupCallback();
        }

        public void BuildMapNavigation(){
            // doesn't work because not available or something...
            nav_builder.BuildNavigation(surfaces);
        }

        List<Vector3> FindNeighbors(Vector3 position){
            // given a position, return a list of the neighbors
            var x = position.x;
            var y = position.y;
            var z = position.z;

            List<Vector3> neighbors = new List<Vector3>();
            neighbors.Add(new Vector3(x+1, y, z));
            neighbors.Add(new Vector3(x-1, y, z));
            neighbors.Add(new Vector3(x, y, z+1));
            neighbors.Add(new Vector3(x, y, z-1));
            return neighbors;
        }

        List<Vector3> FindEmptyNeighbors(List<Vector3> neighbors){
            List<Vector3> filter = new List<Vector3>();
            foreach(Vector3 neighbor in neighbors){
                if(!neighbors_visited.Contains(neighbor)) filter.Add(neighbor);
            }
            return filter;
        }

        // this takes a position that was simple and data oriented and converts to the real world location
        GameObject InstantiateModified(GeneratedNode node, GameObject prefab, int rotation_degrees=0){
            Vector3 modified_position = new Vector3(node.mazePosition.x * prefabSize, node.mazePosition.y, node.mazePosition.z * prefabSize);
            GameObject io = Instantiate(prefab, modified_position, Quaternion.identity);
            Vector3 existing_rot = io.transform.rotation.eulerAngles;
            existing_rot.y = rotation_degrees;
            io.transform.rotation = Quaternion.Euler(existing_rot);
            io.name = "Testing " + node.index.ToString() + " - " + prefab.name.ToString();

            // EXPERIMENTAL****************
            // Get the prefab's rotation

            // Calculate texture offset or rotation_degrees correction
            // Vector2 offset = Vector2.zero;
            // if (Mathf.Approximately(rotation_degrees % 360, 90f) || Mathf.Approximately(rotation_degrees % 360, 270f))
            // {
            //     offset = new Vector2(0.5f, 0.5f); // Example adjustment for 90-degree rotations
            // }

            // // Apply offset to the material
            // Material material = io.transform.Find("Floor").GetComponent<MeshRenderer>().material;
            // material.mainTextureOffset = offset;
            // material.mainTextureScale = new Vector2(5, 5); // Ensure tiling stays consistent
            // EXPERIMENTAL****************
            return io;
        }

        // parameters must define start/end conditions of this function
        void GridRecursiveBacktrackGenerator(Vector3 position, GeneratedNode last_node=null, bool first_pass=false){
            // given a position we must do the stuff
            // Instantiate(xJunc, position, Quaternion.identity);   // commented out while I redesign this logic
            // we should also create a new GeneratedNode
            neighbors_visited.Add(position);
            GeneratedNode current_node = ScriptableObject.CreateInstance("GeneratedNode") as GeneratedNode;
            current_node.Init(position, total_count);
            generating_stack.Push(current_node);
            generated_nodes.Add(current_node);
            // this functionality must be called before we return
            if(!first_pass && last_node!=null){
                // determine the cardinal direction of the lastNode from position...
                // then set this position into the lastPositions 
                
                // I MUST COMPARE THE TWO VECTORS NOW
                if(position.x > last_node.mazePosition.x){
                    // let's consider this EAST of the last position
                    last_node.eastNeighbor = current_node;
                    current_node.westNeighbor = last_node;
                }else if(position.x < last_node.mazePosition.x){
                    // let's consider this WEST of the last position
                    last_node.westNeighbor = current_node;
                    current_node.eastNeighbor = last_node;
                }else if(position.z > last_node.mazePosition.z){
                    // let's consider this NORTH of the last position
                    last_node.northNeighbor = current_node;
                    current_node.southNeighbor = last_node;
                }else if(position.z < last_node.mazePosition.z){
                    // let's consider this SOUTH of the last position
                    last_node.southNeighbor = current_node;
                    current_node.northNeighbor = last_node;
                }else{
                    Debug.Log("SOMETHING WENT HORRIBLY WRONG");
                    // this would be diagonal so it shouldn't ever happen in this algorithm
                    if(last_node.mazePosition.x > position.x && last_node.mazePosition.z > position.z){

                    }
                }
            }
            List<Vector3> empty_neighbors = FindEmptyNeighbors(FindNeighborsGrid(current_node.mazePosition));
            if(total_count==max_dungeon_length){
                return;
            }
            // when there are no empty neighbors, I should pop the stack and "backtrack" to try the last node
            bool control = true;
            while(empty_neighbors.Count <= 0 && control){
                generating_stack.Pop();
                if(generating_stack.Count <= 0){
                    control = false;
                }else{
                    current_node = generating_stack.Peek();
                    empty_neighbors = FindEmptyNeighbors(FindNeighborsGrid(current_node.mazePosition));
                }
            }
            // otherwise we will choose a random neighbor and go
            if(empty_neighbors.Count <=0){
                return;
            }
            Vector3 random_neighbor = empty_neighbors[Random.Range(0, empty_neighbors.Count)];

            total_count++;
            GridRecursiveBacktrackGenerator(random_neighbor, current_node);
        }

        // this will limit the function to only finding coordinates in a grid
        List<Vector3> FindNeighborsGrid(Vector3 position){
            // given a position, return a list of the neighbors
            var x = position.x;
            var y = position.y;
            var z = position.z;

            List<Vector3> neighbors = new List<Vector3>();
            if(x+1 < max_width) neighbors.Add(new Vector3(x+1, y, z));
            if(x-1 >= 0) neighbors.Add(new Vector3(x-1, y, z));
            if(z+1 < max_height) neighbors.Add(new Vector3(x, y, z+1));
            if(z-1 >= 0) neighbors.Add(new Vector3(x, y, z-1));
            return neighbors;
        }

        void RenderGrid(){
            // given a "linked list" path of nodes, instantiate, and rotate accordingly the prefab into position
            foreach(GeneratedNode node in generated_nodes){
                // check out the node's neighbors, then determine the piece
                DetermineAndInstantiateNodePiece(node);
            }
        }
        
        IEnumerator ControlledRender(float wait_time=.1f){
            foreach(GeneratedNode node in generated_nodes){
                DetermineAndInstantiateNodePiece(node);
                yield return new WaitForSeconds(wait_time);
            }
        }

        void DetermineAndInstantiateNodePiece(GeneratedNode node){
            // based on certain path parameters of a given node, determine which prefab will render
            int neighbor_count = 0;
            if(node.northNeighbor) neighbor_count++;
            if(node.southNeighbor) neighbor_count++;
            if(node.eastNeighbor) neighbor_count++;
            if(node.westNeighbor) neighbor_count++;
            if(neighbor_count==0){
                Debug.Log("Neighbor count must be greater than 0.  Did you run this function before generating the grid?");
                return;
            }

            GameObject node_prefab = null;
            int rotation_degrees = 0;
            // triage based on count
            if(neighbor_count==4){
                node_prefab = xJunc;
                xJunctions.Add(node);
            }else if(neighbor_count==3){
                node_prefab = tJunc;
                if(!node.eastNeighbor){
                    // rotation_degrees already set to 0
                }else{
                    if(!node.westNeighbor){
                        rotation_degrees = 180;
                    }else if(!node.northNeighbor){
                        rotation_degrees = -90;
                    }else if(!node.southNeighbor){
                        rotation_degrees = 90;
                    }
                }
            }else if(neighbor_count==1){
                node_prefab = cJunc;
                if(!node.eastNeighbor){
                    // we must rotate
                    if(node.westNeighbor){
                        rotation_degrees = 180;
                    }else if(node.northNeighbor){
                        rotation_degrees = -90;
                    }else if(node.southNeighbor){
                        rotation_degrees = 90;
                    }
                }
            }else{
                if(node.northNeighbor && node.southNeighbor){
                    node_prefab = hall;
                    rotation_degrees = 90;
                }else if(node.eastNeighbor && node.westNeighbor){
                    node_prefab = hall;
                }else{
                    node_prefab = corner;
                    if(!node.northNeighbor && !node.eastNeighbor){
                        rotation_degrees = 180;
                    }else if(!node.northNeighbor){
                        // this means the east neighbor must exist, and the other is south
                        rotation_degrees = 90;
                    }else if(!node.eastNeighbor){
                        // this means the north neighbor must exist, and the other is west
                        rotation_degrees = -90;
                    }
                }
            }
            GameObject spawnedMazePiece = InstantiateModified(node, node_prefab, rotation_degrees);
            spawnedMazePiece.transform.SetParent(gameObject.transform);
            // surfaces.Add(spawnedMazePiece.transform.GetChild(0).GetComponent<NavMeshSurface>());
            surfaces.Add(spawnedMazePiece.GetComponent<NavMeshSurface>());
        }

        public void GetArenaPieces(){
            Transform[] ts = gameObject.GetComponentsInChildren<Transform>();
            if(ts==null){
                Debug.Log("Returning sad");
                return;
            }
            foreach(Transform t in ts){

                if(gameObject.tag!="Respawn"){
                    surfaces.Add(t.gameObject.GetComponent<NavMeshSurface>());
                }
            }
            Debug.Log("surfaces is ready");
        }

        void FillMazeWithItems(int _num_items=20){

            // x junction is not reliable in smaller mazes, until we overhaul our random generation algorithm to more randomly place junctions
            // I could experiment with randomly overriding an x-junc into the map??
            // foreach(GeneratedNode xJunction in xJunctions){
            //     // we should instantiate an item here
            //     Instantiate(itemSpawn, new Vector3(xJunction.mazePosition.x * prefabSize, xJunction.mazePosition.y * prefabSize + 1.5f, xJunction.mazePosition.z * prefabSize), Quaternion.identity);
            // }
            // we must instantiate a dungeon key somewhere
            SpawnItem(999);

            // let's place the objective at the final generated location
            SpawnResource("Prefabs/Objective", generated_nodes.Count - 1, "Objective");
            SpawnResource("Prefabs/DungeonEntrance", 0, "DungeonEntrance");
            
            // item spawning
            for(int i=0;i<_num_items;i++){
                SpawnItem(manager.GenerateItem().id, null);
                // spawn enemy every 3rd item
                if(i%4==2){
                    SpawnEnemy(enemyPrefab, null);    // this function needs to be the standardized spawn function.... not a different one here
                }
                if(i%4==2){
                    SpawnResource("Prefabs/TreeResource", -1, "tree_prefab_"+i.ToString());
                }
            }
        }

        bool SpawnItem(int _item_id, Vector3? _position=null){
            int random_index = Random.Range(5, generated_nodes.Count - 5);
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
                adjustItemSpawnPoint = new Vector3(generated_nodes[random_index].mazePosition.x * prefabSize + xOffset, generated_nodes[random_index].mazePosition.y * prefabSize + 1.5f, generated_nodes[random_index].mazePosition.z * prefabSize + zOffset);
            }
            GameObject itemSpawned = Instantiate(itemSpawn, adjustItemSpawnPoint, Quaternion.identity);
            // after instantiation, track for export
            // GameObject itemSpawned = Instantiate(itemSpawn, new Vector3(generated_nodes[random_index].mazePosition.x * prefabSize, generated_nodes[random_index].mazePosition.y * prefabSize + 1.5f, generated_nodes[random_index].mazePosition.z * prefabSize), Quaternion.identity);
            itemSpawned.transform.SetParent(gameObject.transform);
            itemSpawned.name = "Item: " + itemSpawn.GetComponent<ItemSpawn>().item.name.ToString();
            spawned_items.Add(new KeyValuePair<int, Vector3>(itemSpawn.GetComponent<ItemSpawn>().item.id, adjustItemSpawnPoint));
            return true;
        }

        // this was probably meant to be more generic for all spawns, but let's just turn this into resource nodes
        // this will allow easy save/load
        public void SpawnResource(string _object_prefab, int _index=-1, string _name="spawned prefab"){
            if(_index==-1){
                _index = Random.Range(25, generated_nodes.Count - 25);
            }
            // ensure the mazePosition is n-depth away from the player (we need a function for this)
            GameObject _resource_prefab = Resources.Load(_object_prefab, typeof(GameObject)) as GameObject;
            GameObject spawned_prefab = Instantiate(_resource_prefab, new Vector3(generated_nodes[_index].mazePosition.x * prefabSize, generated_nodes[_index].mazePosition.y * prefabSize + 1.5f, generated_nodes[_index].mazePosition.z * prefabSize), Quaternion.identity);
            spawned_prefab.transform.SetParent(gameObject.transform);
            spawned_prefab.name = _name;
            spawned_resources.Add(new KeyValuePair<string, Transform>(_object_prefab, spawned_prefab.transform));
        }

        public void SpawnResourcePosition(string _object_prefab, Vector3 _position, string _name="spawned prefab"){
            GameObject _resource_prefab = Resources.Load(_object_prefab, typeof(GameObject)) as GameObject;
            GameObject spawned_prefab = Instantiate(_resource_prefab, _position, Quaternion.identity);
            spawned_prefab.transform.SetParent(gameObject.transform);
            spawned_prefab.name = _name;
            spawned_resources.Add(new KeyValuePair<string, Transform>(_object_prefab, spawned_prefab.transform));
        }

        public void SpawnEnemy(GameObject _enemy_prefab, Vector3? _position=null){
            // ensure the mazePosition is n-depth away from the player (we need a function for this)
            GameObject enemy = null;
            int index;
            Vector3 enemySpawnPoint;
            if(_position==null){
                // random spawn point based on index
                index = Random.Range(10, generated_nodes.Count - 10);
                enemySpawnPoint = new Vector3(generated_nodes[index].mazePosition.x * prefabSize, generated_nodes[index].mazePosition.y * prefabSize + 1.5f, generated_nodes[index].mazePosition.z * prefabSize);
            }else{
                enemySpawnPoint = _position.Value;
            }
            enemy = Instantiate(_enemy_prefab, enemySpawnPoint, Quaternion.identity);
            // enemy.transform.SetParent(gameObject.transform);
            enemy.name = "Enemy";
            spawned_enemies.Add(new KeyValuePair<string, Transform>("Prefabs/ModernMan", enemy.transform));
        }

        public List<GeneratedNodeSerializable> ExportMaze(){
            // slot index will correlate with array index
            List<GeneratedNodeSerializable> export_maze_data = new List<GeneratedNodeSerializable>();
            
            foreach(GeneratedNode node in manager.map.GetComponent<Maze>().generated_nodes){
                // convert to serializable list
                export_maze_data.Add(new GeneratedNodeSerializable(node));
                if(node.northNeighbor!=null){
                    export_maze_data[node.index].northNeighbor = node.northNeighbor.index;
                }
                if(node.southNeighbor!=null){
                    export_maze_data[node.index].southNeighbor = node.southNeighbor.index;
                }
                if(node.eastNeighbor!=null){
                    export_maze_data[node.index].eastNeighbor = node.eastNeighbor.index;
                }
                if(node.westNeighbor!=null){
                    export_maze_data[node.index].westNeighbor = node.westNeighbor.index;
                }

            }
            return export_maze_data;
        }

        public bool ImportMaze(List<GeneratedNodeSerializable> dungeon_nodes){
            if(dungeon_nodes==null || dungeon_nodes.Count==0){
                return false;
            }
            // reset nodes
            manager.map.GetComponent<Maze>().generated_nodes.Clear();
            for(int i=0;i<dungeon_nodes.Count;i++){ 
                manager.map.GetComponent<Maze>().generated_nodes.Add(dungeon_nodes[i].ToScriptableObject());
            }
            // after we import/adjust the generated_nodes... we should update the neighbors
            for(int i=0;i<manager.map.GetComponent<Maze>().generated_nodes.Count;i++){
                if(dungeon_nodes[i].northNeighbor!=-1 && dungeon_nodes[i].northNeighbor < manager.map.GetComponent<Maze>().generated_nodes.Count){
                    manager.map.GetComponent<Maze>().generated_nodes[i].northNeighbor = manager.map.GetComponent<Maze>().generated_nodes[dungeon_nodes[i].northNeighbor];
                }
                if(dungeon_nodes[i].southNeighbor!=-1 && dungeon_nodes[i].southNeighbor < manager.map.GetComponent<Maze>().generated_nodes.Count){
                    manager.map.GetComponent<Maze>().generated_nodes[i].southNeighbor = manager.map.GetComponent<Maze>().generated_nodes[dungeon_nodes[i].southNeighbor];
                }
                if(dungeon_nodes[i].eastNeighbor!=-1 && dungeon_nodes[i].eastNeighbor < manager.map.GetComponent<Maze>().generated_nodes.Count){
                    manager.map.GetComponent<Maze>().generated_nodes[i].eastNeighbor = manager.map.GetComponent<Maze>().generated_nodes[dungeon_nodes[i].eastNeighbor];
                }
                if(dungeon_nodes[i].westNeighbor!=-1 && dungeon_nodes[i].westNeighbor < manager.map.GetComponent<Maze>().generated_nodes.Count){
                    manager.map.GetComponent<Maze>().generated_nodes[i].westNeighbor = manager.map.GetComponent<Maze>().generated_nodes[dungeon_nodes[i].westNeighbor];
                }
            }
            return true;
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
            manager.map.GetComponent<Maze>().spawned_items.Clear();
            for(int i=0;i<_load_spawned_items.Count;i++){
                // really... this function should call the Instantiation of the items...
                // manager.map.GetComponent<Maze>().spawned_items.Add(_load_spawned_items[i].ToKeyValue()); // this will happen within the spawn item function...
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
            manager.map.GetComponent<Maze>().spawned_enemies.Clear();
            for(int i=0;i<_load_spawned_enemies.Count;i++){
                GameObject _enemy_prefab = Resources.Load(_load_spawned_enemies[i].object_prefab, typeof(GameObject)) as GameObject;
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
            manager.map.GetComponent<Maze>().spawned_resources.Clear();
            for(int i=0;i<_load_spawned_resources.Count;i++){
                SpawnResourcePosition(_load_spawned_resources[i].object_prefab, _load_spawned_resources[i].maze_position, _load_spawned_resources[i].object_name);
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
            manager.map.GetComponent<Maze>().spawned_items.Add(new KeyValuePair<int, Vector3>(_item_id, _drop_position));
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
