using UnityEngine;
using System.Collections.Generic;

namespace ExploreTogether{
    [System.Serializable]
    public class GeneratedNodeSerializable{
        public Vector3 mazePosition;
        public int index;
        public int northNeighbor = -1;
        public int southNeighbor = -1;
        public int eastNeighbor = -1;
        public int westNeighbor = -1;

        public GeneratedNodeSerializable(GeneratedNode node){
            if(node.mazePosition!=null){
                mazePosition = node.mazePosition;
            }
            index = node.index;
            if(node.northNeighbor!=null){
                this.northNeighbor = node.northNeighbor.index;
            }
            if(node.southNeighbor!=null){
                this.southNeighbor = node.southNeighbor.index;
            }
            if(node.eastNeighbor!=null){
                this.eastNeighbor = node.eastNeighbor.index;
            }
            if(node.westNeighbor!=null){
                this.westNeighbor = node.westNeighbor.index;
            }
        }

        public GeneratedNode ToScriptableObject(){
            GeneratedNode node = ScriptableObject.CreateInstance<GeneratedNode>();
            node.mazePosition = this.mazePosition;
            node.index = this.index;
            return node;
        }
    }
}
