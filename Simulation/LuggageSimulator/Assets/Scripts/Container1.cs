using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Container1 : MonoBehaviour
{
    public static int weight1;
    public Light led1;
    public AudioSource empty;

    [SerializeField]
    public Text textField;

    private void Update()
    {
        if (weight1 == 40) 
        {
            led1.range = 1;
        }
        if (weight1 + ObjectHandling.handledWeight > 40
            && Container2.weight2 + ObjectHandling.handledWeight > 40
            && Container3.weight3 + ObjectHandling.handledWeight > 40)
        {
            led1.range = 1;
            textField.text = "Containers full. Empty!";
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //ObjectDetect.triggered = false;
        Debug.Log(other.tag);
        if (other.CompareTag("White"))
        {
            weight1 += 10;
            ObjectHandling.handledWeight = 0;
        }
        else if (other.CompareTag("Black"))
        {
            weight1 += 20;
            ObjectHandling.handledWeight = 0;
        }
        Debug.Log("Weight 1 = " + weight1);

        if (weight1 == 40 && Container2.weight2 == 40 && Container3.weight3 == 40)
        {
            textField.text = "Containers full. Empty!";
            empty.Play();
        }
    }
}
