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
            // // I think this has to be done outside... unless we feed it the dataset
            // if(this.northNeighbor!=-1 && this.northNeighbor < generated_nodes.Count){
            //     node.northNeighbor = generated_nodes[this.northNeighbor];
            // }else{
            //     Debug.Log("Prevented an issue with this.northNeighbor: " + this.northNeighbor.ToString() + " ***** could have also been this issue with count: " + generated_nodes.Count.ToString());
            // }
            // if(this.southNeighbor!=-1 && this.southNeighbor < generated_nodes.Count){
            //     node.southNeighbor = generated_nodes[this.southNeighbor];
            // }else{
            //     Debug.Log("Prevented an issue with this.southNeighbor: " + this.southNeighbor.ToString() + " ***** could have also been this issue with count: " + generated_nodes.Count.ToString());
            // }
            // if(this.eastNeighbor!=-1 && this.eastNeighbor < generated_nodes.Count){
            //     node.eastNeighbor = generated_nodes[this.eastNeighbor];
            // }else{
            //     Debug.Log("Prevented an issue with this.eastNeighbor: " + this.eastNeighbor.ToString() + " ***** could have also been this issue with count: " + generated_nodes.Count.ToString());
            // }
            // if(this.westNeighbor!=-1 && this.westNeighbor < generated_nodes.Count){
            //     node.westNeighbor = generated_nodes[this.westNeighbor];
            // }else{
            //     Debug.Log("Prevented an issue with this.westNeighbor: " + this.westNeighbor.ToString() + " ***** could have also been this issue with count: " + generated_nodes.Count.ToString());
            // }
            return node;
        }
    }
}
