using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMachScript : MonoBehaviour
{
    public int Score;

    private void Start()
    {
        Score = 0;
    }

    private void ScoreCheck()
    {
        Debug.Log("The Score is " + Score);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Slime"))
        {
            Destroy(other.gameObject);
            Score ++;
            ScoreCheck();
        }        
    }

}
