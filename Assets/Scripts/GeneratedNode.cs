using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExploreTogether {
    public class GeneratedNode : ScriptableObject {
        public Vector3 mazePosition;
        public int index;
        public GeneratedNode northNeighbor = null;
        public GeneratedNode southNeighbor = null;
        public GeneratedNode eastNeighbor = null;
        public GeneratedNode westNeighbor = null;

        public void Init(Vector3 _mazePosition, int _index){
            mazePosition = _mazePosition;
            index = _index;
        }
    }
}
