using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyOneSM : MonoBehaviour {
    //For finding player
    [HideInInspector] public EnemyManager manager;

    //define agent for NavMeash
    public NavMeshAgent agentOne;

    //Scan
    public float scanningRange;
    public Transform origin;

    //Attack
    public float attackTimer;
    bool Attacked;

    //States
    public float inSight, attackRange;

    //Sound
    public AudioSource hitSound;
    private bool soundPlaying = false;

    private void Awake () {
        agentOne = GetComponent<NavMeshAgent> ();
    }

    private void Update () {
        float closest = Mathf.Infinity;
        Transform enemySelect = null;
        RaycastHit hit;
        foreach (Transform p in manager.enemies) {
            float d = Vector3.Distance (transform.position, p.position);
            if (d < inSight)
                if (Physics.Raycast (transform.position, (p.position - transform.position), out hit, inSight))
                    if (hit.transform == p)
                        if (d < closest) {
                            enemySelect = p;
                            closest = d;
                        }
        }

        if (!enemySelect) Scan ();
        else {
            if (closest <= attackRange) Attack (enemySelect);
            else Chase (enemySelect);
        }

        /* Old itteration
            //check to see if player is in sight and in attack range
            playerInRange = Physics.CheckSphere (transform.position, inSight, playerLayer);
            playerInAttackRange = Physics.CheckSphere (transform.position, attackRange, playerLayer);

            //when the player is not in attack range or the chase range run the Scan function
            if (!playerInRange && !playerInAttackRange) Scan ();
            //when the player is in chase range but not in attack range run the Chase function
            if (playerInRange && !playerInAttackRange) Chase ();
            //when the player is in range and close enough to attack run the Attack function
            if (playerInRange && playerInAttackRange) Attack ();
        */
    }

    //Function for finding the player
    private void Scan () {
        if (agentOne.remainingDistance > 0.1f) return;
        agentOne.SetDestination (NavMeshSpot ());

        /* Old itteration
            float RandomZ = Random.Range (-scanningRange, scanningRange);
            float RandomX = Random.Range (-scanningRange, scanningRange);

            Scanning = new Vector3 (transform.position.x + RandomX, transform.position.y, transform.position.z + RandomZ);

            if (Physics.Raycast (Scanning, -transform.up, 200f, platformLayer)) {
                SetScan = true;
                agentOne.SetDestination (Scanning);
            }
        */
    }

    public Vector3 NavMeshSpot () {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * scanningRange;
        randomDirection += origin.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition (randomDirection, out navHit, scanningRange, agentOne.areaMask);
        return navHit.position;
    }

    //Function for attacking the player
    private void Attack (Transform player) {
        float radius = 2.0f;
        Vector3 movePos = player.transform.position;
        movePos = Vector3.MoveTowards (movePos, transform.position, radius);

        agentOne.SetDestination (movePos);

        transform.LookAt (player);

        if (!Attacked) {
            // NEED TO ADD SOME FORM OF ATTACK HERE

            Attacked = true;
            Invoke (nameof (ResetAttack), attackTimer);
        }
    }

    //Function for Chasing the player
    private void Chase (Transform player) => agentOne.SetDestination (player.position);
    private void ResetAttack () => Attacked = false;

    void OnCollisionEnter(Collision spikeyCol)
    {
        if (soundPlaying == false && spikeyCol.gameObject.tag == "Sword")
        {
            hitSound.Play();
            soundPlaying = true;
        }
        else
        {
            soundPlaying = false;
        }
    }
}