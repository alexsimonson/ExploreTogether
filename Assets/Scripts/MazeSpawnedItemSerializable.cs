using UnityEngine;
using System.Collections.Generic;

namespace ExploreTogether{
    [System.Serializable]
    public class MazeSpawnedItemSerializable{
        public int item_id;
        public Vector3 maze_position;

        public MazeSpawnedItemSerializable(int _item_id, Vector3 _position){
            if(_item_id==null || _position==null){
                return;
            }            
            this.item_id = _item_id;
            this.maze_position = _position;
        }

        public KeyValuePair<int, Vector3> ToKeyValue(){
            return new KeyValuePair<int, Vector3>(this.item_id, this.maze_position);
        }
    }
}
