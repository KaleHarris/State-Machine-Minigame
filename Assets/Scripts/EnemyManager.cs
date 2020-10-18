using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    public List<Transform> enemies = new List<Transform> ();
    public List<Transform> allies = new List<Transform> ();
    void Awake () {
        enemies = new List<Transform> ();

        foreach (GameObject p in GameObject.FindGameObjectsWithTag ("Player"))
            enemies.Add (p.transform);

        foreach (GameObject p in GameObject.FindGameObjectsWithTag ("Slime"))
            enemies.Add (p.transform);

        foreach (EnemyOneSM a in GetComponentsInChildren<EnemyOneSM> ()) {
            allies.Add (a.transform);
            a.manager = this;
        }
    }
}