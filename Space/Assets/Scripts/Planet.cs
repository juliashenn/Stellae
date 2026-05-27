using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField] private float gravitationalPull;

    public float GetGravitationalPull { get =>  gravitationalPull; set { gravitationalPull = value; } }

    //private void OnTriggerEnter(Collider other)
    //{
    //    PlayerController playerController = other.GetComponent<PlayerController>();
    //    if (playerController != null )
    //    {
    //        if (playerController.currentPlanet == transform) return;
    //        playerController.currentPlanet = transform;
    //        playerController.EnterNewGravityField();
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    PlayerController playerController = other.GetComponent<PlayerController>();
    //    if (playerController != null)
    //    {
    //        if (playerController.currentPlanet == transform)
    //        {
    //            Debug.Log("space now");
    //            playerController.currentPlanet = null;
    //            playerController.EnterSpace();
    //        }
    //    }
    //}

}
