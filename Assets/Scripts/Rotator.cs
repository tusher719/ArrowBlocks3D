using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{

    [Header("Move Settings")]
    public float raiseHeight = 1f;
    public float raiseSpeed = 5f;
    public float lowerSpeed = 5f;

    [Header("Rotation Settings")]
    public float rotateDuration = 0.5f;
    public float rotationAngle = 90f;

    private bool isRotating = false;
    private Vector3 originalPos;

    private void Start()
    {
        originalPos = transform.position;
    }

    private void OnMouseDown()
    {
        if (!isRotating)
        {
            StartCoroutine(RotateBlock());
        }
    }

    private IEnumerator RotateBlock()
    {
        isRotating = true;

        Vector3 raisedPos = originalPos + Vector3.up * raiseHeight;
        while (Vector3.Distance(transform.position, raisedPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, raisedPos, raiseSpeed * Time.deltaTime);
            yield return null;
        }

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

        while (Vector3.Distance(transform.position, originalPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPos, lowerSpeed * Time.deltaTime);
            yield return null;
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.UseMove();
        }

        isRotating = false;
    }
}
