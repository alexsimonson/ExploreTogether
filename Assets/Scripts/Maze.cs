using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour {

    public GameObject corner;
    public GameObject hall;
    public GameObject tJunc;
    public GameObject xJunc;
    public GameObject cJunc;


    private int size = 10;
    private int prefabSize = 5;

    private List<Vector3> neighbors_visited = new List<Vector3>();
    private List<Vector3> generating_stack = new List<Vector3>();

    public List<GeneratedNode> generated_nodes = new List<GeneratedNode>();

    public int max_dungeon_length = 1000;
    public int total_count = 0;

    void Awake(){
        corner = Resources.Load("Prefabs/Dungeon/Corner", typeof(GameObject)) as GameObject;
        hall = Resources.Load("Prefabs/Dungeon/Hall", typeof(GameObject)) as GameObject;
        tJunc = Resources.Load("Prefabs/Dungeon/T-Junction", typeof(GameObject)) as GameObject;
        xJunc = Resources.Load("Prefabs/Dungeon/X-Junction", typeof(GameObject)) as GameObject;
        cJunc = Resources.Load("Prefabs/Dungeon/C-Junction", typeof(GameObject)) as GameObject;
    }

    // Start is called before the first frame update
    void Start(){
        GenerateRandomMaze();
    }

    void GenerateRandomMaze(){
        // given starting point 0, 0, find empty neighbors
        Vector3 start_pos = new Vector3(0, 0, 0);
        GridRecursiveBacktrackGenerator(start_pos);
        RenderGrid();        
    }

    // parameters must define start/end conditions of this function
    // void RecursiveBacktrackGeneratorTest(Vector3 position){
    //     // given a position we must do the stuff
    //     // Instantiate(xJunc, position, Quaternion.identity);
    //     neighbors_visited.Add(position);
    //     generating_stack.Add(position);
    //     InstantiateModified(position);
    //     List<Vector3> neighbors = FindNeighbors(position);
    //     List<Vector3> empty_neighbors = FindEmptyNeighbors(neighbors);
    //     if(empty_neighbors.Count <= 0 || neighbors_visited.Count==max_dungeon_length) return;
    //     // otherwise we will choose a random neighbor and go
    //     Vector3 random_neighbor = empty_neighbors[Random.Range(0, empty_neighbors.Count)];
    //     RecursiveBacktrackGeneratorTest(random_neighbor);
    // }

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
    void InstantiateModified(Vector3 position, GameObject prefab, int rotation_degrees=0){
        Vector3 modified_position = new Vector3(position.x * prefabSize, position.y, position.z * prefabSize);
        GameObject io = Instantiate(prefab, modified_position, Quaternion.identity);
        Vector3 existing_rot = io.transform.rotation.eulerAngles;
        existing_rot.y = rotation_degrees;
        io.transform.rotation = Quaternion.Euler(existing_rot);
    }

    // parameters must define start/end conditions of this function
    void GridRecursiveBacktrackGenerator(Vector3 position){
        GeneratedNode last_node = (total_count>0) ? generated_nodes[total_count-1] : null;
        // given a position we must do the stuff
        // Instantiate(xJunc, position, Quaternion.identity);   // commented out while I redesign this logic
        neighbors_visited.Add(position);
        generating_stack.Add(position);
        // we should also create a new GeneratedNode
        GeneratedNode current_node = new GeneratedNode(position);
        // InstantiateModified(position);   // commented out while I redesign this logic
        // this functionality must be called before we return
        if(last_node!=null){
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
        generated_nodes.Add(current_node);
        List<Vector3> empty_neighbors = FindEmptyNeighbors(FindNeighborsGrid(position));
        if(empty_neighbors.Count <= 0 || total_count==max_dungeon_length) return;
        // otherwise we will choose a random neighbor and go
        Vector3 random_neighbor = empty_neighbors[Random.Range(0, empty_neighbors.Count)];

        total_count++;
        GridRecursiveBacktrackGenerator(random_neighbor);
    }

    // this will limit the function to only finding coordinates in a grid
    List<Vector3> FindNeighborsGrid(Vector3 position){
        // given a position, return a list of the neighbors
        var x = position.x;
        var y = position.y;
        var z = position.z;

        List<Vector3> neighbors = new List<Vector3>();
        neighbors.Add(new Vector3(x+1, y, z));
        if(x-1 >= 0) neighbors.Add(new Vector3(x-1, y, z));
        neighbors.Add(new Vector3(x, y, z+1));
        if(y-1 >= 0) neighbors.Add(new Vector3(x, y, z-1));
        return neighbors;
    }

    void RenderGrid(){
        // given a "linked list" path of nodes, instantiate, and rotate accordingly the prefab into position
        // rotating may be very fucked and annoying hopefully not though :)
        foreach(GeneratedNode node in generated_nodes){
            // check out the node's neighbors, then determine the piece
            DetermineAndInstantiateNodePiece(node);
        }
    }

    void DetermineAndInstantiateNodePiece(GeneratedNode node){
        Debug.Log(node);
        // based on certain path parameters of a given node, determine which prefab will render

        int neighbor_count = 0;
        if(node.northNeighbor){
            Debug.Log("This if is sufficient for north");
            neighbor_count++;
            Debug.Log("New count after increasing: " + neighbor_count.ToString());
        }
        if(node.southNeighbor){
            Debug.Log("This if is sufficient for south");
            neighbor_count++;
            Debug.Log("New count after increasing: " + neighbor_count.ToString());
        }
        if(node.eastNeighbor){
            Debug.Log("This if is sufficient for east");
            neighbor_count++;
            Debug.Log("New count after increasing: " + neighbor_count.ToString());
        }
        if(node.westNeighbor){
            Debug.Log("This if is sufficient for west");
            neighbor_count++;
            Debug.Log("New count after increasing: " + neighbor_count.ToString());
        }
        if(neighbor_count==0){
            Debug.Log("Neighbor count must be greater than 0.  Did you run this function before generating the grid?");
            return;
        }

        GameObject node_prefab = null;
        int rotation_degrees = 0;
        // triage based on count
        if(neighbor_count==4){
            node_prefab = xJunc;
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
        InstantiateModified(node.mazePosition, node_prefab, rotation_degrees);
    }
}
