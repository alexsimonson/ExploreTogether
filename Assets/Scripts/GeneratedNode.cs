using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedNode : ScriptableObject {
    public Vector3 mazePosition;
    public GeneratedNode northNeighbor = null;
    public GeneratedNode southNeighbor = null;
    public GeneratedNode eastNeighbor = null;
    public GeneratedNode westNeighbor = null;

    public GeneratedNode(Vector3 _mazePosition){
        mazePosition = _mazePosition;
    }
}
