using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private AudioPlayer audioPlayer;

    void Awake()
    {
        audioPlayer = FindObjectOfType<AudioPlayer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            // Stop block movement
            Block blockScript = collision.gameObject.GetComponent<Block>();
            if (blockScript != null)
            {
                blockScript.StopMovement();
                
                // Play hit sound
                if (audioPlayer != null)
                {
                    audioPlayer.PlayHitSound();
                }
            }
        }
    }
}