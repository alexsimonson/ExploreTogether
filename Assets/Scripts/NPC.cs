using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour{
    public new string name;
    public Type type; // string for now, this should probably be an Enum eventually
    public enum Type{
        Zombie,
        Guard,
        Citizen
    }

    // right now all NPCs are the same model, so let's use that to our advantage
    public Transform[] skins = new Transform[2];
    private float timer;
    // Start is called before the first frame updateNavMeshAgent agent;
    UnityEngine.AI.NavMeshAgent agent;
    // public Animator animator;

    static int segments = 20;
    static int fov = 90;
    static float fovHalf = fov / 2;
    float increment = fovHalf/segments;
    float angle;

    private int playerLayer = 1 << 6;

    public State state;
    public enum State{
        Wander,
        Chase,
        Attack,
        Defend,
        Flee,
        Knockback
    }

    private GameObject chaseTarget = null;
    RaycastHit[] hits = new RaycastHit[segments * 2 + 1];

    private bool isDead = false;
    private bool weaponEquipped = false;
    public GameObject rightHand;
    public GameObject weaponSlot1;
    public AnimationClip equipAC;
    public AnimationClip attackSlashAC;
    public float attackSpeed = 2.4f;
    public int attackDamage = 35;

    private bool isAttacking = false;
    private bool isFleeing = false;

    private bool debugMode = false;

    private int zombie_attack_distance = 2;
    public float default_movement_speed = 3.5f;
    Vector3 knockback_vector;
    void Start(){
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        // // animator = gameObject.transform.GetChild(0).GetComponent<Animator>();
        // let's obtain the xbot

        Material[] typeMaterial = new Material[1];
        // setup stuff dependant upon type
        if(type==Type.Zombie){
            typeMaterial[0] = Resources.Load("BasicMaterials/GreenMaterial", typeof(Material)) as Material;
            state = State.Wander;
        }else if(type==Type.Citizen){
            typeMaterial[0] = Resources.Load("BasicMaterials/YellowMaterial", typeof(Material)) as Material;
            state = State.Wander;
        }else if(type==Type.Guard){
            typeMaterial[0] = Resources.Load("BasicMaterials/BlueMaterial", typeof(Material)) as Material;
            state = State.Defend;
        }else{
            if(debugMode) Debug.LogError("No AI type set");
        }
        if(typeMaterial!=null){
            gameObject.GetComponent<MeshRenderer>().material = typeMaterial[0];
            // foreach(Transform skin in skins){
            //     skin.GetComponent<SkinnedMeshRenderer>().materials = typeMaterial;
            // }
        }
    }

    void Update(){
        if(!isDead){
            timer += Time.deltaTime;
            AnimateMovement();
            FOVScan();  // always watching
            // DrawFOV();
            if(debugMode) Debug.Log("Current state: " + state.ToString());
            if(type==Type.Zombie){
                // we should be looking for nearby prey, otherwise wandering around
                if(chaseTarget==null || state==State.Wander){
                    ZombieWander();
                }else if(chaseTarget && state==State.Chase){
                    ZombieChase();
                }else if(chaseTarget && state==State.Attack){
                    if(!isAttacking){
                        StartCoroutine(ZombieAttack());
                    }
                }
            }else if(type==Type.Citizen){
                // we should be wandering around, unless we feel threatened, then we should seek help
                if(CheckThreats()){
                    state = State.Flee;
                }else{
                    state = State.Wander;
                }

                // if there are no threats, we should check if we have any tasks before falling back on "default" behavior
                if(state==State.Flee){
                    // run away from the zombie
                    if(debugMode) // Debug.Log("Citizen should be fleeing");
                    if(!isFleeing){
                        StartCoroutine(Flee());
                    }
                }else{
                    if(debugMode) Debug.Log("COUNT OF TASKS: " + GetComponent<Brain>().tasks.Count);
                    if(GetComponent<Brain>().tasks.Count > 0){
                        // we should accomplish these tasks before default behavior
                        AccomplishTask(GetComponent<Brain>().tasks);
                    }else{
                        if(debugMode) Debug.Log("NO tasks to accomplish");
                        if(state==State.Wander){
                            Wander();
                        }
                    }
                }
            }else if(type==Type.Guard){
                // we should patrol or guard certain areas, unless our help is needed
                if(state==State.Defend){
                    GuardDefend();
                }
                // equip and unequip weapon
                
            }
            if(state==State.Knockback){
                StartCoroutine(IKnockback());
            }
        }
    }

    void AccomplishTask(List<Task> tasks){
        Task task = tasks[0];
        if(debugMode) Debug.Log("Taking steps to accomplish task: " + task.title);
    }

    // we can call this script on any NPC and it will 
    public void Interaction(){
        if(debugMode) Debug.Log("Some sort of basic interaction");
    }

    void Wander(){
        // this should be modified to check NPC Vitals
        if(timer >= 5f && agent.velocity == Vector3.zero){
            Vector3 newPos = RandomNavSphere(transform.position, 100f, -1);
            if(agent.enabled) agent.destination = newPos;
            timer = 0;
        }
    }

    IEnumerator Flee(){
        isFleeing = true;
        agent.destination = -Vector3.forward;
        yield return new WaitForSeconds(5);
        isFleeing = false;
    }

    void GuardDefend(){
        if(!weaponEquipped){
            EquipWeapon();
            weaponEquipped = true;
        }
        // we should detect if the chaseTarget is within our field of view...
        GuardCheckThreat();
        if(chaseTarget!=null){
            // movement speed should be higher here
            // check if within attack range...
            if(InAttackRange("guard")){
                if(!isAttacking){
                    StartCoroutine(GuardAttack());
                }
            }else{
                // move towards nearest threat
                if(debugMode) // Debug.Log("Should move closer to the threat????");
                agent.destination = chaseTarget.transform.position;
            }
        }else{
            Wander();
        }
    }

    void GuardCheckThreat(){
        List<GameObject> potentialVictims = new List<GameObject>();
        foreach(RaycastHit rcHit in hits){
            if(rcHit.transform!=null){
                if(rcHit.transform.tag=="NPC" && rcHit.transform.gameObject.GetComponent<NPC>().type==Type.Zombie){
                    // instead of setting the chase target here, let's find the closest one
                    potentialVictims.Add(rcHit.transform.gameObject);
                }
            }
        }

        // let's sort through the potentialVictims and find the closest one
        chaseTarget = GetClosest(potentialVictims);
    }

    // refactored version of GuardCheckThreat
    bool CheckThreats(){
        foreach(RaycastHit rcHit in hits){
            if(rcHit.transform!=null){
                if(rcHit.transform.tag=="NPC" && rcHit.transform.gameObject.GetComponent<NPC>().type==Type.Zombie){
                    // if there's a threat, we should run
                    return true;
                }
            }
        }
        return false;
    }

    void EquipWeapon(){
        Transform weapon1 = weaponSlot1.transform.GetChild(0);
        if(weapon1 != null){
            // we should remove the constraints from the Rigidbody
            weapon1.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            weaponEquipped = true;
            weapon1.SetParent(rightHand.transform);
            weapon1.transform.localPosition = new Vector3(0.1793f, .0058f, .2256f);
            weapon1.transform.localRotation = Quaternion.Euler(115.383f, -81.457f, -19.586f);
            weapon1.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            // animator.Play("EquipWeapon");d
        }
    }

    void UnequipWeapon(){
        // we should put this away
        Transform weapon1 = rightHand.transform.GetChild(5);
        // animator.Play("UnequipWeapon");
        StartCoroutine(MoveWeaponToSlot(weapon1));
        weaponEquipped = false;
    }

    private IEnumerator MoveWeaponToSlot(Transform weapon1){
        yield return new WaitForSeconds(equipAC.length - .3f);  // the animation has an odd ending, so items should move .3 seconds before it ends
        weapon1.SetParent(weaponSlot1.transform);
        weapon1.transform.localPosition = new Vector3(0.01155472f, 0.200635f, -0.1219239f);
        weapon1.transform.localRotation = Quaternion.Euler(137.027f, -409.164f, 33.53999f);
        weapon1.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    void ZombieWander(){
        Wander();
        // we need to do some sort of raycast scan out
        ZombieFOVDetectVictim();
    }

    void ZombieChase(){
        // we should detect if the chaseTarget is within our field of view...
        ZombieFOVUpdateChase();
        if(chaseTarget!=null){
            // check if within attack range...
            if(InAttackRange("zombie")){
                state = State.Attack;
            }else{
                if(agent.enabled){
                    agent.destination = chaseTarget.transform.position;
                }
            }
        }else{
            state = State.Wander;
        }
    }

    private IEnumerator ZombieAttack(){
        // instead of just dealing damage like an overpowered cunt we should actually do a raycast
        RaycastHit hit;

        int layerMask = 1 << 6;
        isAttacking = true;
        if(Physics.Raycast(gameObject.transform.position, gameObject.transform.forward * 1, out hit, 2, layerMask)){
            Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * hit.distance, Color.red, 2.0f, false);
            hit.transform.gameObject.GetComponent<Health>().DealDamage(5);
        }else{
            Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * 1, Color.green, 2.0f, false);
        }
        // animator.Play("AttackSlash");
        // chaseTarget.GetComponent<Health>().DealDamage(attackDamage);
        // yield return new WaitForSeconds(attackSlashAC.length);
        yield return new WaitForSeconds(attackSpeed);
        isAttacking = false;
        state = State.Chase;
    }

    private IEnumerator GuardAttack(){
        isAttacking = true;
        // animator.Play("AttackSlash");
        chaseTarget.GetComponent<Health>().DealDamage(100);
        //  the target should be null... because it's dead you know?
        yield return new WaitForSeconds(attackSlashAC.length);
        isAttacking = false;
        chaseTarget = null;
    }



    void AnimateMovement(){
        if(agent.velocity != Vector3.zero){
            // animator.SetFloat("InputY", 1);
        }else{
            // animator.SetFloat("InputY", 0);
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) {
        Vector3 randDirection = Random.insideUnitSphere * dist;
 
        randDirection += origin;
 
        NavMeshHit navHit;
 
        NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);
 
        return navHit.position;
    }

    void DrawFOV(){
        angle = increment;
        // Draw the forward ray
        Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * 30, Color.yellow, 0f);
        // let's draw both sides of the ray
        for(var i=0;i<segments;i++){
            Debug.DrawRay(gameObject.transform.position, Quaternion.Euler(0, angle, 0) * gameObject.transform.forward * 30, Color.yellow, 0f);
            Debug.DrawRay(gameObject.transform.position, Quaternion.Euler(0, -angle, 0) * gameObject.transform.forward * 30, Color.yellow, 0f);
            angle += increment;
        }
    }

    void ZombieFOVDetectVictim(){
        List<GameObject> potentialVictims = new List<GameObject>();
        foreach(RaycastHit rcHit in hits){
            if(rcHit.transform!=null){
                if(rcHit.transform.tag=="NPC" && rcHit.transform.gameObject.GetComponent<NPC>().type!=Type.Zombie && state!=State.Chase){
                    state = State.Chase;
                    // instead of setting the chase target here, let's find the closest one
                    potentialVictims.Add(rcHit.transform.gameObject);
                    // chaseTarget = rcHit.transform.gameObject;
                }
                if(rcHit.transform.gameObject.layer==6 && state!=State.Chase){
                    state = State.Chase;
                    potentialVictims.Add(rcHit.transform.gameObject);
                }
            }
        }

        if(state==State.Chase){
            // let's sort through the potentialVictims and find the closest one
            chaseTarget = GetClosest(potentialVictims);
        }
    }

    void CitizenFOVStateLogic(){
        // depending on what we see, this will determine states

    }

    void ZombieFOVUpdateChase(){
        List<GameObject> potentialVictims = new List<GameObject>();
        foreach(RaycastHit rcHit in hits){
            if(rcHit.transform!=null){
                if(rcHit.transform.tag=="NPC" && rcHit.transform.gameObject.GetComponent<NPC>().type!=Type.Zombie){
                    // instead of setting the chase target here, let's find the closest one
                    potentialVictims.Add(rcHit.transform.gameObject);
                    // chaseTarget = rcHit.transform.gameObject;
                }
                if(rcHit.transform.gameObject.layer==6){
                    potentialVictims.Add(rcHit.transform.gameObject);
                }
            }
        }

        chaseTarget = RelocateChaseTarget(potentialVictims);
    }

    GameObject GetClosest(List<GameObject> potentialVictims){
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        GameObject closest = null;
        foreach (GameObject victim in potentialVictims){
            float dist = Vector3.Distance(victim.transform.position, currentPos);
            if (dist < minDist){
                if(debugMode) Debug.Log("Potential victim is closest: " + victim.name);
                closest = victim;
                minDist = dist;
            }
        }
        return closest;
    }
    
    GameObject RelocateChaseTarget(List<GameObject> potentialVictims){
        Vector3 currentPos = transform.position;
        bool targetStillAvailable = false;
        foreach (GameObject victim in potentialVictims){
            if(GameObject.ReferenceEquals(chaseTarget, victim)){
                // ensure victim is not dead
                targetStillAvailable = true;
            }
        }
        if(targetStillAvailable){
            return chaseTarget;
        }else{
            return GetClosest(potentialVictims);
        }
    }

    bool InAttackRange(string which){
        // check if the gameObject is within attackrange of the chase target
        float distance = Vector3.Distance(gameObject.transform.position, chaseTarget.transform.position);
        if(distance < zombie_attack_distance){
            if(debugMode) Debug.Log(which + " should be attacking because threat is close enough");
            return true;
        }else{
            if(debugMode) Debug.Log(which + " must get closer - (distance: " + distance + ")");
            return false;
        }
    }

    void FOVScan(){
        angle = increment;
        RaycastHit hit;
        for(var i=0;i<segments;i++){
            Physics.Raycast(gameObject.transform.position, Quaternion.Euler(0, angle, 0) * gameObject.transform.forward * 30, out hit, 30, playerLayer);
            hits[i] = hit;
            Physics.Raycast(gameObject.transform.position, Quaternion.Euler(0, -angle, 0) * gameObject.transform.forward * 30, out hit, 30, playerLayer);
            hits[i + segments-1] = hit;
            angle += increment;
        }
        Physics.Raycast(gameObject.transform.position, gameObject.transform.forward * 30, out hit, 30, playerLayer);
        hits[segments * 2] = hit;
    }

    public bool GetIsDead(){
        return isDead;
    }

    public void SetIsDead(bool status){
        isDead = status;
    }

    public void SetAgentDestination(Vector3 dest){
        agent.destination = dest;
    }

    public void Knockback(Vector3 direction){
        knockback_vector = direction;
        state = State.Knockback;
    }

    IEnumerator IKnockback(){
        gameObject.GetComponent<NavMeshAgent>().enabled = false;
        gameObject.GetComponent<Rigidbody>().AddForce(knockback_vector, ForceMode.Impulse);
        yield return new WaitForSeconds(.2f);
        gameObject.GetComponent<NavMeshAgent>().enabled = true;
        state = State.Wander;
    }
}
