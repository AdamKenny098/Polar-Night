using System.Collections;
using UnityEngine;

public enum TrapState
{
    Placed,
    Armed,
    Triggered,
    Captured,
    Failed
}

public class PlacedSnowTrap : MonoBehaviour
{
    [Header("Trap State")]
    public TrapState currentState = TrapState.Placed;

    [Header("Setup")]
    public bool armOnStart = true;
    public float captureDelay = 1.25f;

    [Header("Detection")]
    public bool requireCaptureTarget = true;
    public string validTargetTag = "";
    public LayerMask validTargetLayers = ~0;

    [Header("Visuals")]
    public Renderer[] renderers;
    public Material armedMaterial;
    public Material triggeredMaterial;
    public Material capturedMaterial;
    public Material failedMaterial;

    [Header("Feedback")]
    public bool showMessages = true;
    public Sprite trapIcon;

    [Header("Debug")]
    public bool debugTrap = true;

    private TrapCaptureTarget currentTarget;
    private Coroutine captureRoutine;

    private void Awake()
    {
        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>();
        }
    }

    private void Start()
    {
        if (armOnStart)
        {
            ArmTrap();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (debugTrap)
        {
            Debug.Log($"[PlacedSnowTrap] Trigger entered by {other.name}. State={currentState}");
        }

        if (currentState != TrapState.Armed)
        {
            return;
        }

        TrapCaptureTarget target = FindCaptureTarget(other);

        if (!IsValidTarget(other, target))
        {
            if (debugTrap)
            {
                Debug.Log($"[PlacedSnowTrap] Ignored {other.name}. Not a valid capture target.");
            }

            return;
        }

        TriggerTrap(target);
    }

    public void ArmTrap()
    {
        currentState = TrapState.Armed;
        ApplyMaterial(armedMaterial);

        if (debugTrap)
        {
            Debug.Log($"[PlacedSnowTrap] {name} armed.");
        }
    }

    public void TriggerTrap(TrapCaptureTarget target)
    {
        if (currentState != TrapState.Armed)
        {
            return;
        }

        if (!target || !target.CanBeCaptured())
        {
            return;
        }

        currentTarget = target;
        currentState = TrapState.Triggered;
        ApplyMaterial(triggeredMaterial);
        ShowMessage("Trap Triggered");

        if (debugTrap)
        {
            Debug.Log($"[PlacedSnowTrap] {name} triggered by {target.name}.");
        }

        if (captureRoutine != null)
        {
            StopCoroutine(captureRoutine);
        }

        captureRoutine = StartCoroutine(CaptureAfterDelay());
    }

    public void MarkCaptured()
    {
        currentState = TrapState.Captured;
        ApplyMaterial(capturedMaterial);
        ShowMessage("Target Captured");

        if (debugTrap)
        {
            Debug.Log($"[PlacedSnowTrap] {name} captured target.");
        }
    }

    public void MarkFailed()
    {
        currentState = TrapState.Failed;
        ApplyMaterial(failedMaterial);
        ShowMessage("Trap Failed");

        if (debugTrap)
        {
            Debug.Log($"[PlacedSnowTrap] {name} failed.");
        }
    }

    private IEnumerator CaptureAfterDelay()
    {
        yield return new WaitForSeconds(captureDelay);

        if (!currentTarget || !currentTarget.CanBeCaptured())
        {
            MarkFailed();
            yield break;
        }

        currentTarget.Capture(this);
        MarkCaptured();
    }

    private TrapCaptureTarget FindCaptureTarget(Collider other)
    {
        TrapCaptureTarget target = other.GetComponent<TrapCaptureTarget>();

        if (target)
        {
            return target;
        }

        target = other.GetComponentInParent<TrapCaptureTarget>();

        if (target)
        {
            return target;
        }

        return other.GetComponentInChildren<TrapCaptureTarget>();
    }

    private bool IsValidTarget(Collider other, TrapCaptureTarget target)
    {
        if (requireCaptureTarget && !target)
        {
            return false;
        }

        if (target && !target.CanBeCaptured())
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(validTargetTag))
        {
            bool tagMatches = other.CompareTag(validTargetTag) ||
                              (target && target.CompareTag(validTargetTag));

            if (!tagMatches)
            {
                return false;
            }
        }

        int otherLayerBit = 1 << other.gameObject.layer;

        if ((validTargetLayers.value & otherLayerBit) == 0)
        {
            return false;
        }

        return true;
    }

    private void ApplyMaterial(Material material)
    {
        if (!material)
        {
            return;
        }

        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i])
            {
                renderers[i].sharedMaterial = material;
            }
        }
    }

    private void ShowMessage(string message)
    {
        if (!showMessages)
        {
            return;
        }

        if (ItemPickUpUI.Instance)
        {
            ItemPickUpUI.Instance.ShowMessage(message, trapIcon);
        }
    }
}