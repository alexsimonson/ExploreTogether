using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTest : MonoBehaviour{
    public GameObject magPrefab;

    public GameObject mag;

    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        if (Input.GetKeyDown("r")){
            Debug.Log("Reload");
            Reload();
        }
    }

    void Reload(){
        Vector3 magPosition = mag.transform.position;
        GameObject emptyMag = Instantiate(magPrefab, magPosition, Quaternion.identity);
    }
}
