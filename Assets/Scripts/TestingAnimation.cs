using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingAnimation : MonoBehaviour {
    Animation zombie_animation;
    
    void Start(){
        zombie_animation = gameObject.GetComponent<Animation>();
    }

    void Update(){
        if (Input.GetKey(KeyCode.H)){
            zombie_animation.Play("Attack");
        }
    }
}
