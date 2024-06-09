using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServoPush : MonoBehaviour
{
    public Vector3 targetPos;
    public Vector3 initialPos;
    public float speed;
    public GameObject cube;

    Vector3 direction;

    public static bool triggered1 = false;

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
        if (!other.CompareTag("Trash"))
        {
            if (Container1.weight1 <= Container2.weight2 && Container1.weight1 <= Container3.weight3)
            {
                triggered1 = true;
            }
        }
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
