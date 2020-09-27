using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeFoodScript : MonoBehaviour
{
    public bool holdingFood;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hands"))
        {
            holdingFood = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hands"))
        {
            holdingFood = false;
        }
    }
}
