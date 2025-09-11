using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public GameObject hitPrefab;
    public float prefabShowTime = 1f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            if (hitPrefab != null)
            {
                Vector3 spawnPos = collision.contacts[0].point;
                GameObject obj = Instantiate(hitPrefab, spawnPos, Quaternion.identity);
                Destroy(obj, prefabShowTime);
            }

            Block blockScript = collision.gameObject.GetComponent<Block>();
            if (blockScript != null)
            {
                blockScript.StopMovement();
            }
        }
    }
}
