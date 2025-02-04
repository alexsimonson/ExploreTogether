using UnityEngine;
using System.Collections.Generic;

namespace ExploreTogether{
    [System.Serializable]
    public class MazeSpawnedEnemySerializable{
        public GameObject enemy_prefab;
        public Vector3 maze_position;

        public MazeSpawnedEnemySerializable(GameObject _enemy_prefab, Vector3 _position){
            if(_enemy_prefab==null || _position==null){
                return;
            }            
            this.enemy_prefab = _enemy_prefab;
            this.maze_position = _position;
        }

        public KeyValuePair<GameObject, Vector3> ToKeyValue(){
            return new KeyValuePair<GameObject, Vector3>(this.enemy_prefab, this.maze_position);
        }
    }
}
