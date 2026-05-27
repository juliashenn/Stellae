using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float rayCastLength;
    public float rotationSpeed;
    private float tempRotationSpeed;
    public float speed;
    public float gravity;
    private float tempGravity;
    public float jumpForce;
    private Rigidbody rb;
    public Transform currentPlanet;
    public Transform playerVisual;

    private RaycastHit[] hits;
    private Vector3 planetDir;
    private Vector3 normalDir;
    private Vector3 input;

    public bool isTouchingPlanet = false;
    private Transform MainCameraTransform;
    public Transform CameraArmTransform;
    private Animator anim;

    PlayerControls playerControls;

    private bool canJump = true;
    private bool slowDown = false;
    private bool inSpace = false;

    private Quaternion targetSpaceTilt = Quaternion.identity;
    [SerializeField] private float spaceTiltAngle = 20f;
    [SerializeField] private float spaceTiltSpeed = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        MainCameraTransform = Camera.main.transform;
        anim = GetComponent<Animator>();
        tempGravity = gravity;
        tempRotationSpeed = rotationSpeed;

        playerControls = new PlayerControls();
        playerControls.PlayerMovement.Jump.performed += Jump;
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
    private void FixedUpdate()
    {
        HandleMovement();
        HandleGravity();
        HandleRotation();
    }

    private void Jump (InputAction.CallbackContext context)
    {
        if (!canJump)
            return;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(normalDir*jumpForce, ForceMode.Impulse);
        gravity = tempGravity / 2f;
        Invoke(nameof(RestoreGravity), 1f); // after one sec from prev line, restore gravity to normal
        canJump = false;
        rotationSpeed = tempRotationSpeed / 2f;
    }
    private void RestoreGravity()
    {
        gravity = tempGravity;
        canJump = true;
        slowDown = false;
    }

    private void UpdateInput()
    {
        input = Vector3.zero;
        input.z += Keyboard.current.wKey.isPressed ? 1f : 0f;
        input.z += Keyboard.current.sKey.isPressed ? -1f : 0f;
        input.x += Keyboard.current.dKey.isPressed ? 1f : 0f;
        input.x += Keyboard.current.aKey.isPressed ? -1f : 0f;
    }

    private void HandleMovement()
    {
        UpdateInput();
        Vector3 cameraRot = new Vector3(0, MainCameraTransform.localEulerAngles.y + CameraArmTransform.localEulerAngles.y, 0);
        Vector3 movementDir;
        Vector3 dir = Quaternion.Euler(cameraRot) * input;
        if (inSpace)
        {
            Vector3 cameraForward = MainCameraTransform.forward;
            Vector3 cameraRight = MainCameraTransform.right;
            movementDir = (cameraForward * input.z + cameraRight * input.x).normalized;
            //dir = movementDir;
        }
        else
        {
            //dir = Quaternion.Euler(cameraRot) * input;
            movementDir = (transform.forward * dir.z + transform.right * dir.x);
        }
            
        Vector3 currNormalVelocity = Vector3.Project(rb.linearVelocity, normalDir.normalized); // adds back in some gravity? 
        rb.linearVelocity = currNormalVelocity + (movementDir * speed);

        if (movementDir != Vector3.zero)
        {
            if (inSpace)
            {
                anim.SetInteger("AnimationPar", 2);
            }
            else
            {
                anim.SetInteger("AnimationPar", 1);
            }
            playerVisual.localRotation = Quaternion.LookRotation(dir);
        }
        else
        {
            anim.SetInteger("AnimationPar", 0);
        }
        if (slowDown)
            rb.linearVelocity *= 0.5f;
    }

    private void HandleGravity()
    {
        if (currentPlanet == null || inSpace) return; // need to check if this means i can swim around in space or not move

        Vector3[] directions = { -transform.up, transform.forward, -transform.forward, transform.right, -transform.right };
        hits = new RaycastHit[0];
        for (int i = 0; hits.Length == 0 && i < directions.Length; i++)
        {
            hits = Physics.RaycastAll(transform.position, directions[i], rayCastLength);
        }

        if (hits.Length == 0)
        {
            planetDir = currentPlanet.position - transform.position;
            hits = Physics.RaycastAll(transform.position, planetDir, rayCastLength);
        }

        GetPlanetNormal();
        rb.AddForce(normalDir.normalized * gravity, ForceMode.Acceleration);
        hits = new RaycastHit[0];

    }

    private void GetPlanetNormal()
    {
        if (currentPlanet == null) return;
        normalDir = (transform.position - currentPlanet.position).normalized;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform == currentPlanet)
            {
                normalDir = hits[i].normal.normalized;
                break;
            }
        }
        return;
    }

    private void HandleRotation()
    {
        Quaternion targetRot = Quaternion.FromToRotation(transform.up, normalDir) * transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
        if (isTouchingPlanet && canJump)
            rotationSpeed = tempRotationSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == currentPlanet)
        {
            isTouchingPlanet = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.transform == currentPlanet)
            isTouchingPlanet = false;
    }

    public void EnterNewGravityField()
    {
        inSpace = false;
        gravity = tempGravity / 4f; // slow sown when switching
        rb.linearVelocity *= 0.5f;
        rotationSpeed = tempRotationSpeed / 10f;
        slowDown = true;
        canJump = false;
        GetPlanetNormal();
        Invoke(nameof(RestoreGravity), .5f);
    }

    public void EnterSpace()
    {
        inSpace = true;
        gravity = 0;
        normalDir = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rotationSpeed = 0f;
        canJump = false;
    }
}
