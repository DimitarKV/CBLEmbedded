using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour
{
    public GameObject conveyor;
    public void SceneReset()
    {
        SceneManager.LoadScene("Simulation");
        ObjectHandling.handling = false;
        Container1.weight1 = 0;
        Container2.weight2 = 0;
        Container3.weight3 = 0;
        conveyor.SetActive(true);
    }
}
