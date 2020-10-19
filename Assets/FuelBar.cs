using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelBar : MonoBehaviour
{
    public Slider slider;

   public void SetMinSlime(int fuel)
    {
        slider.minValue = fuel;
        slider.value = fuel;
    }

    public void SetFuel(int fuel)
    {
        slider.value = fuel;
    }
}
