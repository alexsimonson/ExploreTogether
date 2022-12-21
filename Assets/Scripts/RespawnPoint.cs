using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour {
    public bool isPlayerSpawn;

    public GameObject enemy_prefab;

    public Manager manager;

    void Start(){
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        while(manager.game_rules.ShouldSpawnEnemy()){
            SpawnEnemy();
        }
    }

    public void SpawnEnemy(){
        int[] offset = {-1, 1};
        int x = Random.Range(0, 2);
        int z = Random.Range(0, 2);
        Instantiate(enemy_prefab, new Vector3(gameObject.transform.position.x + offset[x], 0, gameObject.transform.position.z + offset[z]), Quaternion.identity);
    }
}
