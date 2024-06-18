using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDisks : MonoBehaviour
{
    [SerializeField]
    float[] percentages;
    public GameObject[] diskPrefabs;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(diskPrefabs[RandomSpawn()], transform.position, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private int RandomSpawn()
    {
        float random = UnityEngine.Random.Range(0f, 1f);
        float numForAdding = 0;
        float total = 0;
        for (int i = 0; i < percentages.Length; i++)
        {
            total += percentages[i];
        }

        for (int i = 0; i < diskPrefabs.Length; i++)
        {
            if (percentages[i] / total + numForAdding >= random)
            {
                return i;
            } 
            else
            {
                numForAdding += percentages[i] / total;
            }
        }
        return 0;
    }
    //(Random.Range(0, diskPrefabs.Length))
}
