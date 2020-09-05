using UnityEditor;
using UnityEngine;

public class SwordWhipHilt : MonoBehaviour {
    public GameObject grabber;
    public SwordWhip parent;
    private bool openCheck;
    private GameObject grabCheck;

    void Start () {
        // sets initial variables
        openCheck = parent.open;
        grabCheck = grabber;

        MeshCollider col = GetComponent<MeshCollider> ();
        if (!col.isTrigger) {
            col.enabled = false;
            col.enabled = true;
        }
    }

    void Update () {
        if (grabber) {
            // to stay in users hand
            parent.gameObject.GetComponent<Rigidbody> ().isKinematic = true;

            // fixes flicking / exaggerated velocity
            if (openCheck != parent.open || grabCheck != grabber) {
                parent.Detach ();
                parent.gameObject.transform.position = this.gameObject.transform.position;
                parent.gameObject.transform.rotation = this.gameObject.transform.rotation;
                parent.Reattach ();
                openCheck = parent.open;
                grabCheck = grabber;
            }

            if (parent.open) { // to apply joint physics properly
                this.gameObject.transform.position = grabber.transform.position;
                this.gameObject.transform.rotation = grabber.transform.rotation;
            } else { // to fixes offset when swinging
                parent.gameObject.transform.position = grabber.transform.position;
                parent.gameObject.transform.rotation = grabber.transform.rotation;
            }

        } else {
            // easier pickup physics
            parent.setState (false);

            // hey gravity is a think xD
            parent.gameObject.GetComponent<Rigidbody> ().isKinematic = false;
        }
    }

    public void toggleState () => parent.toggleState ();
}

// inspector clean up
[CustomEditor (typeof (SwordWhipHilt))]
public class SwordWhipHiltEditor : Editor {
    public override void OnInspectorGUI () {
        SwordWhipHilt script = (SwordWhipHilt) target;

        EditorGUILayout.BeginHorizontal ();
        GUILayout.Label ("Grabbing Object");
        script.grabber = (GameObject) EditorGUILayout.ObjectField (script.grabber, typeof (GameObject), true);
        EditorGUILayout.EndHorizontal ();
        if (GUILayout.Button ("Toggle State")) script.toggleState ();
    }
}