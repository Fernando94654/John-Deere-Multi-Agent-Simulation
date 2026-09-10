using UnityEngine;

// Keeps the top-down camera framing the whole field.
// The grid is built once at an arbitrary size, so a fixed camera height/Z clips
// large fields (noticeable past ~30x30). This measures the generated cells and
// pushes the camera up in Y and back in Z so the frustum covers everything.
[RequireComponent(typeof(Camera))]
public class TopCameraFraming : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The generator whose 'fields' array defines the area to frame. " +
             "If left empty it is found in the scene at Start.")]
    public FieldGridGenerator gridGenerator;

    [Header("Framing")]
    [Tooltip("Extra margin around the field, as a fraction of its size. 0.1 = 10% border.")]
    public float padding = 0.1f;

    [Tooltip("Camera never drops below this height, so small fields are not over-zoomed.")]
    public float minHeight = 200f;

    [Tooltip("Re-check the grid every frame and reframe if its size changed (e.g. regenerated).")]
    public bool trackChanges = true;

    private Camera cam;
    private int lastFieldCount = -1;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        if (gridGenerator == null)
        {
            gridGenerator = FindFirstObjectByType<FieldGridGenerator>();
        }
    }

    void LateUpdate()
    {
        if (gridGenerator == null || !gridGenerator.IsGenerated || gridGenerator.fields == null)
        {
            return;
        }

        // One-shot after generation, then only when the grid is rebuilt at a new size.
        if (!trackChanges && lastFieldCount == gridGenerator.fields.Length)
        {
            return;
        }

        if (lastFieldCount != gridGenerator.fields.Length)
        {
            lastFieldCount = gridGenerator.fields.Length;
            Reframe();
        }
    }

    [ContextMenu("Reframe Now")]
    public void Reframe()
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }

        if (gridGenerator == null || gridGenerator.fields == null || gridGenerator.fields.Length == 0)
        {
            return;
        }

        if (!TryGetFieldBounds(out Bounds bounds))
        {
            return;
        }

        // Camera looks straight down (-Y). After the 90 deg X rotation its vertical
        // screen axis maps to world Z and its horizontal axis to world X, so:
        //   world-Z extent is covered by the vertical FOV
        //   world-X extent is covered by the horizontal FOV (vertical * aspect)
        float halfFovRad = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;
        float tanHalfFov = Mathf.Tan(halfFovRad);
        float aspect = cam.aspect; // uses the render-texture aspect when one is assigned

        float halfDepth = bounds.extents.z * (1f + padding);
        float halfWidth = bounds.extents.x * (1f + padding);

        float heightForDepth = halfDepth / tanHalfFov;
        float heightForWidth = halfWidth / (tanHalfFov * aspect);

        float height = Mathf.Max(heightForDepth, heightForWidth, minHeight);

        // Sit directly above the field centre; keep the straight-down orientation.
        Vector3 target = new Vector3(bounds.center.x, bounds.center.y + height, bounds.center.z);
        transform.position = target;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // Make sure the ground stays inside the view volume for very large fields.
        float neededFar = height - bounds.min.y + 10f;
        if (cam.farClipPlane < neededFar)
        {
            cam.farClipPlane = neededFar + 100f;
        }
    }

    // Encapsulates every cell's renderer bounds (falls back to transform position).
    private bool TryGetFieldBounds(out Bounds bounds)
    {
        bounds = default;
        bool initialised = false;

        foreach (GameObject field in gridGenerator.fields)
        {
            if (field == null)
            {
                continue;
            }

            Bounds cellBounds;
            Renderer[] renderers = field.GetComponentsInChildren<Renderer>();

            if (renderers.Length > 0)
            {
                cellBounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                {
                    cellBounds.Encapsulate(renderers[i].bounds);
                }
            }
            else
            {
                cellBounds = new Bounds(field.transform.position, Vector3.zero);
            }

            if (!initialised)
            {
                bounds = cellBounds;
                initialised = true;
            }
            else
            {
                bounds.Encapsulate(cellBounds);
            }
        }

        return initialised;
    }
}
