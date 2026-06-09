using System.Collections;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class FrierenStaff : MonoBehaviour, IInteractable, IPickupable
{
    [Header("Interaction Objects")]
    public Animator flowerAnimator;
    public GameObject flowers;
    public Animator crownAnim;
    public GameObject flowersCrown;

    [Header("Settings")]
    public Transform prevParent { get; private set; }
    public Transform ogPosition { get; private set; }

    private PlayerController playerController;
    private PlanetManager planetManager;
    private Rigidbody rb;
    public bool held = false;
    public bool isHeld { get; private set; }
    public bool RequireHoldingToInteract { get; private set; }

    public void Awake()
    {
        prevParent = transform.parent;
        ogPosition = transform;
        rb = GetComponent<Rigidbody>();

        RequireHoldingToInteract = true;
        isHeld = false;
        flowers.SetActive(false);
        flowersCrown.SetActive(false);
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        planetManager = GameObject.FindGameObjectWithTag("PlanetManager").GetComponent<PlanetManager>();
    }
    

    public void Interact()
    {
        if (!RequireHoldingToInteract || (RequireHoldingToInteract && isHeld))
        {
            Debug.Log("interacting");
            if (flowerAnimator != null && flowers != null)
            {
                flowerAnimator.SetBool("Bloom", true);
                flowers.SetActive(true);
            }

            if (crownAnim != null && flowersCrown != null)
            {
                flowersCrown.SetActive(true);
                crownAnim.SetBool("Drop", true);
            }
        }
    }

    public IEnumerator wait()
    {
        yield return new WaitForSeconds(1f);
    }

    public void PickUp()
    {
        if (isHeld)
        {
            Drop();
            return;
        }
        Debug.Log("picking up");
        isHeld = true;
        held = true;
        if (playerController.GrabPoint != null)
        {
            transform.SetParent(playerController.GrabPoint, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
            
    }

    public void Drop()
    {
        isHeld = false;
        if (prevParent != null)
        {
            Debug.Log("dropping");
            transform.SetParent(prevParent, false);
        }
    }

    private void FixedUpdate()
    {
        if (!isHeld && playerController != null)
        {
            planetManager.HandleObjGravity(transform);
        }
        else
        {
            rb.isKinematic = true;
        }
    }
}
