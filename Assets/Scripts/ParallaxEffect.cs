using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform followTarget;

    private Vector2 startPosition;
    private float startingZ;

    private Vector2 CamMoveSinceStart => (Vector2)cam.transform.position - startPosition;
    private float ZDistanceFromTarget => transform.position.z - followTarget.position.z;
    private float ClippingPlane => cam.transform.position.z + (ZDistanceFromTarget > 0 ? cam.farClipPlane : cam.nearClipPlane);
    private float ParallaxFactor => Mathf.Abs(ZDistanceFromTarget) / ClippingPlane;

    private void Start()
    {
        if (cam == null) cam = Camera.main;
        if (followTarget == null) followTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (cam == null || followTarget == null)
        {
            Debug.LogError("ParallaxEffect: Camera or FollowTarget missing.");
            enabled = false;
            return;
        }
        startPosition = transform.position;
        startingZ = transform.position.z;
    }

    private void Update()
    {
        if (cam == null || followTarget == null) return;
        Vector2 newPosition = startPosition + CamMoveSinceStart * ParallaxFactor;
        transform.position = new Vector3(newPosition.x, newPosition.y, startingZ);
    }
}