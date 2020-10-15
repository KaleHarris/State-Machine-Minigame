using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    public GameObject targetDoor;
    bool isOpen = false;
    bool shouldMove = false;
    private float openPosition = -20f, closePosition = 16.46667f;
    public float  doorSpeed = 20f;

    
    void Update()
    {
        if (shouldMove == true){
            MoveDoor();
        }
      
    }

    void OnTriggerEnter(Collider Other){
        if(Other.gameObject.CompareTag("Hands")){
            shouldMove = true;
        }
    }

    void MoveDoor(){
        if (isOpen == false && targetDoor.transform.position.y > openPosition){
            Vector3 newPos = targetDoor.transform.position;
            newPos.y -= doorSpeed * Time.deltaTime;
            targetDoor.transform.position = newPos;
            if(targetDoor.transform.position.y <= openPosition){
                isOpen = true;
            shouldMove = false;
            }  
        }
        else if (isOpen == true && targetDoor.transform.position.y < closePosition){
            Vector3 newPos = targetDoor.transform.position;
            newPos.y += doorSpeed * Time.deltaTime;
            targetDoor.transform.position = newPos;
            if (targetDoor.transform.position.y >= closePosition){
                isOpen = false;
            shouldMove = false;
            }
        }
    }
}
