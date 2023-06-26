using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GenericNPC : MonoBehaviour
{
    private Rigidbody[] ragdollRigidbodies; // Array to store the rigidbodies of the ragdoll bones

    public State state;
    public enum State
    {
        Wander,
        Chase,
        Attack,
        Defend,
        Flee,
        Knockback,
        Frozen,
        Death
    }

    // Start is called before the first frame update
    UnityEngine.AI.NavMeshAgent agent;

    private VisionSystem visionSystem; // Reference to the VisionSystem script

    public float chaseSpeed = 5f; // Speed at which the NPC chases the player
    public float chaseDistance = 10f; // Maximum distance at which the NPC can chase the player
    public float actionDistance = 2f; // Distance at which the NPC can take action

    private bool isAttacking = false;
    Animation npcAnimation;

    private bool deathHandled = false;

    private void Start()
    {
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>(); // Get the rigidbodies of the ragdoll bones
        npcAnimation = GetComponentInChildren<Animation>();

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        // Get the VisionSystem component from the GameObject
        visionSystem = GetComponent<VisionSystem>();

        // Disable the ragdoll rigidbodies initially
        SetRagdollEnabled(false);
    }

    private void Update()
    {
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
            case State.Death:
                if(!deathHandled){
                    DeathState();
                }
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
                SetState(State.Attack);
                isAttacking = true;
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
        if (isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        // animator.Play("AttackSlash");
        GameObject target = visionSystem.GetChaseTargetObject();
        if (target != null)
        {
            target.GetComponent<Health>().DealDamage(100);
        }
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

    private void DeathState()
    {
        deathHandled = true;
        isAttacking = false;
        // Disable the NavMeshAgent component
        agent.enabled = false;

        // Disable the NPC's collider
        GetComponent<Collider>().enabled = false;

        // Enable the ragdoll rigidbodies
        SetRagdollEnabled(true);

        npcAnimation.enabled = false;
    }

    private void SetRagdollEnabled(bool enabled)
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !enabled;
            rb.useGravity = enabled;
        }
    }

    public bool SetState(State newState)
    {
        if (state == State.Death && newState != State.Death)
        {
            return false;   // Cannot change state from Death to something else
        }
        if (state == newState)
        {
            return false;   // ignore multiple updates
        }

        if (isAttacking && newState != State.Death)
        {
            return false;   // we can't change the state when attacking, unless death occurs
        }
        state = newState;
        Debug.Log("New state set: " + newState.ToString());
        return true;
    }

    public void FakeDeathAnimation(RaycastHit hit)
    {
        agent.enabled = false;  // ensure it's disabled
        Debug.Log("FAKE ANIMATION");
        // Get the normal of the collision surface
        Vector3 surfaceNormal = hit.normal;

        // Calculate the rotation angle based on the normal
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);

        // Rotate the GameObject on the X/Z axis
        transform.rotation = Quaternion.Euler(targetRotation.eulerAngles.x, transform.rotation.eulerAngles.y, targetRotation.eulerAngles.z);
        Debug.Log("FAKE ANIMATION ENDS");
    }

    public void ApplyForceToRagdoll(RaycastHit hit, float forceAmount)
    {
        
        // Check if the hit object is part of the ragdoll
        Rigidbody hitRigidbody = hit.collider.attachedRigidbody;
        if (hitRigidbody != null && IsPartOfRagdoll(hitRigidbody))
        {
            // Apply force in the direction of the ray
            Vector3 force = hit.normal * forceAmount;
            hitRigidbody.AddForceAtPosition(force, hit.point, ForceMode.Impulse);
        }
    }

    // Function to check if a Rigidbody is part of the ragdoll
    bool IsPartOfRagdoll(Rigidbody rb)
    {
        // Here, check if the Rigidbody is part of the ragdoll.
        // For example, you could check if it's in the ragdollRigidbodies array.
        return System.Array.IndexOf(ragdollRigidbodies, rb) >= 0;
    }
}
