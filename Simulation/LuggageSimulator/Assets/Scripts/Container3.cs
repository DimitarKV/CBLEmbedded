using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Container3 : MonoBehaviour
{
    public static int weight3;
    public Light led3;
    public AudioSource empty;

    [SerializeField]
    public Text textField;

    private void Update()
    {
        if (weight3 == 40)
        {
            led3.range = 1;
        }
        if (weight3 + ObjectHandling.handledWeight > 40
            && Container2.weight2 + ObjectHandling.handledWeight > 40
            && Container1.weight1 + ObjectHandling.handledWeight > 40)
        {
            led3.range = 1;
            textField.text = "Bins full, please empty and press Reset!";
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //ObjectDetect.triggered = false;
        if (other.CompareTag("White"))
        {
            weight3 += 10;
            ObjectHandling.handledWeight = 0;
        }
        else if (other.CompareTag("Black"))
        {
            weight3 += 20;
            ObjectHandling.handledWeight = 0;
        }
        Debug.Log("Weight 3 = " + weight3);

        if (weight3 == 40 && Container2.weight2 == 40 && Container1.weight1 == 40)
        {
            textField.text = "Bins full, please empty and press Reset!";
            empty.Play();
        }

    }
}
