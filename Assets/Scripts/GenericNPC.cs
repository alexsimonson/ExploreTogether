using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GenericNPC : MonoBehaviour
{

    Animation npcAnimation;
    
    public State state;
    public enum State
    {
        Wander,
        Chase,
        Attack,
        Defend,
        Flee,
        Knockback,
        Frozen
    }

    // Start is called before the first frame updateNavMeshAgent agent;
    UnityEngine.AI.NavMeshAgent agent;

    private VisionSystem visionSystem; // Reference to the VisionSystem script

    private Transform playerTransform; // Reference to the player's transform
    public float chaseSpeed = 5f; // Speed at which the NPC chases the player
    public float chaseDistance = 10f; // Maximum distance at which the NPC can chase the player
    public float actionDistance = 2f; // Distance at which the NPC can take action

    private bool isAttacking = false;

    private void Start()
    {
        npcAnimation = GetComponentInChildren<Animation>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        // Get the VisionSystem component from the GameObject
        visionSystem = GetComponent<VisionSystem>();
        // Get the player's transform using any desired method, e.g., by finding the player GameObject with a specific tag
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // Check if the player is detected by the VisionSystem
        if (visionSystem.playerDetected)
        {
            // Player detected, change the NPC state to Chase
            state = State.Chase;
        }
        else
        {
            // Player not detected, change the NPC state to Wander (or any other desired default state)
            state = State.Wander;
        }
        switch (state)
        {
            case State.Wander:
                WanderState();
                break;
            case State.Chase:
                ChaseState();
                break;
            case State.Attack:
                AttackState();
                break;
            case State.Defend:
                DefendState();
                break;
            case State.Flee:
                FleeState();
                break;
            case State.Knockback:
                KnockbackState();
                break;
            case State.Frozen:
                FrozenState();
                break;
        }
    }

    private void WanderState()
    {
        if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance)
        {
            // Generate a random point on the NavMesh within a specified range
            Vector3 randomPoint = RandomNavMeshPoint(transform.position, 5f, -1);

            // Set the destination for the NavMeshAgent to the random point
            agent.SetDestination(randomPoint);
        }
    }

    private Vector3 RandomNavMeshPoint(Vector3 origin, float range, int areaMask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * range;
        randomDirection += origin;

        NavMeshHit navMeshHit;
        NavMesh.SamplePosition(randomDirection, out navMeshHit, range, areaMask);

        return navMeshHit.position;
    }


    private void ChaseState()
    {
        // Get the chase target's position from the VisionSystem
        Vector3 chaseTargetPosition = visionSystem.GetChaseTargetPosition();

        // Calculate the distance between the NPC and the chase target
        float distanceToTarget = Vector3.Distance(transform.position, chaseTargetPosition);

        // Check if the chase target is within chase distance
        if (distanceToTarget <= chaseDistance)
        {
            // Rotate towards the chase target's position
            Vector3 direction = (chaseTargetPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            // Check if the chase target is within action distance
            if (distanceToTarget <= actionDistance)
            {
                // Take action against the chase target (e.g., attack)
                state = State.Attack;
            }
            else
            {
                // Move towards the chase target's position
                agent.SetDestination(chaseTargetPosition);
                agent.speed = chaseSpeed;
            }
        }
        else
        {
            // Chase target is out of chase distance, transition to another state (e.g., Wander)
            state = State.Wander;
        }
    }

    private void AttackState()
    {
        // Logic for the Attack state
        if(!isAttacking){
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack(){
        isAttacking = true;
        // animator.Play("AttackSlash");
        visionSystem.GetChaseTargetObject().GetComponent<Health>().DealDamage(100);
        //  the target should be null... because it's dead you know?
        yield return new WaitForSeconds(5); // we need to ensure this is eventually the length of time of attack animation
        isAttacking = false;
    }

    private void DefendState()
    {
        // Logic for the Defend state
    }

    private void FleeState()
    {
        // Logic for the Flee state
    }

    private void KnockbackState()
    {
        // Logic for the Knockback state
    }

    private void FrozenState()
    {
        // Logic for the Frozen state
    }
}
