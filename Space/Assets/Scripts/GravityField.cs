using UnityEngine;

public class GravityField : MonoBehaviour
{
    //[SerializeField] private Planet planet;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null)
        {
            if (playerController.currentPlanet == transform) return;
            playerController.currentPlanet = transform;
            playerController.EnterNewGravityField();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null)
        {
            if (playerController.currentPlanet == transform)
            {
                Debug.Log("space now");
                playerController.currentPlanet = null;
                playerController.EnterSpace();
            }
        }
    }
}