using UnityEngine;

public class ButtonDebug : MonoBehaviour
{
    public void OnPointerDownDebug()
    {
        Debug.Log("Pointer Down on Button!");
    }

    public void OnPointerUpDebug()
    {
        Debug.Log("Pointer Up on Button!");
    }
}