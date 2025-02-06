using UnityEngine;
using System.Collections.Generic;

namespace ExploreTogether{
    [System.Serializable]
    public class MazeSpawnedObjectSerializable{
        public string object_prefab;    // let this refer to directory of prefab
        public Vector3 maze_position;

        public MazeSpawnedObjectSerializable(string _object_prefab, Vector3 _position){
            if(_object_prefab==null || _position==null){
                return;
            }            
            this.object_prefab = _object_prefab;
            this.maze_position = _position;
        }

        public KeyValuePair<string, Vector3> ToKeyValue(){
            return new KeyValuePair<string, Vector3>(this.object_prefab, this.maze_position);
        }
    }
}
