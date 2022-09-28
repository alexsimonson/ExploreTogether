using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhostAim : MonoBehaviour {

    public GameObject portal;
    public Material red_material;
	public GameObject fpsCam;
    public GameObject placedGhostAim = null;

    GameObject raycastObject;   //main camera
    GameObject ghostAim;   
    Vector3 pos;
    RaycastHit hit;
    bool portalOut = false;

    // Use this for initialization
    void Start () {
        raycastObject = fpsCam;
	}
	
	// Update is called once per frame
	void FixedUpdate () {                
        //this raycasts where the player is looking using camera
        //Debug.DrawLine(raycastObject.transform.position, raycastObject.transform.forward * 50, Color.green);
        if(!portalOut){
            CleanupGhostAim();
        }else{
            //check if looking at a place where you can place portal; also check if there is a ghost portal already instantiated and that portal gun is out
            DrawGhostAim();
        }
    }

    void Update(){
        PlayerInput();
    }

    void PlayerInput(){
        //handle taking portal out/putting away
        //this could use a visual representation
        if (Input.GetKeyDown("z")){
            portalOut = !portalOut;
            if(portalOut) Debug.Log("portal equipped");
            if(!portalOut) Debug.Log("portal put away");
        }
        //place portal if mouse clicked and portal out
        if ((Input.GetKeyDown(KeyCode.Mouse0)) && (portalOut == true)){
            //Debug.Log("Placing portal at " + pos);
            CleanupPlacedGhostAim();
            PlacePortal();
            StartCoroutine(PlacedGhostAimDespawn());
        }
    }

    IEnumerator PlacedGhostAimDespawn(){
        yield return new WaitForSeconds(5);
        CleanupPlacedGhostAim();
    }

    void CleanupGhostAim(){
        if(ghostAim) Destroy(ghostAim);
    }

    void CleanupPlacedGhostAim(){
        if(placedGhostAim) Destroy(placedGhostAim);
    }

    void DrawGhostAim(){
        // int layerMask = ~(1 << 10); // bit shift layer 10 (ghostPortal) with ~ means we collide against everything except the ghostPortal itself.  Prevents portal moving towards camera on repeat
        int layerMask = 1 << 3;
        if(Physics.Raycast(raycastObject.transform.position, raycastObject.transform.forward, out hit, 10, layerMask)){
            pos = hit.point;
            CleanupGhostAim();
            ghostAim = Instantiate(portal, pos, gameObject.transform.rotation);   // why am I instantiating every frame, rather than instantiating once and moving?
            ghostAim.transform.parent = gameObject.transform;
        }
    }

    void PlacePortal(){
        placedGhostAim = Instantiate(portal, pos, gameObject.transform.rotation);
        placedGhostAim.gameObject.GetComponent<MeshRenderer>().material = red_material;
        placedGhostAim.gameObject.GetComponent<AreaOfEffect>().areaToEffect = true;
    }
}
