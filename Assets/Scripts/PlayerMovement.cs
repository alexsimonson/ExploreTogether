using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExploreTogether {
    public class PlayerMovement : MonoBehaviour
    {

        float playerHeight = 2f;

        [Header("Movement")]
        public float moveSpeed = 6f;

        [Header("Jumping")]
        public float jumpForce = 5f;

        [Header("Keybinds")]
        [SerializeField] KeyCode jumpKey = KeyCode.Space;
        [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;

        
        float movementMultiplier = 10f;
        float airMultiplier = .4f;
        float horizontalMovement;
        float verticalMovement;
        public bool isCrouching = false;
        float crouchMultiplier = 5f;

        Vector3 moveDirection;

        Rigidbody rb;
        float groundDrag = 6f;
        float airDrag = 2f;
        private float originalColliderHeight = 1.9f;
        [SerializeField] private float crouchHeight = 1f; // The height to shrink the collider when crouched
        private Vector3 originalColliderCenter;

        bool isGrounded;
        
        public bool revoke_movement = false;

        private CapsuleCollider capsuleCollider;

        Camera cam;

        void Awake(){
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            capsuleCollider = GetComponent<CapsuleCollider>();
            originalColliderCenter = capsuleCollider.center;
            cam = GetComponentInChildren<Camera>();
        }

        private void Update(){
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight/2 + 0.1f);  // adding .1f to be more consistent
            if(!revoke_movement){
                MyInput();
                ControlDrag();

                if(Input.GetKeyDown(jumpKey) && isGrounded){
                    Jump();
                }

                if(Input.GetKeyDown(crouchKey)){
                    Debug.Log("Crouching key pressed");
                    ToggleCrouch();
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
            HandleAnimation();
        }

        void MovePlayer(){
            if(isGrounded){
                if(isCrouching){
                    rb.AddForce(moveDirection.normalized * moveSpeed * crouchMultiplier,  ForceMode.Acceleration);
                }else{
                    rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier,  ForceMode.Acceleration);
                }
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

        void HandleAnimation(){
            if(gameObject.GetComponentInChildren<Animation>()==null) return;
            if(moveDirection==Vector3.zero){
                // stop animation
                gameObject.GetComponentInChildren<Animation>().Stop();
            }else{
                gameObject.GetComponentInChildren<Animation>().Play("Walk");
            }
        }

        private void ToggleCrouch()
        {
            isCrouching = !isCrouching;

            if (isCrouching)
            {
                // Shrink the collider
                capsuleCollider.height = crouchHeight;
                capsuleCollider.center = new Vector3(0f, crouchHeight / 2f, 0f);
                cam.transform.position = new Vector3(gameObject.transform.position.x, crouchHeight, gameObject.transform.position.z);
            }
            else
            {
                // Restore the collider to its original size
                capsuleCollider.height = originalColliderHeight;
                capsuleCollider.center = originalColliderCenter;
                cam.transform.position = new Vector3(cam.transform.position.x, 1.7f, cam.transform.position.z);
            }
        }
    }
}
