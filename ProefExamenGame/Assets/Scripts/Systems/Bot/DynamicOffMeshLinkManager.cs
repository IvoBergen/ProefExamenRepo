using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Manages dynamic off-mesh links for multiple moving platforms.
/// Uses a world-space box activation zone centered on the jump-off point.
/// </summary>
public class DynamicOffMeshLinkManager : MonoBehaviour
{
    [System.Serializable]
    public class PlatformLink
    {
        [Tooltip("The moving platform Transform.")]
        public Transform movingPlatform;

        [Tooltip("The OffMeshLink component for this platform.")]
        public OffMeshLink offMeshLink;

        [Tooltip("The jump-off point Transform (where the AI starts the jump).")]
        public Transform jumpOffPoint;

        [Tooltip("Local offset on the platform where the AI should land.")]
        public Vector3 landingOffset = new Vector3(0f, 0.1f, 0f);

        [Tooltip("World-space box size centered on the jump-off point. X = width, Y = height, Z = depth.")]
        public Vector3 activationBoxSize = new Vector3(10f, 6f, 10f);

        // Runtime end marker parented to the platform
        [HideInInspector] public Transform endMarker;
    }

    [Header("Platform Links")]
    public PlatformLink[] platforms;

    [Header("Debug")]
    public bool debugLogging = true;

    private void Start()
    {
        foreach (var p in platforms)
        {
            if (p.movingPlatform == null || p.offMeshLink == null || p.jumpOffPoint == null)
            {
                Debug.LogWarning("[DynamicOffMeshLinkManager] Missing references, skipping entry.");
                continue;
            }

            GameObject marker = new GameObject($"OffMeshLink_EndMarker_{p.movingPlatform.name}");
            marker.transform.SetParent(p.movingPlatform);
            marker.transform.localPosition = p.landingOffset;
            p.endMarker = marker.transform;

            p.offMeshLink.startTransform = p.jumpOffPoint;
            p.offMeshLink.endTransform = p.endMarker;
            p.offMeshLink.activated = false;

            if (debugLogging)
                Debug.Log($"[DynamicOffMeshLinkManager] Initialized link for {p.movingPlatform.name}");
        }
    }

    private void Update()
    {
        foreach (var p in platforms)
        {
            if (p.movingPlatform == null || p.offMeshLink == null || p.jumpOffPoint == null) continue;

            bool inRange = IsInWorldBox(p);

            if (inRange != p.offMeshLink.activated)
            {
                p.offMeshLink.activated = inRange;
                if (debugLogging)
                    Debug.Log($"[DynamicOffMeshLinkManager] {p.movingPlatform.name} link {(inRange ? "ACTIVATED" : "DEACTIVATED")}");
            }

            if (p.offMeshLink.activated)
                p.offMeshLink.UpdatePositions();
        }
    }

    private bool IsInWorldBox(PlatformLink p)
    {
        // Simple world-space axis-aligned box centered on the jump-off point
        Vector3 center = p.jumpOffPoint.position;
        Vector3 platPos = p.movingPlatform.position;
        Vector3 half = p.activationBoxSize * 0.5f;

        return Mathf.Abs(platPos.x - center.x) <= half.x &&
               Mathf.Abs(platPos.y - center.y) <= half.y &&
               Mathf.Abs(platPos.z - center.z) <= half.z;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (platforms == null) return;

        foreach (var p in platforms)
        {
            if (p.jumpOffPoint == null) continue;

            bool active = p.offMeshLink != null && p.offMeshLink.activated;

            // Draw world-space activation box
            Gizmos.color = new Color(1f, 1f, 0f, 0.15f);
            Gizmos.DrawCube(p.jumpOffPoint.position, p.activationBoxSize);
            Gizmos.color = active ? Color.green : Color.red;
            Gizmos.DrawWireCube(p.jumpOffPoint.position, p.activationBoxSize);

            if (p.movingPlatform == null) continue;

            // Draw line from jump-off to platform
            Vector3 endWorld = p.movingPlatform.TransformPoint(p.landingOffset);
            Gizmos.color = active ? Color.green : Color.red;
            Gizmos.DrawLine(p.jumpOffPoint.position, endWorld);
            Gizmos.DrawSphere(p.jumpOffPoint.position, 0.15f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(endWorld, 0.2f);
        }
    }
#endif
}