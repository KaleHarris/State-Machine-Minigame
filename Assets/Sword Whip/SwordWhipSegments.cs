using UnityEditor;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (MeshCollider))]
public class SwordWhipSegments : MonoBehaviour {
    // positional variables
    public Vector3 closedAnchor;
    public Vector3 openAnchor;

    // animation variables
    public float deltaComplete;
    public float angularYLimit;
    public float angularZLimit;
    public float highAngularXLimit;
    public float lowAngularXLimit;

    // animation variables
    public bool openState = true;
    private bool complete = true;
    private float thetaTime;

    private void Start () {
        MeshCollider col = GetComponent<MeshCollider> ();
        if (!col.isTrigger) {
            col.enabled = false;
            col.enabled = true;
        }
    }
    public void Config (Vector3 _anchor, bool _openState, float _time) {
        closedAnchor = _anchor;
        openState = _openState;
        deltaComplete = _time;
        BaseConfig ();
        if (!_openState) StartClosed ();
    }

    private void BaseConfig () {
        ConfigurableJoint joint = this.gameObject.GetComponent<ConfigurableJoint> ();
        angularYLimit = joint.angularYLimit.limit;
        angularZLimit = joint.angularZLimit.limit;
        highAngularXLimit = joint.highAngularXLimit.limit;
        lowAngularXLimit = joint.lowAngularXLimit.limit;
        openAnchor = joint.anchor;
    }

    void StartClosed () {
        ConfigurableJoint joint = this.gameObject.GetComponent<ConfigurableJoint> ();
        SoftJointLimit zeros = new SoftJointLimit ();
        zeros.limit = zeros.bounciness = zeros.contactDistance = 0f;

        joint.anchor = closedAnchor;
        joint.angularYLimit = joint.angularZLimit = joint.highAngularXLimit = joint.lowAngularXLimit = zeros;
        joint.xMotion = joint.yMotion = joint.zMotion = joint.angularXMotion = joint.angularYMotion = joint.angularZMotion = ConfigurableJointMotion.Locked;
    }

    public void startAnimation () {
        complete = false;
        thetaTime = 0;
        openState = !openState;
    }

    public float Animate (float add) {
        if (complete) return 0;

        // change in time
        thetaTime += (add == 0) ? Time.deltaTime : add;

        // animate
        if (openState) aniOpen ();
        else aniClose ();

        // completion
        if (thetaTime > deltaComplete) {
            complete = true;
            return thetaTime - deltaComplete;
        }

        return -1;
    }

    // animate to open state
    void aniOpen () {
        ConfigurableJoint joint = this.gameObject.GetComponent<ConfigurableJoint> ();
        joint.xMotion = joint.yMotion = joint.zMotion = joint.angularXMotion = joint.angularYMotion = joint.angularZMotion = ConfigurableJointMotion.Limited;

        joint.anchor = Vector3.Lerp (closedAnchor, openAnchor, thetaTime / deltaComplete);

        SoftJointLimit deltaJoint = new SoftJointLimit ();

        deltaJoint.limit = angularYLimit;
        joint.angularYLimit = deltaJoint;

        deltaJoint.limit = angularZLimit;
        joint.angularZLimit = deltaJoint;

        deltaJoint.limit = highAngularXLimit;
        joint.highAngularXLimit = deltaJoint;

        deltaJoint.limit = lowAngularXLimit;
        joint.lowAngularXLimit = deltaJoint;
    }

    // animate to closed state
    void aniClose () {
        ConfigurableJoint joint = this.gameObject.GetComponent<ConfigurableJoint> ();

        joint.anchor = Vector3.Lerp (openAnchor, closedAnchor, thetaTime / deltaComplete);

        SoftJointLimit deltaJoint = new SoftJointLimit ();

        if (thetaTime > deltaComplete) {
            joint.xMotion = joint.yMotion = joint.zMotion = joint.angularXMotion = joint.angularYMotion = joint.angularZMotion = ConfigurableJointMotion.Locked;

            joint.angularYLimit = deltaJoint;
            joint.angularZLimit = deltaJoint;
            joint.highAngularXLimit = deltaJoint;
            joint.lowAngularXLimit = deltaJoint;
        } else {
            deltaJoint.limit = angularYLimit - angularYLimit * (thetaTime / deltaComplete);
            joint.angularYLimit = deltaJoint;

            deltaJoint.limit = angularZLimit - angularZLimit * (thetaTime / deltaComplete);
            joint.angularZLimit = deltaJoint;

            deltaJoint.limit = highAngularXLimit - highAngularXLimit * (thetaTime / deltaComplete);
            joint.highAngularXLimit = deltaJoint;

            deltaJoint.limit = lowAngularXLimit - lowAngularXLimit * (thetaTime / deltaComplete);
            joint.lowAngularXLimit = deltaJoint;
        }
    }
}

// inspector shows nothing
[CustomEditor (typeof (SwordWhipSegments))] public class SwordWhipSegmentsEditor : Editor { public override void OnInspectorGUI () { } }