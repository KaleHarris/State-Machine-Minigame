using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlimeFuelTracking : MonoBehaviour
{
    public int StartFuel = 0;
    public int currentFuel;
    public FuelBar fuelBar;

    // Start is called before the first frame update
    void Start()
    {
        currentFuel = StartFuel;
        fuelBar.SetMinSlime(StartFuel);
    }

    void AddFuel(int fuel)
    {
        currentFuel += fuel;

        fuelBar.SetFuel(currentFuel);
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Slime"))
        {
            Debug.Log("GOT HERE");
            AddFuel(+1);
        }
    }
}
