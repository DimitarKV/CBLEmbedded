using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetect : MonoBehaviour
{
    public GameObject[] servoTriggers;
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trash"))
        {
            Debug.Log("Trash");
        } 
        else if (other.CompareTag("White"))
        {
            Debug.Log("White");
        }
        else if (other.CompareTag("Black"))
        {
            Debug.Log("Black");
        }
    }
}
