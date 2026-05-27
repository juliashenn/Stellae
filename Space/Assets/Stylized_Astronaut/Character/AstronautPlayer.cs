using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AstronautPlayer : MonoBehaviour
{
    private Animator anim;
    private CharacterController controller;
    private Transform cameraObject;

    [Header("Movement")]
    public float speed = 7.0f;
    public float turnSpeed = 10.0f;
    public float jumpForce = 10f;
    public float jumpDuration = 0.4f;
    public Transform transformBody;

    [Header("Gravity")]
    public float surfaceGravity = 1f;
    public float gravityAcceleration = 9.8f;
    [Range(0f, 1f)]
    public float stickToSurface = 0.8f;
    public float surfaceRotationSpeed = 5f;

    [Header("Ground Check")]
    public Transform groundCollider;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayerMask;

    [Header("Planet")]
    public Planet planetObj;
    public Transform planet;

    private Vector3 gravityDirection;
    private float gravityStrength = 1f;
    private Vector3 jumpVector;
    private Vector3 movementVector;
    private bool isGrounded;
    private Vector3 groundNormal;
    private bool isJumping;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        cameraObject = Camera.main.transform;
    }

    void Update()
    {
        ApplyGravity();
        CheckGround();
        RotateToSurface();
        HandleMovement();
        HandleJumpInput();

        Vector3 finalMove = movementVector + (gravityDirection * gravityStrength) + jumpVector;
        controller.Move(finalMove * Time.deltaTime);
    }

    void ApplyGravity()
    {
        gravityDirection = (planet.position - transform.position).normalized;

        if (!isGrounded)
            gravityStrength += planetObj.GetGravitationalPull * Time.deltaTime;
        else
            gravityStrength = surfaceGravity;
    }

    void CheckGround()
    {
        if (Physics.CheckSphere(groundCollider.position, groundCheckRadius, groundLayerMask))
        {
            Physics.Raycast(groundCollider.position, -transform.up, out RaycastHit hit, 5f);
            isGrounded = true;
            groundNormal = hit.normal;
            return;
        }
        isGrounded = false;
        groundNormal = -gravityDirection;
    }

    void RotateToSurface()
    {
        Quaternion gravityRotation = Quaternion.FromToRotation(transform.up, -gravityDirection) * transform.rotation;
        Quaternion surfaceRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
        Quaternion finalRotation = Quaternion.Lerp(gravityRotation, surfaceRotation, stickToSurface);
        transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, surfaceRotationSpeed * Time.deltaTime);
    }

    void HandleMovement()
    {
        Vector3 up = -gravityDirection;

        Vector3 camForward = Vector3.ProjectOnPlane(cameraObject.forward, up);
        Vector3 camRight = Vector3.ProjectOnPlane(cameraObject.right, up);

        // If camera is pointing nearly straight up/down, fall back to body's axes
        if (camForward.sqrMagnitude < 0.01f)
            camForward = Vector3.ProjectOnPlane(transformBody.forward, up);
        if (camRight.sqrMagnitude < 0.01f)
            camRight = Vector3.ProjectOnPlane(transformBody.right, up);

        camForward.Normalize();
        camRight.Normalize();

        Vector2 input = Vector2.zero;
        input.y += Keyboard.current.wKey.isPressed ? 1f : 0f;
        input.y += Keyboard.current.sKey.isPressed ? -1f : 0f;
        input.x += Keyboard.current.dKey.isPressed ? 1f : 0f;
        input.x += Keyboard.current.aKey.isPressed ? -1f : 0f;
        input = Vector2.ClampMagnitude(input, 1f);

        anim.SetInteger("AnimationPar", isGrounded && input != Vector2.zero ? 1 : 0);

        if (isGrounded)
        {
            movementVector = (camForward * input.y + camRight * input.x) * speed;

            if (movementVector.sqrMagnitude > 0.01f)
            {
                Quaternion targetBodyRot = Quaternion.LookRotation(movementVector.normalized, up);
                transformBody.rotation = Quaternion.Slerp(transformBody.rotation, targetBodyRot, turnSpeed * Time.deltaTime);
            }
        }
        else
        {
            movementVector = Vector3.zero;
        }
    }

    void HandleJumpInput()
    {
        if (isGrounded && !isJumping && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(ApplyJump());
            anim.CrossFade("Jump_start", 0.2f);
        }
    }

    private IEnumerator ApplyJump()
    {
        isJumping = true;
        gravityStrength = 0f;
        jumpVector = Vector3.zero;
        float force = jumpForce;
        float t = 0f;
        while (t < jumpDuration)
        {
            jumpVector = -gravityDirection * force;
            force = Mathf.Lerp(jumpForce, 0f, t / jumpDuration);
            t += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        jumpVector = Vector3.zero;
        isJumping = false;
    }
}