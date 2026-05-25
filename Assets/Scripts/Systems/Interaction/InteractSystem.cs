// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Handles player interaction with objects in the scene using raycasting and an interaction icon.

using UnityEngine;
using UnityEngine.UI;

public interface IInteractable
{
    void Interact();
}

public class InteractSystem : MonoBehaviour
{
    public static InteractSystem Instance;

    [Header("Raycast")]
    public Transform rayOrigin;
    public float interactRange = 5f;
    public Image interactIcon;
    public LayerMask interactionMask = ~0;

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;
    public float interactionCooldown = 0.2f;

    [Header("Debug")]
    public bool debugInteraction = true;
    public bool drawDebugRay = true;
    public Color debugRayHitColor = Color.green;
    public Color debugRayMissColor = Color.red;

    public IInteractable currentInteractable;

    private static bool inputLocked;
    private static float lockedUntilTime;

    private float nextAllowedInteractionTime;
    private RaycastHit lastHit;
    private bool hasLastHit;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        ClearInteractionState();
    }

    private void Update()
    {
        if (drawDebugRay && rayOrigin)
        {
            Color rayColor = hasLastHit ? debugRayHitColor : debugRayMissColor;
            Debug.DrawRay(rayOrigin.position, rayOrigin.forward * interactRange, rayColor);
        }

        if (Input.GetKeyDown(interactKey))
        {
            DebugInteractionPress();
        }

        if (IsInputLocked())
        {
            ClearInteractionState();
            return;
        }

        ScanForInteractable();

        if (currentInteractable == null)
        {
            return;
        }

        if (Time.unscaledTime < nextAllowedInteractionTime)
        {
            return;
        }

        if (Input.GetKeyDown(interactKey))
        {
            nextAllowedInteractionTime = Time.unscaledTime + interactionCooldown;
            currentInteractable.Interact();
        }
    }

    public static void SetInputLocked(bool locked, float cooldownAfterUnlock = 0.2f)
    {
        inputLocked = locked;

        if (!locked)
        {
            lockedUntilTime = Time.unscaledTime + Mathf.Max(0f, cooldownAfterUnlock);
        }

        if (Instance)
        {
            Instance.enabled = true;
            Instance.ClearInteractionState();
        }

        Debug.Log($"[InteractSystem] Input locked set to {locked}. Cooldown after unlock: {cooldownAfterUnlock}");
    }

    public static void LockForSeconds(float seconds)
    {
        inputLocked = false;
        lockedUntilTime = Time.unscaledTime + Mathf.Max(0f, seconds);

        if (Instance)
        {
            Instance.enabled = true;
            Instance.ClearInteractionState();
        }

        Debug.Log($"[InteractSystem] Interaction cooldown set for {seconds} seconds.");
    }

    public static bool IsInputLocked()
    {
        if (inputLocked)
        {
            return true;
        }

        return Time.unscaledTime < lockedUntilTime;
    }

    private void ScanForInteractable()
    {
        ClearInteractionState();

        if (!rayOrigin)
        {
            return;
        }

        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, interactRange, interactionMask, QueryTriggerInteraction.Ignore))
        {
            hasLastHit = false;
            return;
        }

        hasLastHit = true;
        lastHit = hit;

        IInteractable interactable = FindInteractable(hit.collider);

        if (interactable == null)
        {
            return;
        }

        currentInteractable = interactable;

        if (interactIcon)
        {
            interactIcon.enabled = true;
        }
    }

    private IInteractable FindInteractable(Collider hitCollider)
    {
        if (!hitCollider)
        {
            return null;
        }

        IInteractable interactable = hitCollider.GetComponent<IInteractable>();

        if (interactable != null)
        {
            return interactable;
        }

        interactable = hitCollider.GetComponentInParent<IInteractable>();

        if (interactable != null)
        {
            return interactable;
        }

        interactable = hitCollider.GetComponentInChildren<IInteractable>();

        return interactable;
    }

    private void DebugInteractionPress()
    {
        if (!debugInteraction)
        {
            return;
        }

        Debug.Log(
            $"[InteractSystem] E pressed. " +
            $"enabled={enabled}, " +
            $"inputLocked={inputLocked}, " +
            $"lockedUntilTime={lockedUntilTime:F2}, " +
            $"time={Time.unscaledTime:F2}, " +
            $"isInputLocked={IsInputLocked()}, " +
            $"currentInteractable={(currentInteractable != null ? currentInteractable.GetType().Name : "null")}"
        );

        if (!rayOrigin)
        {
            Debug.LogWarning("[InteractSystem] No rayOrigin assigned.");
            return;
        }

        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, interactRange, interactionMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log(
                $"[InteractSystem] Ray MISS. " +
                $"origin={rayOrigin.position}, " +
                $"direction={rayOrigin.forward}, " +
                $"range={interactRange}"
            );
            return;
        }

        GameObject hitObject = hit.collider.gameObject;
        IInteractable directInteractable = hit.collider.GetComponent<IInteractable>();
        IInteractable parentInteractable = hit.collider.GetComponentInParent<IInteractable>();
        IInteractable childInteractable = hit.collider.GetComponentInChildren<IInteractable>();

        Debug.Log(
            $"[InteractSystem] Ray HIT. " +
            $"hitObject='{hitObject.name}', " +
            $"collider='{hit.collider.GetType().Name}', " +
            $"root='{hitObject.transform.root.name}', " +
            $"tag='{hitObject.tag}', " +
            $"layer='{LayerMask.LayerToName(hitObject.layer)}', " +
            $"distance={hit.distance:F2}, " +
            $"directInteractable={(directInteractable != null ? directInteractable.GetType().Name : "null")}, " +
            $"parentInteractable={(parentInteractable != null ? parentInteractable.GetType().Name : "null")}, " +
            $"childInteractable={(childInteractable != null ? childInteractable.GetType().Name : "null")}"
        );

        if (parentInteractable == null && directInteractable == null && childInteractable == null)
        {
            Debug.LogWarning(
                $"[InteractSystem] Hit '{hitObject.name}' but found no IInteractable on hit object, parent, or children."
            );
        }
    }

    private void ClearInteractionState()
    {
        currentInteractable = null;

        if (interactIcon)
        {
            interactIcon.enabled = false;
        }
    }
}