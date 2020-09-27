using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;

public class Slime : MonoBehaviour {
    public ObiSoftbody softBody;
    public SkinnedMeshRenderer meshRenderer;
    public MeshCollider collider;

    void LateUpdate () {
        Mesh colliderMesh = new Mesh ();
        meshRenderer.BakeMesh (colliderMesh);
        collider.sharedMesh = null;
        collider.sharedMesh = colliderMesh;
    }

    public void Death () => softBody.deformationResistance = 0;
}