using System.Collections;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Header("Move Settings")]
    public float raiseHeight = 3f;
    public float raiseSpeed = 10f;
    public float lowerSpeed = 10f;

    [Header("Rotation Settings")]
    public float rotateDuration = 0.5f;
    public float rotationAngle = 90f;

    [Header("Visual Effects")]
    public Color hitFlashColor = Color.red;
    public float flashDuration = 0.3f;

    private bool isRotating = false;
    private Vector3 originalPos;

    private void Start()
    {
        originalPos = transform.position;
    }

    private void OnMouseDown()
    {
        // Only allow rotation when input is enabled
        if (!isRotating && GameManager.instance != null && GameManager.instance.allowBlockInput)
        {
            StartCoroutine(RotateBlock());
        }
    }

    private IEnumerator RotateBlock()
    {
        isRotating = true;

        // Raise up
        Vector3 raisedPos = originalPos + Vector3.up * raiseHeight;
        while (Vector3.Distance(transform.position, raisedPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, raisedPos, raiseSpeed * Time.deltaTime);
            yield return null;
        }

        // Rotate
        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, rotationAngle, 0);
        float t = 0f;
        while (t < rotateDuration)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRot, endRot, t / rotateDuration);
            yield return null;
        }
        transform.rotation = endRot;

        // Lower down
        while (Vector3.Distance(transform.position, originalPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPos, lowerSpeed * Time.deltaTime);
            yield return null;
        }

        // Update original position after lowering
        originalPos = transform.position;

        // Use a move
        if (GameManager.instance != null)
        {
            GameManager.instance.UseMove();
        }

        isRotating = false;
    }

    // Called by Block when it hits this rotator
    public void OnBlockHit()
    {
        Debug.Log("Rotator hit detected!"); // Debug line
        StartCoroutine(FlashCylinders());
    }

    private IEnumerator FlashCylinders()
    {
        // Get this cylinder's renderer
        Renderer thisRenderer = GetComponent<Renderer>();
        
        // Try to get parent renderer
        Renderer parentRenderer = null;
        if (transform.parent != null)
        {
            parentRenderer = transform.parent.GetComponent<Renderer>();
        }

        // Store original colors
        Color thisOriginalColor = Color.white;
        Color parentOriginalColor = Color.white;

        // Flash this cylinder
        if (thisRenderer != null)
        {
            thisOriginalColor = thisRenderer.material.color;
            thisRenderer.material.color = hitFlashColor;
            Debug.Log("Flashing this cylinder");
        }

        // Flash parent cylinder
        if (parentRenderer != null)
        {
            parentOriginalColor = parentRenderer.material.color;
            parentRenderer.material.color = hitFlashColor;
            Debug.Log("Flashing parent cylinder");
        }

        // Wait
        yield return new WaitForSeconds(flashDuration);

        // Restore this cylinder
        if (thisRenderer != null)
        {
            thisRenderer.material.color = thisOriginalColor;
        }

        // Restore parent cylinder
        if (parentRenderer != null)
        {
            parentRenderer.material.color = parentOriginalColor;
        }
    }
}