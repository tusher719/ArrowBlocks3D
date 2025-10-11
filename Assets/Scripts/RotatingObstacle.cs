using System.Collections;
using UnityEngine;

public class RotatingObstacle : MonoBehaviour
{
    [Header("Visual Effects")]
    public Color hitFlashColor = Color.red;
    public float flashDuration = 0.3f;

    [Header("References")]
    public GameObject rotatingCylinder; // The child cylinder that rotates

    private Renderer parentRenderer;
    private Renderer rotatingRenderer;

    private void Start()
    {
        parentRenderer = GetComponent<Renderer>();
        
        if (rotatingCylinder != null)
        {
            rotatingRenderer = rotatingCylinder.GetComponent<Renderer>();
        }
    }

    // Called by Block.cs when it hits this obstacle
    public void OnBlockHit()
    {
        StartCoroutine(FlashBothCylinders());
    }

    private IEnumerator FlashBothCylinders()
    {
        // Store original colors
        Color parentOriginalColor = Color.white;
        Color rotatingOriginalColor = Color.white;

        // Flash parent cylinder
        if (parentRenderer != null)
        {
            parentOriginalColor = parentRenderer.material.color;
            parentRenderer.material.color = hitFlashColor;
        }

        // Flash rotating cylinder
        if (rotatingRenderer != null)
        {
            rotatingOriginalColor = rotatingRenderer.material.color;
            rotatingRenderer.material.color = hitFlashColor;
        }

        // Wait
        yield return new WaitForSeconds(flashDuration);

        // Restore parent
        if (parentRenderer != null)
        {
            parentRenderer.material.color = parentOriginalColor;
        }

        // Restore rotating
        if (rotatingRenderer != null)
        {
            rotatingRenderer.material.color = rotatingOriginalColor;
        }
    }
}