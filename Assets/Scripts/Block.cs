using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float tilesToMove = 80f;
    [SerializeField] float moveSpeed = 50f;
    [SerializeField] float safeDistance = 1.5f;

    [Header("Effects")]
    [SerializeField] GameObject hitPrefab;

    public enum Direction { Up, Down, Left, Right }
    public Direction blockDirection;

    private bool isMoving = false;
    private static List<Block> allBlocks = new List<Block>();
    private Rigidbody rb;
    private AudioPlayer audioPlayer;

    void Awake()
    {
        allBlocks.Add(this);
        rb = GetComponent<Rigidbody>();

        // Initialize Rigidbody safely
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        }

        audioPlayer = FindObjectOfType<AudioPlayer>();
        if (audioPlayer == null)
            Debug.LogWarning("AudioPlayer not found in the scene!");
    }

    private void OnDestroy()
    {
        allBlocks.Remove(this);
    }

    private void OnMouseDown()
    {
        // If input not allowed and block is not moving, ignore
        if (!GameManager.instance.allowBlockInput && !isMoving)
            return;

        // Start movement only if not already moving
        if (!isMoving)
        {
            Vector3 dir = GetDirectionVector();
            StartCoroutine(MoveBlock(dir));
            GameManager.instance.UseMove();
        }
    }

    Vector3 GetDirectionVector()
    {
        switch (blockDirection)
        {
            case Direction.Up: return Vector3.forward;
            case Direction.Down: return Vector3.back;
            case Direction.Left: return Vector3.left;
            case Direction.Right: return Vector3.right;
            default: return Vector3.zero;
        }
    }

    private IEnumerator MoveBlock(Vector3 direction)
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + direction * tilesToMove;

        bool hitSomething = false;

        // Raycast to detect obstacles
        if (Physics.Raycast(startPos, direction, out RaycastHit hit, tilesToMove))
        {
            if (hit.collider.CompareTag("Block") || hit.collider.CompareTag("Obstacle"))
            {
                hitSomething = true;

                // Spawn hit effect
                if (hitPrefab != null)
                    Instantiate(hitPrefab, hit.point, Quaternion.identity);

                // Play hit sound
                audioPlayer?.PlayHitSound();

                // Stop before obstacle (safeDistance)
                float stopDist = Mathf.Max(0, hit.distance - safeDistance);
                endPos = startPos + direction * stopDist;
            }
        }

        // Play movement sound if nothing hit
        if (!hitSomething)
            audioPlayer?.PlayMoveSound();

        // Smooth movement toward end position
        while (Vector3.Distance(transform.position, endPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = endPos;
        isMoving = false;
        audioPlayer?.StopAllSounds();
    }

    public void StopMovement()
    {
        StopAllCoroutines();
        isMoving = false;
        audioPlayer?.StopAllSounds();
    }

    public static void DisableAllIdleBlocks()
    {
        foreach (var b in allBlocks)
        {
            if (!b.isMoving)
                b.enabled = false; // Disable interaction
        }
    }
}
