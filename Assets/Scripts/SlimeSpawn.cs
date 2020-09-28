using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSpawn : MonoBehaviour
{

   // public GameObject slimeForSpawn;
    //public int XposSpawn;
   // public int ZposSpawn;
   // public int SlimeCount;

    public GameObject[] slimeGroup;
    public float timeToSpawn = 10f;
    private Vector3 spawnPos;

    // Start is called before the first frame update
    void Start()
    {

        InvokeRepeating("Spawn", timeToSpawn, timeToSpawn);


       // XposSpawn = Random.Range(-26, 26);
       // ZposSpawn = Random.Range(-26, 26);
        
       // Instantiate(slimeForSpawn, new Vector3(XposSpawn, 0, ZposSpawn), Quaternion.identity);
       // StartCoroutine(SlimeDrop());
    } 

    void Spawn()
    {
        spawnPos.x = Random.Range(-26, 26);
        spawnPos.y = 0.5f;
        spawnPos.z = Random.Range(-26, 26);
        Instantiate(slimeGroup[UnityEngine.Random.Range(0, slimeGroup.Length - 1)], spawnPos, Quaternion.identity);
    }

    /*
    IEnumerator SlimeDrop()
    {
        while (SlimeCount < 10)
        {
            
            
            yield return new WaitForSeconds(10f);
            SlimeCount += 1;
        }
    }
    */
}
