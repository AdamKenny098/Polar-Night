using UnityEngine;

public class TrapPlacementSystem : MonoBehaviour
{
    public static TrapPlacementSystem Instance;

    [Header("Input")]
    public KeyCode placementKey = KeyCode.T;
    public KeyCode cancelKey = KeyCode.Escape;
    public KeyCode placeKey = KeyCode.Mouse0;

    [Header("Trap Item")]
    public Item trapItem;

    [Header("Prefabs")]
    public GameObject placedTrapPrefab;
    public GameObject previewTrapPrefab;

    [Header("References")]
    public Inventory playerInventory;
    public Camera playerCamera;
    public Transform playerRoot;

    [Header("Placement")]
    public float placementRange = 4f;
    public LayerMask placementMask = ~0;
    public float surfaceOffset = 0.05f;
    public bool requireTrapItem = true;

    [Header("Validation")]
    public float maxSurfaceAngle = 35f;
    public float collisionCheckRadius = 0.45f;
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
            TryPlaceTrap();
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
            GameObject prefabToUse = previewTrapPrefab ? previewTrapPrefab : placedTrapPrefab;

            if (!prefabToUse)
            {
                Debug.LogWarning("[TrapPlacementSystem] No trap preview or placed trap prefab assigned.");
                isPlacing = false;
                return;
            }

            currentPreview = Instantiate(prefabToUse);
            DisablePreviewColliders(currentPreview);
        }

        currentPreview.SetActive(true);

        if (InteractSystem.Instance)
        {
            InteractSystem.SetInputLocked(true);
        }
    }

    public void StopPlacementMode()
    {
        isPlacing = false;
        currentPlacementValid = false;

        if (currentPreview)
        {
            currentPreview.SetActive(false);
        }

        if (InteractSystem.Instance)
        {
            InteractSystem.SetInputLocked(false, 0.2f);
        }
    }

    private bool CanStartPlacement()
    {
        if (!placedTrapPrefab)
        {
            Debug.LogWarning("[TrapPlacementSystem] Cannot place trap. No placedTrapPrefab assigned.");
            return false;
        }

        if (requireTrapItem)
        {
            if (!trapItem)
            {
                Debug.LogWarning("[TrapPlacementSystem] Cannot place trap. No trapItem assigned.");
                return false;
            }

            if (!playerInventory)
            {
                Debug.LogWarning("[TrapPlacementSystem] Cannot place trap. No player inventory found.");
                return false;
            }

            if (!playerInventory.HasItem(trapItem, 1))
            {
                Debug.Log("[TrapPlacementSystem] Cannot place trap. Player has no Basic Snow Trap.");
                return false;
            }
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
                Debug.Log($"[TrapPlacementSystem] Invalid surface angle: {surfaceAngle}");
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

            if (blockingCollider.GetComponentInParent<TrapPlacementSystem>())
            {
                continue;
            }

            if (debugPlacement)
            {
                Debug.Log($"[TrapPlacementSystem] Placement blocked by {blockingCollider.name}");
            }

            return false;
        }

        return true;
    }

    private void TryPlaceTrap()
    {
        if (!currentPlacementValid)
        {
            Debug.Log("[TrapPlacementSystem] Cannot place trap here.");
            return;
        }

        if (requireTrapItem && !playerInventory.TryRemoveItem(trapItem, 1))
        {
            Debug.Log("[TrapPlacementSystem] Failed to remove trap item from inventory.");
            StopPlacementMode();
            return;
        }

        GameObject placedTrap = Instantiate(placedTrapPrefab, currentPlacementPosition, currentPlacementRotation);
        placedTrap.name = "Placed Snow Trap";

        if (ItemPickUpUI.Instance && trapItem)
        {
            ItemPickUpUI.Instance.ShowMessage("-1 " + trapItem.Name, trapItem.icon);
        }

        StopPlacementMode();

        Debug.Log("[TrapPlacementSystem] Trap placed.");
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