using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{

    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    Camera cam;

    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float xRotation;
    float yRotation;

    public bool revoke_look = false;

    private void Start(){
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Confined; // originally this was .Locked
        Cursor.visible = false;
    }

    void Update(){
        if(!revoke_look){
            MyInput();
            cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }

    void MyInput(){
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }

    public void RevokeLook(){
        revoke_look = true;
    }

    public void AllowLook(){
        revoke_look = false;
    }

    public void ToggleLook(Component sender, object data){
        revoke_look = (bool)data;
    }
}
