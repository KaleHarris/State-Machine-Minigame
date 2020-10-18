using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameReset : MonoBehaviour {
    public GameObject enemyLayer;
    public GameObject foodLayer;
    public GameObject food;

    List<Vector3> foodPos = new List<Vector3> ();
    List<Vector3> enemyPos = new List<Vector3> ();
    void Start () {
        SlimeFoodScript[] s = foodLayer.GetComponentsInChildren<SlimeFoodScript> ();
        foreach (SlimeFoodScript _s in s) foodPos.Add (_s.gameObject.transform.position);

        EnemyOneSM[] e = enemyLayer.GetComponentsInChildren<EnemyOneSM> ();
        foreach (EnemyOneSM _e in e) enemyPos.Add (_e.gameObject.transform.position);
    }

    public void ResetStage () {
        EnemyOneSM[] enemies = enemyLayer.GetComponentsInChildren<EnemyOneSM> ();
        for (var e = 0; e < enemies.Length; e++) enemies[e].transform.position = enemyPos[e];

    }
}