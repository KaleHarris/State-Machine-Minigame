using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class SlimeSM : MonoBehaviour
{
    public NavMeshAgent Slime;

    public Transform player;

    public LayerMask floorLayer, playerLayer;

    NavMeshHit hit;

    public Vector3 roaming;
    bool setToRoam;
    public float RoamRange;

    public float playerInSight, needToFlee;
    public bool playerInRange, fleeRange;

    private Transform startTransform;
    public float multiplyBy;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("VRRig").transform;
        Slime = GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    private void Update()
    {
        playerInRange = Physics.CheckSphere(transform.position, playerInSight, playerLayer);
        //fleeRange = Physics.CheckSphere(transform.position, needToFlee, playerLayer);


        if (!playerInRange && !fleeRange)
        {
            Roam();
        }

        //when the player is in chase range but not in attack range run the Chase function
        if (playerInRange && !fleeRange)
        {
            Follow();
        }

        //when the player is in range and close enough to attack run the Attack function
        if (playerInRange && fleeRange)
        {
            Follow();
        }
    }

    private void Roam()
    {

        Debug.Log("Slime is Roaming");

        if (!setToRoam) RoamArea();

        if (setToRoam)
            Slime.SetDestination(roaming);

        Vector3 distanceToDestination = transform.position - roaming;

        if (distanceToDestination.magnitude < 1f)
            setToRoam = false;
    }

    private void RoamArea()
    {
        float RandomZ = Random.Range(-RoamRange, RoamRange);
        float RandomX = Random.Range(-RoamRange, RoamRange);

        roaming = new Vector3(transform.position.x + RandomX, transform.position.y, transform.position.z + RandomZ);

        if (Physics.Raycast(roaming, -transform.up, 2f, floorLayer))
            setToRoam = true;
    }

    

    private void Follow()
    {
        
        

        if (GameObject.Find("SlimeFood").GetComponent<SlimeFoodScript>().holdingFood)
        {
            Slime.SetDestination(player.position);
        }
        else
        {
            startTransform = transform;

            transform.rotation = Quaternion.LookRotation(transform.position - player.position);

            Vector3 runTo = transform.position + transform.forward * multiplyBy;

            NavMesh.SamplePosition(runTo, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));

            transform.position = startTransform.position;
            transform.rotation = startTransform.rotation;

            Slime.SetDestination(hit.position);
        }
    }
    /*
    private void Flee()
    {
       
        Debug.Log("I Am Fleeing");
        startTransform = transform;

        transform.rotation = Quaternion.LookRotation(transform.position - player.position);

        Vector3 runTo = transform.position + transform.forward * multiplyBy;
       
        NavMesh.SamplePosition(runTo, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));

        transform.position = startTransform.position;
        transform.rotation = startTransform.rotation;

        Slime.SetDestination(hit.position);
        
    }  
    */
}
