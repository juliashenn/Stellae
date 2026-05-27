using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AstronautThirdPersonCamera
{

  public class AstronautThirdPersonCamera : MonoBehaviour
  {
        public AstronautPlayer playerScript;
        private const float Y_ANGLE_MIN = -50.0f;
        private const float Y_ANGLE_MAX = 80.0f;

        public Transform lookAt;
        public Transform camTransform;
        public float distance = 5.0f;

        private float currentX = 0.0f;
        private float currentY = 45.0f;
        private float sensitivityX = 20.0f;
        private float sensitivityY = 20.0f;

        private Quaternion currentOrientation;
        private Vector2 mouseDelta;
        private void Start()
        {
            camTransform = transform;

            currentX = 0f;
            currentY = 100f;
            currentOrientation = Quaternion.identity;
        }

    private void Update()
    {
            mouseDelta = Mouse.current.delta.ReadValue();

            currentX += mouseDelta.x * sensitivityX * Time.deltaTime;
            currentY -= mouseDelta.y * sensitivityY * Time.deltaTime;

            currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
        }

    private void LateUpdate()
    {
            Vector3 planetUp = (lookAt.position - playerScript.planet.position).normalized;

            // Incrementally rotate the orientation each frame rather than rebuilding from scratch
            // This avoids the singularity from recomputing yaw axis from world vectors every frame
            Quaternion yawDelta = Quaternion.AngleAxis(mouseDelta.x * sensitivityX * Time.deltaTime, planetUp);
            Quaternion pitchDelta = Quaternion.AngleAxis(-mouseDelta.y * sensitivityY * Time.deltaTime, currentOrientation * Vector3.right);

            currentOrientation = yawDelta * pitchDelta * currentOrientation;

            // Extract and clamp pitch to prevent flipping
            float pitch = Vector3.SignedAngle(
                Vector3.ProjectOnPlane(currentOrientation * Vector3.forward, planetUp),
                currentOrientation * Vector3.forward,
                currentOrientation * Vector3.right
            );
            if (pitch < Y_ANGLE_MIN || pitch > Y_ANGLE_MAX)
            {
                float clampedPitch = Mathf.Clamp(pitch, Y_ANGLE_MIN, Y_ANGLE_MAX);
                Quaternion pitchCorrection = Quaternion.AngleAxis(clampedPitch - pitch, currentOrientation * Vector3.right);
                currentOrientation = pitchCorrection * currentOrientation;
            }

            camTransform.position = lookAt.position + currentOrientation * (planetUp * distance);
            camTransform.rotation = Quaternion.LookRotation(
                lookAt.position - camTransform.position,
                planetUp
            );
        }
  }
}
