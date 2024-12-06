using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    public GameObject inventoryUI; 
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 4.0f;
    public Transform armPivot;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

    }

    void Update()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftControl);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        Quaternion goalAngle = playerCamera.transform.rotation * Quaternion.Euler(MathF.Sin(Time.time*MathF.Sqrt(MathF.Abs(curSpeedX) + 0.21f)*5)*(MathF.Abs(curSpeedX) + 0.21f), MathF.Sin(Time.time*MathF.Sqrt(MathF.Abs(curSpeedY) + 0.2f)*5)*(MathF.Abs(curSpeedY) + 0.2f), 0);
        armPivot.rotation = Quaternion.RotateTowards(armPivot.rotation, goalAngle, Quaternion.Angle(armPivot.rotation, goalAngle) *  (1 - (float) Math.Pow(2, -Time.deltaTime * 10)));
        
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
        if (Input.GetKeyDown("e"))
            canMove = !canMove;

        // Player and Camera rotation
        if (canMove)
        {
            inventoryUI.SetActive(false);
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            armPivot.rotation *= Quaternion.Euler(0, -Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        else {
            inventoryUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }
}