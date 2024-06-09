using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Empty : MonoBehaviour
{
    public GameObject conveyor;
    public AudioSource empty;
    // Update is called once per frame
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Conveyor"))
        {
            Debug.Log("empty");
            empty.Play();
        }
    }
}
