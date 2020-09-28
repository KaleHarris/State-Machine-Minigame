using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class MovementInVR : MonoBehaviour
{
    //set variables
    public float speed = 1;
    public XRNode inputSource;
    public float grav = -9.81f;
    private float fallspeed;
    private XRRig rig;
    private Vector2 inputAxis;
    private CharacterController character;
    public LayerMask ground;

    // Start is called before the first frame update
    void Start()
    {
        // set the character equal to the component it has
        character = GetComponent<CharacterController>();
        //set the rig equal to the Rig component
        rig = GetComponent<XRRig>();
    }

    // Update is called once per frame
    void Update()
    {
        //get the left controller as an input device
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        //use the joystick as an input
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
    }

    private void FixedUpdate()
    {
        characterFollowHeadset();

        //checks to see what way the player is facing
        Quaternion headTurn = Quaternion.Euler(0, rig.cameraGameObject.transform.eulerAngles.y, 0);
        Vector3 direction = headTurn * new Vector3(inputAxis.x, 0, inputAxis.y);

        //Moves the character 
        character.Move(direction * Time.fixedDeltaTime * speed);

        //if the player is on the ground set the fall speed to 0 otherwise make the player fall at the given gravity
        bool isGrounded = PlayerOnGround();
        if (isGrounded)
        {
         fallspeed = 0;
        }
        else
        {
            fallspeed += grav * Time.fixedDeltaTime;
        }
        //moves the character down to make them fall
        character.Move(Vector3.down * fallspeed * Time.deltaTime);
    }

    void characterFollowHeadset()
    {
        character.height = rig.cameraInRigSpaceHeight;
        Vector3 characterCenter = transform.InverseTransformPoint(rig.cameraGameObject.transform.position);
        character.center = new Vector3(characterCenter.x, character.height / 2 + character.skinWidth, characterCenter.z);
    }

    //Checks to see if the player is on the ground or not
    bool PlayerOnGround()
    {
        Vector3 groundRay = transform.TransformPoint(character.center);
        float rayLength = character.center.y + 0.01f;
        bool hitGround = Physics.SphereCast(groundRay, character.radius, Vector3.down, out RaycastHit rayInfo, rayLength, ground);
        return hitGround;
    }
}
