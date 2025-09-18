using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block"))
        {
            Block blockScript = other.GetComponent<Block>();
            if (blockScript != null)
            {
                blockScript.StopMovement();
            }

            if (GameManager.instance != null)
            {
                GameManager.instance.BlockCleared();
            }

            Key key = other.GetComponentInChildren<Key>();
            if (key != null)
            {
                GameManager.instance.AddKey();
                Destroy(key.gameObject);
            }

            Destroy(other.gameObject);
        }
    }
}