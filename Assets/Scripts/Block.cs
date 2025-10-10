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
    [SerializeField] float chainPushDistance = 0.5f; // How far each block pushes forward
    [SerializeField] float chainPushSpeed = 10f; // Speed of push animation
    [SerializeField] float chainDelay = 0.05f; // Delay between each chain reaction

    public enum Direction { Up, Down, Left, Right }
    public Direction blockDirection;

    private bool isMoving = false;
    private static List<Block> allBlocks = new List<Block>();
    private Rigidbody rb;
    private AudioPlayer audioPlayer;

    // Track moving blocks
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
        movingBlocksCount++; // Increment when movement starts
        
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

                    // Get the first hit object
                    GameObject firstHitObject = hit.collider.gameObject;

                    // Flash and animate the FIRST hit object immediately
                    Renderer firstRenderer = firstHitObject.GetComponent<Renderer>();
                    if (firstRenderer != null)
                    {
                        StartCoroutine(FlashColor(firstRenderer));
                    }
                    StartCoroutine(ChainPushAnimation(firstHitObject.transform, direction));

                    // Then start chain reaction for objects AFTER the first hit
                    StartCoroutine(TriggerChainReaction(hit.point, direction, firstHitObject));

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
        movingBlocksCount--; // Decrement when movement ends
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
        if (targetRenderer != null) // Check if still exists
            targetRenderer.material.color = originalColor;
    }

    private IEnumerator TriggerChainReaction(Vector3 hitPoint, Vector3 direction, GameObject firstHitObject)
    {
        List<GameObject> chainObjects = new List<GameObject>();
        Vector3 checkPosition = hitPoint;
        
        // Find all objects in chain direction (AFTER the first hit object)
        for (int i = 0; i < 10; i++) // Max 10 objects in chain
        {
            RaycastHit chainHit;
            if (Physics.Raycast(checkPosition + direction * 0.1f, direction, out chainHit, tilesToMove))
            {
                GameObject hitObj = chainHit.collider.gameObject;
                
                // Skip the first hit object (already animated)
                if (hitObj == firstHitObject)
                {
                    checkPosition = chainHit.point;
                    continue;
                }
                
                if (chainHit.collider.CompareTag("Block") || chainHit.collider.CompareTag("Obstacle"))
                {
                    chainObjects.Add(hitObj);
                    checkPosition = chainHit.point;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }

        // Wait for first animation delay
        yield return new WaitForSeconds(chainDelay);

        // Trigger chain reaction with delay (NO red flash for these)
        for (int i = 0; i < chainObjects.Count; i++)
        {
            GameObject obj = chainObjects[i];
            if (obj != null)
            {
                // Push and pull back animation ONLY (no red flash)
                StartCoroutine(ChainPushAnimation(obj.transform, direction));
            }

            // Delay before next object in chain
            yield return new WaitForSeconds(chainDelay);
        }
    }

    private IEnumerator ChainPushAnimation(Transform target, Vector3 direction)
    {
        Vector3 originalPosition = target.position;
        Vector3 pushedPosition = originalPosition + direction * chainPushDistance;

        // Push forward
        float elapsed = 0f;
        float pushTime = chainPushDistance / chainPushSpeed;

        while (elapsed < pushTime)
        {
            if (target == null) yield break;
            
            target.position = Vector3.Lerp(originalPosition, pushedPosition, elapsed / pushTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (target != null)
            target.position = pushedPosition;

        // Pull back
        elapsed = 0f;
        while (elapsed < pushTime)
        {
            if (target == null) yield break;
            
            target.position = Vector3.Lerp(pushedPosition, originalPosition, elapsed / pushTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure back to original position
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
        // This gives time for blocks to settle before checking win/lose
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