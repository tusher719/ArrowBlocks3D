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

    private void Update()
    {
        Movement();
    }

    void Movement()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                isMoving = false;
            }
        }
    }

    void OnMouseDown()
    {
        Vector3 direction = GetDirectionVector();
        Move(direction);
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

    void Move(Vector3 direction)
    {
        if (!isMoving)
        {
            targetPos = transform.position + direction * tilesToMove;
            isMoving = true;

            GameManager.instance.UseMove();
        }
    }

    public void StopMovement()
    {
        isMoving = false;
    }
}
