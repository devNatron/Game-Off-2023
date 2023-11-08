using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groudDrag;

    [Header("Groud Check")]
    public float playerHeight;
    public LayerMask groudMask;
    bool isGrounded;
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 MoveDirection;
    Rigidbody playerRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerRigidbody.freezeRotation = true;
    }

    void Update()
    {
        isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            playerHeight * 0.5f + 0.2f,
            groudMask
        );

        MyInput();
        SpeedControl();

        if (isGrounded)
        {
            playerRigidbody.drag = groudDrag;
        }
        else
        {
            playerRigidbody.drag = 0;
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        MoveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        playerRigidbody.AddForce(MoveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(playerRigidbody.velocity.x, 0f, playerRigidbody.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            playerRigidbody.velocity = new Vector3(
                limitedVel.x,
                playerRigidbody.velocity.y,
                limitedVel.z
            );
        }
    }
}
