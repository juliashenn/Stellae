using UnityEngine;
using UnityEngine.InputSystem;


public interface IInteractable
{
    public void Interact();
    public bool RequireHoldingToInteract { get; }
}

public interface IPickupable
{
    public bool isHeld { get; }
    Transform prevParent { get; }
    Transform ogPosition { get; }
    public void PickUp();
}
public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public InteractorRange interactorRange;
    public float InteractRange;

    private bool grabbing = false;
    public GameObject grabbedObj;
    void Start()
    {
        
    }

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            GameObject target = GetInteractable();
            if (target != null && target.TryGetComponent(out IInteractable interactObj))
            {
                if (interactObj.RequireHoldingToInteract)
                {
                    if (target.TryGetComponent(out IPickupable pickupObj))
                    {
                        if (pickupObj.isHeld)
                        {
                            interactObj.Interact();
                        }
                        else
                        {
                            pickupObj.PickUp();
                        }
                    }
                }
                else
                {
                    interactObj.Interact();
                }
            }
        }
    }

    private GameObject GetInteractable()
    {
        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                return hitInfo.collider.gameObject;
            }
        }

        if (interactorRange != null)
        {
            return interactorRange.GetNearestInteractable();
        }
        return null;
    }
}
