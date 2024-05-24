using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{
    public Vector3 targetPos;
    public Vector3 initialPos;
    public float speed;
    public GameObject cube;

    Vector3 direction;

    private bool triggered1 = false;
    private bool triggered2 = false;
    private bool triggered3 = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (triggered1)
        {
            moveCubeFront();
        }
        else
        {
            moveCubeBack();
        }

        //moveCubeBack();
        //triggered = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OLOLOLO");
        triggered1 = true;
    }
    private void moveCubeFront()
    {
        direction = (targetPos - cube.transform.position).normalized;
        cube.transform.position += direction * speed * Time.deltaTime;
        if(Vector3.Distance(cube.transform.position, targetPos) <= 0.1f)
        {
            triggered1 = false;
        }
    }

    private void moveCubeBack()
    {
        direction = (initialPos - cube.transform.position).normalized;
        cube.transform.position += direction * speed * Time.deltaTime;
    }
}
