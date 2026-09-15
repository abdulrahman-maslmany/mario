using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject ProjectilePrefab;

    public void FireProjectilePrefab()
    {
        if (ProjectilePrefab && launchPoint)
        {
            Instantiate(ProjectilePrefab, launchPoint.position, ProjectilePrefab.transform.rotation);
        }
        else
        {
            Debug.LogWarning("ProjectilePrefab or launchPoint not assigned!");
        }
    }
}
