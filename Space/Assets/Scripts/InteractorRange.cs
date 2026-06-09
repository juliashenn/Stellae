using System.Collections.Generic;
using UnityEngine;

public class InteractorRange : MonoBehaviour
{
    private List<GameObject> nearbyInteractables = new List<GameObject>();
    void Start()
    {
        nearbyInteractables.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactObj))
        {
            if (!nearbyInteractables.Contains(other.gameObject))
            {
                nearbyInteractables.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactObj))
        {
            if (nearbyInteractables.Contains(other.gameObject))
            {
                nearbyInteractables.Remove(other.gameObject);
            }
        }
    }

    public GameObject GetNearestInteractable()
    {
        if (nearbyInteractables  != null && nearbyInteractables.Count > 0)
            return nearbyInteractables[0];
        return null;
    }
}
