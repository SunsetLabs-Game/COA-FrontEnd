using UnityEngine;

/// <summary>
/// Handles the NPC Attack Reaction
/// </summary>
public class NPCEnemyInteraction : MonoBehaviour
{
    /// <summary> Transform of the object the NPC is locked on to</summary>
    [SerializeField] private Transform target;

    /// <summary> Range in which the NPC can attack the player</summary>
    [SerializeField] private float attackRange = 10f;

    /// <summary> Range in which the NPC can track and follow the player</summary>
    [SerializeField] private float trackingRange = 20f;

    /// <summary> The sphere collider used as a detection trigger </summary>
    private GameObject detectTrigger; 

    void Start()
    {
        detectTrigger = GetComponent<SphereCollider>().gameObject;

        // Set the radius of the trigger to match the detection range
        detectTrigger.GetComponent<SphereCollider>().radius = attackRange / 2;
    }

    void Update()
    {
        if(target != null) //The NPC is currently locked on to a player
        {
            //Player is out of tracking range
            if((target.position - transform.position).magnitude >= trackingRange) 
            {
                target = null;
            }
            else if((target.position - transform.position).magnitude < 10)
            {
                //Play attack animation
            }
            else
            {
                //Keep chasing the player
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) //A Player has entered the NPC's attack range
        {
            target = other.transform;
        }
    }
}
