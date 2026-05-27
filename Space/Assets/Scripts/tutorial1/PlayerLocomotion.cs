using System.Security.Cryptography;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    InputManager inputManager;

    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody rb;

    public float movementSpeed = 7.0f;
    public float rotationSpeed = 15.0f;

    public void Awake()
    {
        inputManager = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
    }
    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * inputManager.verticalInput; //move forward
        moveDirection += cameraObject.right * inputManager.horizontalInput;  // move side to side
        moveDirection.Normalize();

        moveDirection.y = 0; // no movement up into the sky or down into ground

        moveDirection *= movementSpeed;

        Vector3 movementVelocity = moveDirection;
        rb.linearVelocity = movementVelocity;
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;
        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection += cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection); // where we are looking is where we wanna rotate
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // multiply with deltatime to make the speed the same no matter the frame rate
        
        transform.rotation = playerRotation;
    }
}
