using UnityEngine;
using System.Collections.Generic;

namespace ExploreTogether{
    [System.Serializable]
    public class MazeSpawnedObjectSerializable{
        public string object_prefab;    // let this refer to directory of prefab
        public Vector3 maze_position;
        public string object_name;

        public MazeSpawnedObjectSerializable(string _object_prefab, Vector3 _position, string _object_name="maze_object_serial"){
            if(_object_prefab==null || _position==null){
                return;
            }            
            this.object_prefab = _object_prefab;
            this.maze_position = _position;
            this.object_name = _object_name;
        }
    }
}
