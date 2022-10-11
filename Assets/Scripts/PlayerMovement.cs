using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    float playerHeight = 2f;

    [Header("Movement")]
    public float moveSpeed = 6f;

    [Header("Jumping")]
    public float jumpForce = 5f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;

    
    
    float movementMultiplier = 10f;
    float airMultiplier = .4f;
    float horizontalMovement;
    float verticalMovement;

    Vector3 moveDirection;

    Rigidbody rb;
    float groundDrag = 6f;
    float airDrag = 2f;

    bool isGrounded;
    
    public bool revoke_movement = false;

    private void Start(){
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update(){
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight/2 + 0.1f);  // adding .1f to be more consistent
        if(!revoke_movement){
            MyInput();
            ControlDrag();

            if(Input.GetKeyDown(jumpKey) && isGrounded){
                Jump();
            }
        }
    }

    void MyInput(){
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");
        moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;
    }

    void ControlDrag(){
        if(isGrounded){
            rb.drag = groundDrag;
        }else{
            rb.drag = airDrag;
        }
    }

    private void FixedUpdate(){
        MovePlayer();
    }

    void MovePlayer(){
        if(isGrounded){
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier,  ForceMode.Acceleration);
        }else{
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier,  ForceMode.Acceleration);
        }
    }

    void Jump(){
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    public void RevokeMovement(){
        revoke_movement = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;    // is this necessary?
    }

    public void AllowMovement(){
        revoke_movement = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.freezeRotation = true;
    }
}
