using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float tilesToMove = 80f;
    [SerializeField] float moveSpeed = 50f;
    [SerializeField] float safeDistance = 1.5f;

    [Header("Hit Effect Settings")]
    [SerializeField] Color hitFlashColor = Color.red;
    [SerializeField] float flashDuration = 0.3f;
    [SerializeField] float shakeIntensity = 0.2f;
    [SerializeField] float shakeDuration = 0.3f;

    public enum Direction { Up, Down, Left, Right }
    public Direction blockDirection;

    private bool isMoving = false;
    private static List<Block> allBlocks = new List<Block>();
    private Rigidbody rb;
    private AudioPlayer audioPlayer;

    public static int movingBlocksCount = 0;

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
        movingBlocksCount++;
        
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + direction * tilesToMove;

        bool hitSomething = false;

        // Raycast to detect obstacles
        if (Physics.Raycast(startPos, direction, out RaycastHit hit, tilesToMove))
        {
            if (hit.collider.CompareTag("Block") || hit.collider.CompareTag("Obstacle"))
            {
                // Check if hit block is moving - if yes, don't stop
                Block hitBlock = hit.collider.GetComponent<Block>();
                if (hitBlock != null && hitBlock.isMoving)
                {
                    // Hit a moving block - ignore it and keep moving
                    hitSomething = false;
                }
                else
                {
                    hitSomething = true;

                    // Flash the hit object's color and shake it
                    Renderer hitRenderer = hit.collider.GetComponent<Renderer>();
                    if (hitRenderer != null)
                    {
                        StartCoroutine(FlashColor(hitRenderer));
                        StartCoroutine(ShakeObject(hit.collider.transform));
                    }

                    // Play hit sound
                    audioPlayer?.PlayHitSound();

                    // Stop before obstacle (safeDistance)
                    float stopDist = Mathf.Max(0, hit.distance - safeDistance);
                    endPos = startPos + direction * stopDist;
                }
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
        movingBlocksCount--;
        audioPlayer?.StopAllSounds();
        
        // Check if all blocks stopped moving and no moves left
        if (movingBlocksCount <= 0 && GameManager.instance != null)
        {
            if (GameManager.instance.allowBlockInput == false)
            {
                // All blocks stopped, trigger final check
                StartCoroutine(DelayedGameCheck());
            }
        }
    }

    private IEnumerator FlashColor(Renderer targetRenderer)
    {
        // Store original color
        Color originalColor = targetRenderer.material.color;

        // Flash to hit color
        targetRenderer.material.color = hitFlashColor;

        // Wait for flash duration
        yield return new WaitForSeconds(flashDuration);

        // Return to original color
        if (targetRenderer != null)
            targetRenderer.material.color = originalColor;
    }

    private IEnumerator ShakeObject(Transform target)
    {
        Vector3 originalPosition = target.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // Random shake offset
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float z = Random.Range(-1f, 1f) * shakeIntensity;

            target.position = originalPosition + new Vector3(x, 0, z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Return to original position
        if (target != null)
            target.position = originalPosition;
    }

    public void StopMovement()
    {
        StopAllCoroutines();
        if (isMoving)
        {
            movingBlocksCount--;
            isMoving = false;
        }
        audioPlayer?.StopAllSounds();
    }

    private IEnumerator DelayedGameCheck()
    {
        yield return new WaitForSeconds(0.1f);
    }

    public static void DisableAllIdleBlocks()
    {
        foreach (var b in allBlocks)
        {
            if (!b.isMoving)
                b.enabled = false;
        }
    }
}