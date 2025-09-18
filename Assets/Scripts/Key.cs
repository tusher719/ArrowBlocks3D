using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!collected && other.CompareTag("finish"))
        {
            collected = true;

            Debug.Log("Key Collected");

            if (GameManager.instance != null)
            {
                GameManager.instance.AddKey();
            }

            Destroy(gameObject);
        }
    }
}
