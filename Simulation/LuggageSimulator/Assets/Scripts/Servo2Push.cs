using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Servo2Push : MonoBehaviour
{
    public Vector3 targetPos;
    public Vector3 initialPos;
    public float speed;
    public GameObject cube;

    Vector3 direction;

    private bool triggered = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (triggered)
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
        if (!other.CompareTag("Trash"))
        {
            triggered = true;
        }
    }
    private void moveCubeFront()
    {
        direction = (targetPos - cube.transform.position).normalized;
        cube.transform.position += direction * speed * Time.deltaTime;
        if (Vector3.Distance(cube.transform.position, targetPos) <= 0.1f)
        {
            triggered = false;
        }
    }

    private void moveCubeBack()
    {
        direction = (initialPos - cube.transform.position).normalized;
        cube.transform.position += direction * speed * Time.deltaTime;
    }
}
