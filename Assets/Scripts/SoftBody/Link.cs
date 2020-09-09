using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link : MonoBehaviour {
    public GameObject sync;
    public void Update () {
        if (sync) this.gameObject.transform.position = sync.transform.position;
    }
}