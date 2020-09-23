using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SoftBody : MonoBehaviour {

    #region Setting up a softbody
    public List<Transform> Stem = new List<Transform> ();
    public List<Transform> Set1 = new List<Transform> ();
    public List<Transform> Set2 = new List<Transform> ();

    public void Link () { // specialized for The Teardrop creature
        var children = this.GetComponentsInChildren<Transform> ();
        Transform target = null;
        for (var c = 0; c < children.Length; c++) {
            if (children[c].name == "Bones") target = children[c];
            if (children[c].name == "Flesh") {
                DestroyImmediate (children[c].gameObject);
                return;
            }
        }

        children = target.GetComponentsInChildren<Transform> ();
        List<Transform> parts = new List<Transform> ();

        for (var g = 0; g < children.Length; g++)
            if (children[g].parent == target.gameObject.transform) parts.Add (children[g]);

        List<Transform> stem = new List<Transform> ();
        List<Transform> _set1 = new List<Transform> ();
        List<Transform> _set2 = new List<Transform> ();

        for (var p = 0; p < parts.Count; p++)
            try {
                if (int.Parse (parts[p].name.Split ('.') [1]) < 20) _set1.Add (parts[p]);
                else _set2.Add (parts[p]);
            } catch {
                var _parts = parts[p].GetComponentsInChildren<Transform> ();
                for (var s = 0; s < _parts.Length; s++)
                    stem.Add (_parts[s]);
            }

        List<Transform> set1 = new List<Transform> ();
        List<Transform> set2 = new List<Transform> ();
        for (var i = 0; i < 2; i++) {
            for (var j = 0; j < _set1.Count; j++)
                if (j % 2 == i) set1.Add (_set1[j]);
            for (var h = 0; h < _set2.Count; h++)
                if (h % 2 == i) set2.Add (_set2[h]);
        }

        Stem = stem;
        Set1 = set1;
        Set2 = set2;

        GameObject flesh;
        flesh = new GameObject ("Flesh");
        flesh.transform.SetParent (this.gameObject.transform, true);

        for (var x = 0; x < stem.Count; x++)
            connect ((!stem[x].gameObject.GetComponent<Link> ()) ? stem[x].gameObject.AddComponent<Link> () as Link : stem[x].gameObject.GetComponent<Link> (), x, stem, flesh);
        for (var x = 0; x < set1.Count; x++) {
            Transform[] set1_ = set1[x].GetComponentsInChildren<Transform> ();
            for (var y = 0; y < set1_.Length; y++)
                connect ((!set1_[y].gameObject.GetComponent<Link> ()) ? set1_[y].gameObject.AddComponent<Link> () as Link : set1_[y].gameObject.GetComponent<Link> (), y, flesh);
        }
        for (var x = 0; x < Set2.Count; x++) {
            Transform[] Set2_ = Set2[x].GetComponentsInChildren<Transform> ();
            for (var y = 0; y < Set2_.Length; y++)
                connect ((!Set2_[y].gameObject.GetComponent<Link> ()) ? Set2_[y].gameObject.AddComponent<Link> () as Link : Set2_[y].gameObject.GetComponent<Link> (), y, flesh);
        }
    }

    private void connect (Link link, int y, List<Transform> origin, GameObject flesh) {
        GameObject connector;
        connector = new GameObject ("connector");
        link.sync = connector;
        link.sync.transform.SetParent (flesh.gameObject.transform, true);
        connector.transform.position = link.gameObject.transform.position;
        Rigidbody rb = connector.gameObject.AddComponent<Rigidbody> () as Rigidbody;
        rb.mass = 0.1f;
        SphereCollider collider = connector.gameObject.AddComponent<SphereCollider> () as SphereCollider;
        collider.radius = new float[5] { 0.1f, 0.2f, 0.1f, 0.15f, 0.01f }[y];
    }

    private void connect (Link link, int y, GameObject flesh) {
        GameObject connector;
        connector = new GameObject ("connector");
        link.sync = connector;
        link.sync.transform.SetParent (flesh.gameObject.transform, true);
        connector.transform.position = link.gameObject.transform.position;
        Rigidbody rb = connector.gameObject.AddComponent<Rigidbody> () as Rigidbody;
        rb.mass = 0.1f;
        SphereCollider collider = connector.gameObject.AddComponent<SphereCollider> () as SphereCollider;
        collider.radius = new float[4] { 0.05f, 0.1f, 0.1f, 0.05f }[y];
    }

    public void Bink () { // needs work
        for (var x = 0; x < Stem.Count; x++) polish (Stem[x].GetComponent<Link> ().sync, x, Stem);
        for (var x = 0; x < Set1.Count; x++) {
            Transform[] Set1_ = Set1[x].GetComponentsInChildren<Transform> ();
            for (var y = 0; y < Set1_.Length; y++)
                polish (Set1_[y].gameObject.GetComponent<Link> ().sync, x, y, Set1, Set1_, Stem);
        }
        for (var x = 0; x < Set2.Count; x++) {
            Transform[] set2_ = Set2[x].GetComponentsInChildren<Transform> ();
            for (var y = 0; y < set2_.Length; y++)
                polish (set2_[y].gameObject.GetComponent<Link> ().sync, x, y, Set2, set2_, Stem);
        }
    }

    private void polish (GameObject connector, int y, List<Transform> origin) {
        if (y < origin.Count - 1) {
            SpringJoint spring = connector.gameObject.AddComponent<SpringJoint> () as SpringJoint;
            spring.connectedBody = origin[y + 1].GetComponent<Link> ().sync.GetComponent<Rigidbody> ();
            spring.spring = 100;
            spring.anchor = new Vector3 (0, 0, 0);
        }
    }

    private void polish (GameObject connector, int x, int y, List<Transform> origin, Transform[] branch, List<Transform> stem) {
        SpringJoint spring = null;
        for (var u = -1; u < 2; u++) { // x
            for (var v = -1; v < 2; v++) { // y
                if (Mathf.Abs (u) == Mathf.Abs (v) && u != 0) continue;
                spring = connector.gameObject.AddComponent<SpringJoint> () as SpringJoint;
                spring.anchor = new Vector3 (0, 0, 0);
                spring.connectedAnchor = new Vector3 (0, 0, 0);
                spring.spring = 100;
                if (u == v) spring.connectedBody = stem[y].GetComponent<Link> ().sync.GetComponent<Rigidbody> ();
                else {
                    if (Mathf.Abs (u) > Mathf.Abs (v)) {
                        if (x == origin.Count - 1 || x == 0) spring.connectedBody = origin[Mathf.Abs (x - origin.Count) - 1].GetComponentsInChildren<Transform> () [y].GetComponent<Link> ().sync.GetComponent<Rigidbody> ();
                        else spring.connectedBody = origin[x + u].GetComponentsInChildren<Transform> () [y].GetComponent<Link> ().sync.GetComponent<Rigidbody> ();
                    } else if (y != branch.Length - 1 && y != 0) spring.connectedBody = branch[y + v].GetComponent<Link> ().sync.GetComponent<Rigidbody> ();
                }

                // destroy all none connected springs
                if (!spring.connectedBody) DestroyImmediate (spring);
            }
        }
    }
    #endregion 
}

[CustomEditor (typeof (SoftBody))]
public class SoftBodyEditor : Editor {
    public override void OnInspectorGUI () {
        SoftBody script = (SoftBody) target;
        if (GUILayout.Button ("Link")) script.Link ();
        if (GUILayout.Button ("Bink")) script.Bink ();

        EditorGUILayout.PropertyField (serializedObject.FindProperty ("Stem"));
        EditorGUILayout.PropertyField (serializedObject.FindProperty ("Set1"));
        EditorGUILayout.PropertyField (serializedObject.FindProperty ("Set2"));
    }
}