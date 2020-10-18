using System.Collections;
using System.Collections.Generic;
using Obi;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Slime : Agent {
    public FoodManager manager;
    public ObiSoftbody softBody;
    private ObiActor actor;
    public bool trainingMode = false;
    public float speed = 60f;
    public float jumpForce = 3f;
    public List<Transform> food = new List<Transform> ();
    public float inSight = 40;
    private Collider c;
    public EnemyManager e;
    public bool[] testingFood = new bool[0];

    void Start () {
        actor = GetComponent<ObiActor> ();
        c = GetComponent<Collider> ();
        e = GameObject.FindObjectOfType<EnemyManager> ();
    }

    public override void Initialize () {
        actor = GetComponent<ObiActor> ();
        c = GetComponent<Collider> ();
        e = GameObject.FindObjectOfType<EnemyManager> ();
        if (!trainingMode) MaxStep = 0;
        c.enabled = true;
        actor.ResetParticles ();
        softBody.deformationResistance = 0.7f;
    }

    public override void OnEpisodeBegin () { }

    public override void OnActionReceived (float[] vectorAction) {
        // jump
        if (Mathf.FloorToInt (vectorAction[2]) > 1)
            actor.AddForce (new Vector3 (0, -1, 0) * jumpForce, ForceMode.VelocityChange);
        // movement
        actor.AddForce (Vector3.Normalize (new Vector3 (vectorAction[0], 0, vectorAction[1])) * speed, ForceMode.Force);
    }

    public override void CollectObservations (VectorSensor sensor) {
        if (testingFood.Length != food.Count) {
            testingFood = new bool[food.Count];
            for (var _f = 0; _f < testingFood.Length; _f++) testingFood[_f] = true;
        }

        float closest = Mathf.Infinity;
        Transform enemySelect = null;
        RaycastHit hit;
        foreach (Transform p in e.allies) {
            float d = Vector3.Distance (transform.position, p.position);
            if (d < inSight)
                if (Physics.Raycast (transform.position, (p.position - transform.position), out hit, inSight))
                    if (hit.transform == p)
                        if (d < closest) {
                            enemySelect = p;
                            closest = d;
                        }
        }

        // 3
        if (enemySelect == null) sensor.AddObservation (new float[3]);
        else sensor.AddObservation (enemySelect);

        // 4
        sensor.AddObservation (transform.rotation);

        closest = Mathf.Infinity;
        enemySelect = null;
        for (var f = 0; f < food.Count; f++) {
            float d = Vector3.Distance (transform.position, food[f].position);
            if (d < inSight && testingFood[f])
                if (Physics.Raycast (transform.position, (food[f].position - transform.position), out hit, inSight))
                    if (hit.transform == food[f])
                        if (d < closest) {
                            enemySelect = food[f];
                            closest = d;
                        }
        }

        // 3
        if (enemySelect == null) sensor.AddObservation (new float[3]);
        else sensor.AddObservation (enemySelect);
    }

    public void Death () {
        AddReward (-1f);
        c.enabled = false;
        if (!trainingMode)
            softBody.deformationResistance = 0;
        EndEpisode ();

        if (trainingMode) {
            actor.ResetParticles ();
            Vector3 pos = e.allies[0].GetComponent<EnemyOneSM> ().NavMeshSpot ();
            pos.y = 1.5f;
            actor.Teleport (pos, manager.gameObject.transform.rotation);
        }
    }
}