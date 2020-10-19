using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTracking : MonoBehaviour
{
    public int startHealth = 1000;
    public int currentHealth;
    public HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startHealth;
        healthBar.SetMaxHealth(startHealth);
    }

    void takeHealth(int health)
    {
        currentHealth -= health;

        healthBar.SetHealth(currentHealth);
    }

    public void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("GOT HERE");
            takeHealth(1);
        }
    }
}
