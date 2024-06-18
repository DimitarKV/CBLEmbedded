using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;

public class ObjectHandling : MonoBehaviour
{
    public static bool handling = false;

    public Vector3 targetPos;
    public Vector3 initialPos;
    public float speed;
    public GameObject cube;

    Vector3 direction;

    public GameObject conveyor;

    public static int handledWeight;

    public AudioSource empty;

    [SerializeField]
    public Text textField;

    // Update is called once per frame
    void Update()
    {
        if (handling == false)
        {
            moveCubeBack();
        }
        else
        {
            moveCubeFront();
        }
        if ((Container1.weight1 + handledWeight > 40 && Container2.weight2 + handledWeight > 40 
            && Container3.weight3 + handledWeight > 40) || (Container1.weight1 == 40 
            && Container2.weight2 == 40 && Container3.weight3 == 40))
        {
            conveyor.SetActive(false);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        handling = true;
        if (other.CompareTag("White"))
        {
            handledWeight = 10;
            textField.text = "10 kg baggage";
        } 
        else if (other.CompareTag("Black"))
        {
            handledWeight = 20;
            textField.text = "20 kg baggage";
        }
        else if (other.CompareTag("Trash"))
        {
            textField.text = "Moving foreign object to trash!";
        }

        if (Container2.weight2 + handledWeight > 40
            && Container1.weight1 + handledWeight > 40
            && Container3.weight3 + handledWeight > 40)
        {
            empty.Play();
            textField.text = "Bins full, please empty and press Reset!";
        }
    }
    private void OnTriggerExit(Collider other)
    {
        handling = false;
        if (other.CompareTag("Trash"))
        {
            textField.text = "";
        }
    }


    private void moveCubeFront()
    {
        direction = (targetPos - cube.transform.position).normalized;
        cube.transform.position += direction * speed * Time.deltaTime;
    }

    private void moveCubeBack()
    {
        direction = (initialPos - cube.transform.position).normalized;
        cube.transform.position += direction * speed * Time.deltaTime;
    }
}
