using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Container2 : MonoBehaviour
{
    public static int weight2;
    public Light led2;
    public AudioSource empty;

    [SerializeField]
    public Text textField;

    private void Update()
    {
        if (weight2 == 40)
        {
            led2.range = 1;
        }
        if (weight2 + ObjectHandling.handledWeight > 40
            && Container1.weight1 + ObjectHandling.handledWeight > 40
            && Container3.weight3 + ObjectHandling.handledWeight > 40)
        {
            led2.range = 1;
            textField.text = "Containers full. Empty!";
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //ObjectDetect.triggered = false;
        if (other.CompareTag("White"))
        {
            weight2 += 10;
        }
        else if (other.CompareTag("Black"))
        {
            weight2 += 20;
        }
        Debug.Log("Weight 2 = " + weight2);

        if (weight2 == 40 && Container1.weight1 == 40 && Container3.weight3 == 40)
        {
            textField.text = "Containers full. Empty!";
            empty.Play();
        }
    }
}
