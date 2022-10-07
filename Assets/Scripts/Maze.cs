using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Maze : MonoBehaviour {

    public GameObject corner;
    public GameObject hall;
    public GameObject tJunc;
    public GameObject xJunc;
    public GameObject cJunc;
    public GameObject itemSpawn;
    public GameObject respawnPrefab;

    private int size = 10;
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

    public Manager manager;

    public List<NavMeshSurface> surfaces = new List<NavMeshSurface>();
    public NavigationBuilder nav_builder;



    void Awake(){
        corner = Resources.Load("Prefabs/Dungeon/Corner", typeof(GameObject)) as GameObject;
        hall = Resources.Load("Prefabs/Dungeon/Hall", typeof(GameObject)) as GameObject;
        tJunc = Resources.Load("Prefabs/Dungeon/T-Junction", typeof(GameObject)) as GameObject;
        xJunc = Resources.Load("Prefabs/Dungeon/X-Junction", typeof(GameObject)) as GameObject;
        cJunc = Resources.Load("Prefabs/Dungeon/C-Junction", typeof(GameObject)) as GameObject;
        itemSpawn = Resources.Load("Prefabs/Item", typeof(GameObject)) as GameObject;
    }

    void Start(){
        nav_builder = gameObject.GetComponent<NavigationBuilder>();
    }

    // Start is called before the first frame update
    void Update(){
        if(controlled_generation){
            controlled_generation = false;
            GenerateRandomMaze();
        }
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
        FillMazeWithItems();
        maze_generated = true;
        // when the maze is generated we should alert the manager if there is one
        if(manager==null) return;
        nav_builder.BuildNavigation(surfaces);
        manager.MazeGenerated();

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
            Debug.Log("Max dungeon length reached");
            return;
        }
        // when there are no empty neighbors, I should pop the stack and "backtrack" to try the last node
        bool control = true;
        while(empty_neighbors.Count <= 0 && control){
            generating_stack.Pop();
            if(generating_stack.Count <= 0){
                control = false;
                Debug.Log("The end of the line");
            }else{
                current_node = generating_stack.Peek();
                empty_neighbors = FindEmptyNeighbors(FindNeighborsGrid(current_node.mazePosition));
            }
        }
        // otherwise we will choose a random neighbor and go
        if(empty_neighbors.Count <=0){
            Debug.Log("Returning no neighbors");
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
        // surfaces.Add(spawnedMazePiece.transform.GetChild(0).GetComponent<NavMeshSurface>());
        surfaces.Add(spawnedMazePiece.GetComponent<NavMeshSurface>());
    }

    void FillMazeWithItems(){
        foreach(GeneratedNode xJunction in xJunctions){
            // we should instantiate an item here
            Instantiate(itemSpawn, new Vector3(xJunction.mazePosition.x * prefabSize, xJunction.mazePosition.y * prefabSize + 1.5f, xJunction.mazePosition.z * prefabSize), Quaternion.identity);
        }

        // let's instantiate 5 monster spawners throughout the maze
        for(int i=0;i<1;i++){
            int random_index = Random.Range(25, generated_nodes.Count - 25);
            Instantiate(respawnPrefab, new Vector3(generated_nodes[random_index].mazePosition.x * prefabSize, generated_nodes[random_index].mazePosition.y * prefabSize + 1.5f, generated_nodes[random_index].mazePosition.z * prefabSize), Quaternion.identity);
        }
    }
}