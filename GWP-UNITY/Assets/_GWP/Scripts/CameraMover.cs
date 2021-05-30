using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public enum MovementModes { Direct, SmoothDamp, Inertia }

    public Camera cam;
    public MovementModes movementMode;

    public Vector3 Velocity { get; set; }
    public float MaxCamSize => Mathf.Min(positionBounds.width / cam.aspect / 2, positionBounds.height / 2);

    public float StartingCameraSize 
    { 
        get => null == _startingCamSize 
            ? (_startingCamSize = cam.orthographicSize).Value 
            : _startingCamSize.Value;
    }
    private float? _startingCamSize;

    public Vector3 TargetPos { get; set; }

    public float TargetSize 
    { 
        get => _targetSize;
        set => _targetSize = Mathf.Clamp(value, minCamSize, MaxCamSize); 
    }
    private float _targetSize;

    private Rect AdjustedBounds // Calculate adjusted bounds
    {
        get
        {
            return Rect.MinMaxRect(
                positionBounds.xMin + cam.orthographicSize * cam.aspect, positionBounds.yMin + cam.orthographicSize
                , positionBounds.xMax - cam.orthographicSize * cam.aspect, positionBounds.yMax - cam.orthographicSize);
        }
    }

    [Header("Limits")]
    public Rect positionBounds;
    [SerializeField] private float minCamSize = 2.68f;
    [SerializeField] private Vector3 planeNormal = Vector3.forward;

    [Header("Timing")]
    [SerializeField] private float smoothTime = 0.5f;
    [SerializeField] private float inertialDamping = 5f;

    private Quaternion planeRotation;
    private Vector3 normalOffset;
    private float sizeVelocity;
    private bool wasInitialized = false;
    private const float camSizeLerpAmount = 16f;

    public void Initialize(Vector3? position = null, float? size = null)
    {
        if (wasInitialized) return;
        wasInitialized = true;

        _startingCamSize = cam.orthographicSize;
        planeRotation = Quaternion.FromToRotation(Vector3.forward, planeNormal);
        normalOffset = Vector3.Project(transform.position, planeNormal);
        TargetPos = (null != position) ? position.Value : transform.position;
        TargetSize = (null != size) ? size.Value : cam.orthographicSize;
    }

    public void ClampTargetSize() => TargetSize = TargetSize; // Setter handles clamping

    public void UpdateMove()
    {
        Vector3 pos = transform.position;
        switch (movementMode)
        {
            case MovementModes.Direct:
                pos = TargetPos;
                break;
            case MovementModes.SmoothDamp:
                Vector3 vel = Velocity;
                pos = Vector3.SmoothDamp(transform.position, TargetPos, ref vel, smoothTime);
                Velocity = vel;
                break;
            case MovementModes.Inertia:
                pos = transform.position + Velocity * Time.deltaTime;
                Velocity *= (1f - inertialDamping * Time.deltaTime);
                break;
        }

        pos = Vector3.ProjectOnPlane(pos, normalOffset);
        pos += normalOffset;

        transform.position = BoundMove(pos);
    }

    public void UpdateZoom()
    {
        if (!cam.orthographic) { cam.orthographicSize = minCamSize; return; }
        ClampTargetSize();
        if (movementMode == MovementModes.SmoothDamp)
        {
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, TargetSize, ref sizeVelocity, smoothTime);
        }
        else
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, TargetSize, camSizeLerpAmount * Time.deltaTime);
        }
    }

    private Vector3 BoundMove(Vector3 pos)
    {
        Vector3 moveDir = pos - transform.position;

        // Get Bounds
        Rect adjBounds = AdjustedBounds;
        Vector3 max = planeRotation * adjBounds.max;
        Vector3 min = planeRotation * adjBounds.min;

        // Cancel out movement if it would result in an out of bounds position
        if (moveDir.x < 0 && pos.x <= min.x) { moveDir.x = 0; }
        else if (moveDir.x > 0 && pos.x >= max.x) { moveDir.x = 0; }

        if (moveDir.y < 0 && pos.y <= min.y) { moveDir.y = 0; }
        else if (moveDir.y > 0 && pos.y >= max.y) { moveDir.y = 0; }

        if (moveDir.z < 0 && pos.z <= min.z) { moveDir.z = 0; }
        else if (moveDir.z > 0 && pos.z >= max.z) { moveDir.z = 0; }

        pos = transform.position + moveDir;

        // Reposition onto bounds if position is out of bounds
        if (pos.x < min.x) { moveDir.x += min.x - transform.position.x; }
        else if (pos.x > max.x) { moveDir.x += max.x - transform.position.x; }

        if (pos.y < min.y) { moveDir.y += min.y - transform.position.y; }
        else if (pos.y > max.y) { moveDir.y += max.y - transform.position.y; }

        if (pos.z < min.z) { moveDir.z += min.z - transform.position.z; }
        else if (pos.z > max.z) { moveDir.z += max.z - transform.position.z; }

        return transform.position + moveDir + normalOffset;
    }

    private void Awake() => Initialize();

    private void Update()
    {
        UpdateMove();
        UpdateZoom();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (null == cam) { cam = GetComponent<Camera>(); }

        planeRotation = Quaternion.FromToRotation(Vector3.forward, planeNormal);

        Rect adjBounds = AdjustedBounds;
        Gizmos.color = Color.cyan;
        GizmoUtility.DrawRect(adjBounds, planeRotation);

        Gizmos.color = Color.blue;
        GizmoUtility.DrawRect(positionBounds, planeRotation);
    }
#endif
}
