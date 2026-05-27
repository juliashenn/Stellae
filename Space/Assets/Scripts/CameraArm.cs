using UnityEngine;
using UnityEngine.InputSystem;

public class CameraArm : MonoBehaviour
{
    public float verticalMax = 60f;
    public float verticalMin = -20f;
    public Vector2 sensitivity = Vector2.one;
    public PlayerController controller;
    PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        AdjustRange();
        AdjustCamera();
    }

    private void AdjustRange()
    {
        if (controller != null)
        {
            if (controller.currentPlanet != null)
            {
                verticalMax = 60f;
                verticalMin = -20f;
            }
            else
            {
                verticalMax = 75f;
                verticalMin = -75f;
            }
        }
    }

    private void Start()
    {
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    private void AdjustCamera()
    {
        Vector2 input = Mouse.current.delta.ReadValue();
        input *= sensitivity;
        transform.localRotation = Quaternion.Euler(new Vector3(input.y, input.x * -1f, 0) + transform.localRotation.eulerAngles);

        float clampedX = 0;

        if (transform.localRotation.eulerAngles.x < 180)
            clampedX = Mathf.Clamp(transform.localRotation.eulerAngles.x, verticalMin, verticalMax);
        else
            clampedX = Mathf.Clamp(transform.localRotation.eulerAngles.x, 360f + verticalMin, 360f + verticalMax);
        transform.localRotation = Quaternion.Euler(new Vector3(clampedX, transform.localRotation.eulerAngles.y, 0));
    }
}
