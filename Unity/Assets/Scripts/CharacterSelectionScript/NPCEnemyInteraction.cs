using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// Handles the NPC Attack Reaction
/// </summary>
public class NPCEnemyInteraction : MonoBehaviour
{
    // Placeholder functions for Animation events.
    [HideInInspector] public UnityEvent OnFootR = new UnityEvent();
    [HideInInspector] public UnityEvent OnFootL = new UnityEvent();
    [HideInInspector] public UnityEvent OnStrike = new UnityEvent();

    public EnemyNPCState state;

    private Animator animator;
    private NavMeshAgent agent;
    private SphereCollider detectTrigger;

    [Tooltip("Transform of the object the NPC is locked on to (The Player)")]
    [SerializeField] private Transform target;

    [Tooltip("Range in which the NPC can detect the player")]
    [SerializeField] private float detectionRange = 10f;

    [Tooltip("Range in which the NPC can attack the player")]
    [SerializeField] private float attackRange = 5f;

    [Tooltip("Range in which the NPC can track and follow the player after detection")]
    [SerializeField] private float trackingRange = 20f;

    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float walkSpeed = 2f;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        detectTrigger = GetComponent<SphereCollider>();

        agent.speed = walkSpeed;
        state = EnemyNPCState.idle;
        detectTrigger.radius = detectionRange / 2; // Set the radius of the trigger to match the detection range
    }

    void Update()
    {
        if(target != null) //The NPC is currently locked on to a player
        {
            if (Vector3.Distance(target.position, transform.position) >= trackingRange) //Player is out of tracking range
            {
                target = null;
                state = EnemyNPCState.idle;
                animator.SetBool("Walk", false);
                animator.SetBool("Run", false);
            }
            else if (Vector3.Distance(target.position, transform.position) <= attackRange) //Player is within attacking range
            {
                state = EnemyNPCState.attacking;
                transform.LookAt(target);
                animator.SetTrigger("Attack");
            }
            else //Keep chasing the player
            {
                agent.SetDestination(target.position);

                if(Vector3.Distance(target.position, transform.position) > (trackingRange -attackRange)/2)
                {
                    agent.speed = runSpeed;
                    animator.SetBool("Run", true);
                    state = EnemyNPCState.running;
                }
                else
                {
                    agent.speed = walkSpeed;
                    animator.SetBool("Walk", true);
                    state = EnemyNPCState.walking;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) //A Player has entered the NPC's attack range
        {
            target = other.transform;
            agent.SetDestination(target.position);
        }
    }

    #region AnimationEventHandlers
    public void FootR()
    {
        OnFootR.Invoke();
    }

    public void FootL()
    {
        OnFootL.Invoke();
    }

    public void Strike()
    {
        OnStrike.Invoke();
    }
    #endregion
}

public enum EnemyNPCState : int
{
    idle, walking, running, attacking
}

