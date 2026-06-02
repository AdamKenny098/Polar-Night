using UnityEngine;

public class PlacedEntityBait : MonoBehaviour
{
    [Header("Bait State")]
    public bool isActive = true;
    public bool hasBeenConsumed;

    [Header("Attraction")]
    public float attractionRadius = 18f;
    public Transform attractionPoint;

    [Header("Consume Behaviour")]
    public bool destroyOnConsume = true;
    public float destroyDelay = 0.1f;
    public GameObject visualRoot;

    [Header("Debug")]
    public bool debugBait;

    private void Awake()
    {
        if (!attractionPoint)
        {
            attractionPoint = transform;
        }

        if (!visualRoot)
        {
            visualRoot = gameObject;
        }
    }

    private void OnEnable()
    {
        EntityBaitRegistry.Register(this);
    }

    private void OnDisable()
    {
        EntityBaitRegistry.Unregister(this);
    }

    public Vector3 GetAttractionPosition()
    {
        if (attractionPoint)
        {
            return attractionPoint.position;
        }

        return transform.position;
    }

    public bool CanAttractEntity()
    {
        return isActive && !hasBeenConsumed;
    }

    public void ConsumeBait()
    {
        if (hasBeenConsumed)
        {
            return;
        }

        hasBeenConsumed = true;
        isActive = false;

        EntityBaitRegistry.Unregister(this);

        if (debugBait)
        {
            Debug.Log($"[PlacedEntityBait] {name} consumed.");
        }

        if (ItemPickUpUI.Instance)
        {
            ItemPickUpUI.Instance.ShowMessage("Bait Consumed", null);
        }

        if (destroyOnConsume)
        {
            Destroy(gameObject, destroyDelay);
            return;
        }

        if (visualRoot)
        {
            visualRoot.SetActive(false);
        }
    }
}