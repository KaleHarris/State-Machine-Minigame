using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    public InputDeviceCharacteristics ControllerCharacteristics;
    public GameObject handModelPrefab;
    private GameObject activeHandModel;
    private InputDevice targetDevice;
    private Animator handAnimator;

    // Start is called before the first frame update
    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
       
        InputDevices.GetDevicesWithCharacteristics(ControllerCharacteristics, devices);

        /* 
        foreach (var item in devices)
        {
            Debug.Log(item.name + item.characteristics);
        }
        */

        if(devices.Count > 0)
        {
            targetDevice = devices[0];
        }

        activeHandModel = Instantiate(handModelPrefab, transform);
        handAnimator = activeHandModel.GetComponent<Animator>();
    }

    void UpdateHandAnimation()
    {
        if(targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHandAnimation();

        targetDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool gripButtonValue);
        if (gripButtonValue)
        {
            //Debug.Log("primary button has been pressed");
        }
        targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
        if(triggerValue > 0.1f)
        {
           // Debug.Log("Trigger has been pressed" + triggerValue);
        }

        

    }
}
