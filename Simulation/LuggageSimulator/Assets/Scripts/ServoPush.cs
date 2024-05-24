using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ServoPush : MonoBehaviour
{
    public Transform back;
    public Transform front;

    Transform target;
    Transform current;

    public float speed;
    private float sinTime; 
    
    public GameObject servo1;

    // Start is called before the first frame update
    void Start()
    {
        current = back;
        target = front;
        servo1.transform.position = current.position;
    }

    private void Update()
    {
        current = back;
        target = front;
        Debug.Log("AAAAAAAAAAAAAAA");
        if (servo1.transform.position != target.position)
        {
            /*sinTime = Time.deltaTime * speed;
            sinTime = Mathf.Clamp(sinTime, 0, Mathf.PI);
            float t = evaluate(sinTime);
            servo1.transform.position = Vector3.Lerp(current.position, target.position, t);*/
            servo1.transform.position = Vector3.MoveTowards(current.position, target.position, speed);
        }
        swap();
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    public void swap()
    {
        if (servo1.transform.position != target.position) 
        {
            return;
        }
        current = front;
        target = back;
        servo1.transform.position = Vector3.MoveTowards(current.position, target.position, speed);
    }
    public float evaluate(float z)
    {
        return 0.5f * Mathf.Sin(z - Mathf.PI / 2f) + 0.5f;
    }
}
