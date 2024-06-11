using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public AudioSource error;
    private void OnCollisionEnter()
    {
        Debug.Log("error");
        error.Play();
    }
}
