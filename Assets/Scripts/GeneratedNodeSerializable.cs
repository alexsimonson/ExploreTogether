using UnityEngine;
using System.Collections.Generic;

namespace ExploreTogether{
    [System.Serializable]
    public class GeneratedNodeSerializable{
        public Vector3 mazePosition;
        public int index;
        public GeneratedNode northNeighbor;
        public GeneratedNode southNeighbor;
        public GeneratedNode eastNeighbor;
        public GeneratedNode westNeighbor;

        public GeneratedNodeSerializable(GeneratedNode node){
            mazePosition = node.mazePosition;
            index = node.index;
            northNeighbor = node.northNeighbor;
            southNeighbor = node.southNeighbor;
            eastNeighbor = node.eastNeighbor;
            westNeighbor = node.westNeighbor;
        }
    }
}
