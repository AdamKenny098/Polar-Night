using UnityEngine;

public class BaitPlacementSystem : MonoBehaviour
{
    public static BaitPlacementSystem Instance;

    [Header("Input")]
    public KeyCode placementKey = KeyCode.B;
    public KeyCode cancelKey = KeyCode.Escape;
    public KeyCode placeKey = KeyCode.Mouse0;

    [Header("Bait Item")]
    public Item baitItem;

    [Header("Prefabs")]
    public GameObject placedBaitPrefab;
    public GameObject previewBaitPrefab;

    [Header("References")]
    public Inventory playerInventory;
    public Camera playerCamera;
    public Transform playerRoot;

    [Header("Placement")]
    public float placementRange = 4f;
    public LayerMask placementMask = ~0;
    public float surfaceOffset = 0.05f;
    public bool requireBaitItem = true;

    [Header("Validation")]
    public float maxSurfaceAngle = 35f;
    public float collisionCheckRadius = 0.35f;
    public LayerMask blockingMask = ~0;

    [Header("Debug")]
    public bool debugPlacement;

    private GameObject currentPreview;
    private bool isPlacing;
    private bool currentPlacementValid;
    private Vector3 currentPlacementPosition;
    private Quaternion currentPlacementRotation;

    private Material validPreviewMaterial;
    private Material invalidPreviewMaterial;

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

