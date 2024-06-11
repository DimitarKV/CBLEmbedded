using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetect : MonoBehaviour
{
    public Vector3 targetPos;
    public Vector3 initialPos;
    public float speed;
    public GameObject cube;

    Vector3 direction;

    public static bool triggered = false;

    // Update is called once per frame
    void Update()
    {
        if (triggered)
        {
            if (ObjectHandling.handling == false)
            {
                moveCubeBack();
            }
            else
            {
                moveCubeFront();
            }
        }
        else
        {
            moveCubeFront();
        }

        //moveCubeBack();
        //triggered = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        triggered = true;
    }
    private void moveCubeBack()
    {
        Debug.Log("back");
        direction = (targetPos - cube.transform.position).normalized;
        cube.transform.position += direction * speed * Time.deltaTime;
        if (Vector3.Distance(cube.transform.position, targetPos) <= 0.1f)
        {
            triggered = false;
        }
    }

    private void moveCubeFront()
    {
        Debug.Log("front");
        direction = (initialPos - cube.transform.position).normalized;
        cube.transform.position += direction * speed * Time.deltaTime;
    }
}
