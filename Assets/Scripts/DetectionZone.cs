using UnityEngine;
using System.Collections.Generic;

public class DetectionZone : MonoBehaviour
{
    private List<Collider2D> detectedColliders = new List<Collider2D>();

    public List<Collider2D> DetectedColliders => detectedColliders;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!detectedColliders.Contains(other))
        {
            detectedColliders.Add(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (detectedColliders.Contains(other))
        {
            detectedColliders.Remove(other);
        }
    }
}