        CreatePreviewMaterials();
    }

    private void Start()
    {
        ResolveReferences();
    }

    private void Update()
    {
        if (Input.GetKeyDown(placementKey))
        {
            TogglePlacementMode();
        }

        if (!isPlacing)
        {
            return;
        }

        UpdatePreview();

        if (Input.GetKeyDown(cancelKey))
        {
            StopPlacementMode();
            return;
        }

        if (Input.GetKeyDown(placeKey))
        {
            TryPlaceBait();
        }
    }

    public void TogglePlacementMode()
    {
        if (isPlacing)
        {
            StopPlacementMode();
        }
        else
        {
            StartPlacementMode();
        }
    }

    public void StartPlacementMode()
    {
        ResolveReferences();

        if (!CanStartPlacement())
        {
            return;
        }

        isPlacing = true;

        if (!currentPreview)
        {
            GameObject prefabToUse = previewBaitPrefab ? previewBaitPrefab : placedBaitPrefab;

            if (!prefabToUse)
            {
                Debug.LogWarning("[BaitPlacementSystem] No bait preview or placed bait prefab assigned.");
                isPlacing = false;
                return;
            }

            currentPreview = Instantiate(prefabToUse);
            DisablePreviewColliders(currentPreview);
        }

        currentPreview.SetActive(true);

        InteractSystem.SetInputLocked(true);
    }

    public void StopPlacementMode()
    {
        isPlacing = false;
        currentPlacementValid = false;

        if (currentPreview)
        {
            currentPreview.SetActive(false);
        }

        InteractSystem.SetInputLocked(false, 0.2f);
    }

    private bool CanStartPlacement()
    {
        if (!placedBaitPrefab)
        {
            Debug.LogWarning("[BaitPlacementSystem] Cannot place bait. No placedBaitPrefab assigned.");
            return false;
        }

        if (!requireBaitItem)
        {
            return true;
        }

        if (!baitItem)
        {
            Debug.LogWarning("[BaitPlacementSystem] Cannot place bait. No baitItem assigned.");
            return false;
        }

        if (!playerInventory)
        {
            Debug.LogWarning("[BaitPlacementSystem] Cannot place bait. No player inventory found.");
            return false;
        }

        if (!playerInventory.HasItem(baitItem, 1))
        {
            Debug.Log("[BaitPlacementSystem] Cannot place bait. Player has no Entity Bait.");
            return false;
        }

        return true;
    }

    private void UpdatePreview()
    {
        if (!currentPreview)
        {
            return;
        }

        currentPlacementValid = CalculatePlacement(out currentPlacementPosition, out currentPlacementRotation);

        currentPreview.transform.SetPositionAndRotation(currentPlacementPosition, currentPlacementRotation);
        ApplyPreviewMaterial(currentPlacementValid);
    }

    private bool CalculatePlacement(out Vector3 position, out Quaternion rotation)
    {
        position = Vector3.zero;
        rotation = Quaternion.identity;

        if (!playerCamera)
        {
            return false;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, placementRange, placementMask, QueryTriggerInteraction.Ignore))
        {
            position = playerCamera.transform.position + playerCamera.transform.forward * placementRange;
            rotation = Quaternion.identity;
            return false;
        }

        Collider placementSurface = hit.collider;

        position = hit.point + hit.normal * surfaceOffset;

        Vector3 flatForward = Vector3.ProjectOnPlane(playerCamera.transform.forward, hit.normal);

        if (flatForward.sqrMagnitude < 0.01f)
        {
            flatForward = Vector3.ProjectOnPlane(transform.forward, hit.normal);
        }

        rotation = Quaternion.LookRotation(flatForward.normalized, hit.normal);

        float surfaceAngle = Vector3.Angle(hit.normal, Vector3.up);

        if (surfaceAngle > maxSurfaceAngle)
        {
            if (debugPlacement)
            {
                Debug.Log($"[BaitPlacementSystem] Invalid surface angle: {surfaceAngle}");
            }

            return false;
        }

        Collider[] blockingHits = Physics.OverlapSphere(
            position,
            collisionCheckRadius,
            blockingMask,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < blockingHits.Length; i++)
        {
            Collider blockingCollider = blockingHits[i];

            if (!blockingCollider)
            {
                continue;
            }

            if (blockingCollider == placementSurface)
            {
                continue;
            }

            if (currentPreview && blockingCollider.transform.IsChildOf(currentPreview.transform))
            {
                continue;
            }

            if (playerRoot && blockingCollider.transform.IsChildOf(playerRoot))
            {
                continue;
            }

            if (blockingCollider.GetComponentInParent<BaitPlacementSystem>())
            {
                continue;
            }

            if (debugPlacement)
            {
                Debug.Log($"[BaitPlacementSystem] Placement blocked by {blockingCollider.name}");
            }

            return false;
        }

        return true;
    }

    private void TryPlaceBait()
    {
        if (!currentPlacementValid)
        {
            Debug.Log("[BaitPlacementSystem] Cannot place bait here.");
            return;
        }

        if (requireBaitItem && !playerInventory.TryRemoveItem(baitItem, 1))
        {
            Debug.Log("[BaitPlacementSystem] Failed to remove bait item from inventory.");
            StopPlacementMode();
            return;
        }

        GameObject placedBait = Instantiate(placedBaitPrefab, currentPlacementPosition, currentPlacementRotation);
        placedBait.name = "Placed Entity Bait";

        if (ItemPickUpUI.Instance && baitItem)
        {
            ItemPickUpUI.Instance.ShowMessage("-1 " + baitItem.Name, baitItem.icon);
        }

        StopPlacementMode();

        Debug.Log("[BaitPlacementSystem] Entity bait placed.");
    }

    private void ResolveReferences()
    {
        if (!playerCamera)
        {
            playerCamera = Camera.main;
        }

        GameObject player = GameObject.Find("Player");

        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player)
        {
            playerRoot = player.transform;

            if (!playerInventory)
            {
                playerInventory = player.GetComponentInChildren<Inventory>();
            }
        }
    }

    private void DisablePreviewColliders(GameObject preview)
    {
        Collider[] colliders = preview.GetComponentsInChildren<Collider>();

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }
    }

    private void ApplyPreviewMaterial(bool valid)
    {
        if (!currentPreview)
        {
            return;
        }

        Renderer[] renderers = currentPreview.GetComponentsInChildren<Renderer>();
        Material material = valid ? validPreviewMaterial : invalidPreviewMaterial;

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sharedMaterial = material;
        }
    }

    private void CreatePreviewMaterials()
    {
        Shader shader = GetCompatibleShader();

        validPreviewMaterial = new Material(shader);
        SetMaterialColor(validPreviewMaterial, new Color(0.1f, 0.8f, 0.25f, 0.45f));

        invalidPreviewMaterial = new Material(shader);
        SetMaterialColor(invalidPreviewMaterial, new Color(0.9f, 0.1f, 0.1f, 0.45f));
    }

    private Shader GetCompatibleShader()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");

        if (shader)
        {
            return shader;
        }

        shader = Shader.Find("HDRP/Lit");

        if (shader)
        {
            return shader;
        }

        shader = Shader.Find("Standard");

        if (shader)
        {
            return shader;
        }

        return Shader.Find("Sprites/Default");
    }

    private void SetMaterialColor(Material material, Color color)
    {
        if (!material)
        {
            return;
        }

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
            return;
        }

        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }
    }
}