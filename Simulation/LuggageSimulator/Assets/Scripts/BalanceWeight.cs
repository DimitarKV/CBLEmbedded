using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceWeight : MonoBehaviour
{
    private int weight1 = 0;
    private int weight2 = 0;
    private int weight3 = 0;
    int currToHandle = 1;
    private int currWeight = 0;

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trash"))
        {
            ServoPush.triggered1 = false;
            Servo2Push.triggered2 = false;
            Servo3Push.triggered3 = false;
        }
        else
        {
            if (weight1 <= weight2 && weight1 <= weight3)
            {
                currToHandle = 1;
            } 
            else if (weight2 <= weight1 && weight2 <= weight3)
            {
                currToHandle = 2;
            } 
            else
            {
                currToHandle = 3;
            }
        
            if (other.CompareTag("White"))
            {
                currWeight  += 10;
            }
            else if (other.CompareTag("Black"))
            {
                currWeight += 20;
            }
        } 
    }
}
