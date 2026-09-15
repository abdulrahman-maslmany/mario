using UnityEngine;

public class ReferenceHolder : MonoBehaviour
{
    public GameObject player;

    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Player not found! Ensure a GameObject with tag 'Player' exists in the scene.");
            }
        }
    }
}