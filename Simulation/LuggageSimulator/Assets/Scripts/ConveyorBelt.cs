using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConveyorBelt : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public List<GameObject> onBelt;

    void Start()
    {
        Time.timeScale = 1;
    }

    void Update()
    {
        foreach (var item in onBelt)
        {
            //item.GetComponent<Rigidbody>().velocity = direction * speed * Time.deltaTime;
            item.transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        onBelt.Add(collision.gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        onBelt.Remove(collision.gameObject);
    }
}
