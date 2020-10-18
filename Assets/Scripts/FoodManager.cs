using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour {
    public List<Transform> food = new List<Transform> ();
    private void Awake () {
        if (food.Count > 0) return;
        food = new List<Transform> ();

        foreach (SlimeFoodScript f in GetComponentsInChildren<SlimeFoodScript> ())
            food.Add (f.transform);

        foreach (GameObject a in GameObject.FindGameObjectsWithTag ("Slime")) {
            Slime _b = a.GetComponent<Slime> ();
            _b.manager = this;
            _b.food = food;
        }
    }
}