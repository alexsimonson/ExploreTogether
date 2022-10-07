using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationBuilder : MonoBehaviour {
    
    public void BuildNavigation(List<NavMeshSurface> surfaces){
        foreach(NavMeshSurface surface in surfaces){
            surface.BuildNavMesh();
        }
    }


}
