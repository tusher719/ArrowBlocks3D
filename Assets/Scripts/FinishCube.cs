using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block"))
        {
            
            if (GameManager.instance != null)
            {
                GameManager.instance.BlockCleared();
            }

            Destroy(other.gameObject);
        }
    }
}
