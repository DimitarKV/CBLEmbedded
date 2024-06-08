using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDisks : MonoBehaviour
{
    public GameObject[] diskPrefabs;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(diskPrefabs[(Random.Range(0, diskPrefabs.Length))], transform.position, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
