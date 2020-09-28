using UnityEditor;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class SwordWhip : MonoBehaviour {
    public int segments = 1;
    public GameObject hilt;
    public GameObject segment;
    public GameObject tip;
    public Vector3 jointClosedPos;
    public bool open = true;
    public float time = 0;
    private bool opencheck = true;
    private GameObject[] children;
    private bool animComplete = true;

    public void CreateSword () {
        if (!this.gameObject.GetComponent<ConfigurableJoint> ()) {
            Debug.LogError ("<color=red>Sword was not Created!</color>");
            Debug.LogError ("<color=red>A COMPONENT IS MISSING:</color>\nAdd a <color=blue>Configurable Joint</color> to the Object with the <color=blue>SwordWhip</color> script");
            return;
        }

        opencheck = open;

        // get original transformation
        Vector3 _posit = this.gameObject.transform.position;
        Quaternion _rotat = this.gameObject.transform.rotation;
        Vector3 _scale = this.gameObject.transform.localScale;

        this.gameObject.transform.position = new Vector3 (0, 0, 0);
        this.gameObject.transform.rotation = new Quaternion (0, 0, 0, 0);
        this.gameObject.transform.localScale = new Vector3 (1, 1, 1);

        // delete children if it has any
        Transform[] transforms = this.gameObject.GetComponentsInChildren<Transform> ();
        for (var t = 0; t < transforms.Length; t++)
            if (this.gameObject != transforms[t].gameObject)
                DestroyImmediate (transforms[t].gameObject);

        // actual start to the sword creation
        this.gameObject.GetComponent<Rigidbody> ().useGravity = true;

        hilt.GetComponent<Rigidbody> ().isKinematic = true;

        // create the sword
        Instantiate (hilt, this.transform);
        if (segments > 0) {
            for (var i = 0; i < segments - 1; i++) {
                Instantiate (segment, this.transform);
            }
            Instantiate (tip, this.transform);
        }

        // copy joint values
        ConfigurableJoint thisJoint = this.GetComponent<ConfigurableJoint> ();
        UnityEditorInternal.ComponentUtility.CopyComponent (thisJoint);

        SwordWhipSegments[] _segments = this.GetComponentsInChildren<SwordWhipSegments> ();
        for (var s = 0; s < _segments.Length; s++) {
            ConfigurableJoint joint = _segments[s].gameObject.GetComponent<ConfigurableJoint> ();

            // paste joint values
            UnityEditorInternal.ComponentUtility.PasteComponentValues (joint);

            _segments[s].gameObject.transform.position = new Vector3 (0, ((1 * s) + 2), 0);
            _segments[s].Config (jointClosedPos, open, time / segments);

            if (s > 0) {
                joint.connectedBody = _segments[s - 1].gameObject.GetComponent<Rigidbody> ();
            } else if (s == 0) { // connected to hilt
                joint.connectedBody = this.GetComponentInChildren<SwordWhipHilt> ().gameObject.GetComponent<Rigidbody> ();
                joint.connectedAnchor = new Vector3 (0, joint.connectedAnchor.y + 1, 0);
            }
        }

        // hilt variables 
        this.gameObject.GetComponentInChildren<SwordWhipHilt> ().parent = this;

        // reset original transformation
        this.gameObject.transform.position = _posit;
        this.gameObject.transform.rotation = _rotat;
        this.gameObject.transform.localScale = _scale;

        Debug.Log ("Sword Created");
    }

    void Start () {
        Destroy (this.gameObject.GetComponent<ConfigurableJoint> ());
        opencheck = open;
    }

    bool b_ = true;
    public void DeltaActive (bool b) {
        if (b == b_) return;
        b_ = b;
        MeshRenderer[] mr = this.GetComponentsInChildren<MeshRenderer> ();
        foreach (MeshRenderer r in mr)
            r.enabled = b;
        MeshCollider[] mc = this.GetComponentsInChildren<MeshCollider> ();
        foreach (MeshCollider c in mc)
            c.enabled = b;
        BoxCollider[] bc = this.GetComponentsInChildren<BoxCollider> ();
        foreach (BoxCollider a in bc)
            a.enabled = b;
    }

    public void Detach () {
        // set up children Array to reattach
        SwordWhipSegments[] _segments = this.gameObject.GetComponentsInChildren<SwordWhipSegments> ();
        children = new GameObject[(_segments.Length + 1)];
        children[0] = this.gameObject.GetComponentInChildren<SwordWhipHilt> ().gameObject;
        for (var s = 0; s < _segments.Length; s++)
            children[s + 1] = _segments[s].gameObject;

        this.gameObject.transform.DetachChildren ();
    }

    public void Reattach () {
        for (var c = 0; c < children.Length; c++)
            children[c].transform.SetParent (this.gameObject.transform, true);
    }

    public void toggleState () => open = !open;
    public void setState (bool o) => open = o;

    void Update () {
        this.gameObject.GetComponent<BoxCollider> ().isTrigger = open;

        SwordWhipSegments[] swordSegments = this.gameObject.GetComponentsInChildren<SwordWhipSegments> ();

        // sets up animation phase
        if (open != opencheck)
            for (var s = 0; s < swordSegments.Length; s++) {
                swordSegments[s].startAnimation ();
                opencheck = open;
                animComplete = false;
            }

        if (animComplete) return;

        // animation close/open
        int deltaComplete = 0;
        float gammaTime = 0;
        if (!open)
            for (var s = 0; s < swordSegments.Length; s++) {
                gammaTime = swordSegments[s].Animate (gammaTime);
                if (gammaTime == -1) break;
                deltaComplete = s + 1;
            }
        else
            for (var s = swordSegments.Length - 1; s > -1; s--) {
                gammaTime = swordSegments[s].Animate (gammaTime);
                if (gammaTime == -1) break;
                deltaComplete = swordSegments.Length - s;
            }
        // animation complete
        if (deltaComplete == swordSegments.Length) animComplete = true;

    }
}

