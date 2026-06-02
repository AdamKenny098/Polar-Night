using UnityEngine;

public enum EntityEvidenceType
{
    Footprint,
    Residue,
    Scratch,
    DisturbedSnow
}

public class EntityEvidenceMarker : MonoBehaviour
{
    [Header("Evidence")]
    public EntityEvidenceType evidenceType = EntityEvidenceType.Footprint;
    public string sourceEntityName;

    [Header("Lifetime")]
    public bool destroyAfterLifetime = true;
    public float lifetime = 180f;

    private void Start()
    {
        if (destroyAfterLifetime && lifetime > 0f)
        {
            Destroy(gameObject, lifetime);
        }
    }
}