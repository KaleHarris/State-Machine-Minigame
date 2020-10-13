using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;


public class SlimeSM : MonoBehaviour
{
    public AudioClip deathSound; 
    public AudioSource audioSourceSlime; //Used for ambient slimey noise

    public NavMeshAgent Slime;

    public Transform player;

    public LayerMask floorLayer, playerLayer;

    NavMeshHit hit;

    public Vector3 roaming;
    bool setToRoam;
    public float RoamRange;

    public float playerInSight;
    public bool playerInRange;

    private Transform startTransform;
    public float multiplyBy;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("VRRig").transform;
        Slime = GetComponent<NavMeshAgent>();

        audioSourceSlime.Play(); //Adds slime noise and plays, loops, and sets random pitch for each slime to distinguish them
        audioSourceSlime.loop = true;
        audioSourceSlime.pitch = (Random.Range(0.5f, 1.2f));
    }

    // Update is called once per frame
    private void Update()
    {
        //audioSourceSlime.pitch = (Random.Range(0.5f, 1.2f));

        playerInRange = Physics.CheckSphere(transform.position, playerInSight, playerLayer);
        //fleeRange = Physics.CheckSphere(transform.position, needToFlee, playerLayer);


        if (!playerInRange)
        {
            Roam();
        }

        //when the player is in chase range but not in attack range run the Chase function
        if (playerInRange)
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

        if (Vector3.Distance(GameObject.Find("VRRig").transform.position, Slime.transform.position) < 100)
        {
            startTransform = transform;

            transform.rotation = Quaternion.LookRotation(transform.position - player.position);

            Vector3 runTo = transform.position + transform.forward * multiplyBy;

            NavMesh.SamplePosition(runTo, out hit, 20, 1 << NavMesh.GetAreaFromName("Walkable"));

            transform.position = startTransform.position;
            transform.rotation = startTransform.rotation;

            Slime.SetDestination(hit.position);           
        }
        else
        {
            Slime.SetDestination(player.position);
        }
    }

    private void OnCollisionEnter(Collision SlimeCol)
    {

        if (SlimeCol.gameObject.tag == "Sword")
        {
            Destroy(this.gameObject);
            ScoreScript.scoreValue += 1;

            AudioSource.PlayClipAtPoint (deathSound, transform.position); //Plays the death sound at the slime's position when hit
            audioSourceSlime.Stop();
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
