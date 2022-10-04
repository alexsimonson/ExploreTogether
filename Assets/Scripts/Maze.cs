using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour {

    public GameObject corner;
    public GameObject hall;
    public GameObject tJunc;
    public GameObject xJunc;


    private int size = 10;
    private int prefabSize = 5;

    private List<Vector3> neighbors_visited = new List<Vector3>();
    private List<Vector3> generating_stack = new List<Vector3>();

    public int max_dungeon_length = 100;

    // Start is called before the first frame update
    void Start(){
        GenerateRandomMaze();
    }

    // Update is called once per frame
    void Update(){
        
    }

    void GenerateRandomMaze(){
        // given starting point 0, 0, find empty neighbors
        Vector3 start_pos = new Vector3(0, 0, 0);
        RecursiveBacktrackGenerator(start_pos);
    }

    // parameters must define start/end conditions of this function
    void RecursiveBacktrackGenerator(Vector3 position){
        // given a position we must do the stuff
        // Instantiate(xJunc, position, Quaternion.identity);
        neighbors_visited.Add(position);
        generating_stack.Add(position);
        InstantiateModified(position);
        List<Vector3> neighbors = FindNeighbors(position);
        List<Vector3> empty_neighbors = FindEmptyNeighbors(neighbors);
        if(empty_neighbors.Count <= 0 || neighbors_visited.Count==max_dungeon_length) return;
        // otherwise we will choose a random neighbor and go
        Vector3 random_neighbor = empty_neighbors[Random.Range(0, empty_neighbors.Count)];
        RecursiveBacktrackGenerator(random_neighbor);
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
    void InstantiateModified(Vector3 position){
        Vector3 modified_position = new Vector3(position.x * prefabSize, position.y, position.z * prefabSize);
        Instantiate(xJunc, modified_position, Quaternion.identity);
    }
}