// custom inspector
[CustomEditor (typeof (SwordWhip))]
public class SwordWhipEditor : Editor {
    bool variables, parts = true;
    public override void OnInspectorGUI () {
        SwordWhip script = (SwordWhip) target;

        parts = EditorGUILayout.Foldout (parts, "Sword Objects");
        if (parts) {
            EditorGUILayout.BeginHorizontal ();
            GUILayout.Label ("Hilt");
            script.hilt = (GameObject) EditorGUILayout.ObjectField (script.hilt, typeof (GameObject), false);
            EditorGUILayout.EndHorizontal ();

            EditorGUILayout.BeginHorizontal ();
            GUILayout.Label ("Segment");
            script.segment = (GameObject) EditorGUILayout.ObjectField (script.segment, typeof (GameObject), false);
            EditorGUILayout.EndHorizontal ();

            EditorGUILayout.BeginHorizontal ();
            GUILayout.Label ("Tip");
            script.tip = (GameObject) EditorGUILayout.ObjectField (script.tip, typeof (GameObject), false);
            EditorGUILayout.EndHorizontal ();
        }

        variables = EditorGUILayout.Foldout (variables, "Sword Variables");
        if (variables) {
            script.segments = EditorGUILayout.IntField ("Segments", ((script.segments >= 0) ? script.segments : 0));
            script.open = EditorGUILayout.Toggle ("Starts Open", script.open);
            script.jointClosedPos = EditorGUILayout.Vector3Field ("Closed Position", script.jointClosedPos);
            script.time = Mathf.Abs (EditorGUILayout.FloatField ("Animation Time", script.time));
        }

        if (GUILayout.Button ("Create Sword")) script.CreateSword ();
        if (GUILayout.Button ("Toggle State")) script.toggleState ();
    }
}