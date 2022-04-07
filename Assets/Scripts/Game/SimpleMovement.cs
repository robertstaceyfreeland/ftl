using UnityEngine;

public class SimpleMovement : MonoBehaviour
{

    private Rigidbody2D rigidBody2d;
    public float moveSpeed = 3;

    void Start()
    {
        //rigidBody2d = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            moveY = +1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveY = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX = +1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1;
        }

        Vector3 moveDirection = new Vector3(moveX, moveY).normalized;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;

    }
}
