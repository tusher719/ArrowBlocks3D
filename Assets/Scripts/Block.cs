using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] float tilesToMove = 10f;
    [SerializeField] float moveSpeed = 10f;

    public enum Direction { Up, Down, Left, Right }
    public Direction blockDirection;

    private Vector3 targetPos;
    private bool isMoving = false;

    AudioPlayer audioPlayer;

    void Awake()
    {
        audioPlayer = FindObjectOfType<AudioPlayer>();
        if (audioPlayer == null)
        {
            Debug.LogError("AudioPlayer not found in the scene!", this);
        }
    }

    void OnMouseDown()
    {
        if (!isMoving)
        {
            Vector3 direction = GetDirectionVector();
            Vector3 targetPos = transform.position + direction * tilesToMove;

            StartCoroutine(MoveToPosition(targetPos));

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
        }
        return Vector3.zero;
    }

    IEnumerator MoveToPosition(Vector3 targetPos)
    {
        isMoving = true;
        PlayMoveSound();
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
        StopAllSound();
    }

    public void StopMovement()
    {
        StopAllCoroutines();
        isMoving = false;
        StopAllSound();
    }

    void PlayMoveSound()
    {
        if (audioPlayer != null)
        {
            audioPlayer.PlayMoveSound();
        }
        audioPlayer.PlayMoveSound();
    }
    void StopAllSound()
    {
        if (audioPlayer != null)
        {
            audioPlayer.StopAllSounds();
        }
    }
}
