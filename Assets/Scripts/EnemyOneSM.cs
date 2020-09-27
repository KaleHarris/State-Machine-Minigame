using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyOneSM : MonoBehaviour
{

    //define agent for NavMeash
    public NavMeshAgent agentOne;
    //For finding player
    public Transform player;
    //create Layermasks
    public LayerMask platformLayer, playerLayer;

    //Scan
    public Vector3 Scanning;
    bool SetScan;
    public float scanningRange;

    //Attack
    public float attackTimer;
    bool Attacked;

    //States
    public float inSight, attackRange;
    public bool playerInRange, playerInAttackRange;


    private void Awake()
    {
        player = GameObject.Find("VRRig").transform;
        agentOne = GetComponent<NavMeshAgent>();
    }
    
    // Update is called once per frame
    private void Update()
    {
        //check to see if player is in sight and in attack range
        playerInRange = Physics.CheckSphere(transform.position, inSight, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        //when the player is not in attack range or the chase range run the Scan function
        if (!playerInRange && !playerInAttackRange) Scan();
        //when the player is in chase range but not in attack range run the Chase function
        if (playerInRange && !playerInAttackRange) Chase();
        //when the player is in range and close enough to attack run the Attack function
        if (playerInRange && playerInAttackRange) Attack();

    }
    //Function for finding the player
    private void Scan()
    {
        //Debug.Log("I Am Scanning");

        if (!SetScan) ScanningArea();

        if (SetScan)
            agentOne.SetDestination(Scanning);

        Vector3 distanceToDestination = transform.position - Scanning;

        if (distanceToDestination.magnitude < 1f)
            SetScan = false;
    }
    
    private void ScanningArea()
    {
        float RandomZ = Random.Range(-scanningRange, scanningRange);
        float RandomX = Random.Range(-scanningRange, scanningRange);

        Scanning = new Vector3(transform.position.x + RandomX, transform.position.y, transform.position.z + RandomZ);

        if (Physics.Raycast(Scanning, -transform.up, 2f, platformLayer))
            SetScan = true;
    }
    //Function for Chasing the player
    private void Chase()
    {
        //Debug.Log("I Am Chasing");
        agentOne.SetDestination(player.position);
    }
    //Function for attacking the player
    private void Attack()
    {
        float radius = 2.0f;
        Vector3 movePos = player.transform.position;
        movePos = Vector3.MoveTowards(movePos, transform.position, radius);

        //Debug.Log("I Am Attacking");
        agentOne.SetDestination(movePos);

        transform.LookAt(player);

        if (!Attacked)
        {
            // NEED TO ADD SOME FORM OF ATTACK HERE

            Attacked = true;
            Invoke(nameof(ResetAttack), attackTimer);
        }
    }

    private void ResetAttack()
    {
        Attacked = false;
    }
}
