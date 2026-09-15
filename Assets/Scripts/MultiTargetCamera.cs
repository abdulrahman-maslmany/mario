using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class MultiTargetCamera : MonoBehaviour
{
    [Header("🎯 Targets Settings")]
    [Tooltip("أضف جميع اللاعبين أو الأهداف هنا")]
    public List<Transform> targets = new List<Transform>();

    [Header("📷 Camera Movement")]
    public Vector3 offset = new Vector3(0f, 2f, -10f);
    [Range(0f, 10f)] public float smoothSpeed = 5f;

    [Header("🔍 Zoom Settings")]
    public float minZoom = 5f;
    public float maxZoom = 10f;
    public float zoomLimiter = 30f;

    [Header("🧱 Camera Limits (اختياري)")]
    public bool useLimits = false;
    public Vector2 minLimit;
    public Vector2 maxLimit;

    private Camera cam;
    private Vector3 velocity;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (targets == null || targets.Count == 0)
            return;

        Move();
        Zoom();
    }

    private void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 desiredPosition = centerPoint + offset;

        if (useLimits)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minLimit.x, maxLimit.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minLimit.y, maxLimit.y);
        }

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / smoothSpeed);
    }

    private void Zoom()
    {
        float greatestDistance = GetGreatestDistance();
        float newZoom = Mathf.Lerp(maxZoom, minZoom, greatestDistance / zoomLimiter);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
    }

    private Vector3 GetCenterPoint()
{
    // إزالة العناصر المحذوفة
    targets.RemoveAll(t => t == null);

    if (targets.Count == 0)
        return transform.position;

    if (targets.Count == 1)
        return targets[0].position;

    var bounds = new Bounds(targets[0].position, Vector3.zero);
    for (int i = 1; i < targets.Count; i++)
    {
        if (targets[i] != null)
            bounds.Encapsulate(targets[i].position);
    }

    return bounds.center;
}

    private float GetGreatestDistance()
    {
        if (targets.Count < 2) return 0f;

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        foreach (var target in targets)
        {
            if (target != null)
                bounds.Encapsulate(target.position);
        }
        return Mathf.Max(bounds.size.x, bounds.size.y);
    }

    // 🧩 إضافة أو إزالة اللاعبين أثناء اللعب (مثلاً عند Spawn أو موت اللاعب)
    public void AddTarget(Transform newTarget)
    {
        if (!targets.Contains(newTarget))
            targets.Add(newTarget);
    }

    public void RemoveTarget(Transform targetToRemove)
    {
        if (targets.Contains(targetToRemove))
            targets.Remove(targetToRemove);
    }
}
