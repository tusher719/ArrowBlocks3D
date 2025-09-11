using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCube : MonoBehaviour
{
    public int pointValue = 5;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Block"))
        {
            Score scoreSystem = FindObjectOfType<Score>();
            if(scoreSystem != null)
            {
                scoreSystem.AddPoint(pointValue);
            }
        }
    }
}
