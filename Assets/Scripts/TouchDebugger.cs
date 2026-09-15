using UnityEngine;
using UnityEngine.InputSystem;

public class TouchDebugger : MonoBehaviour
{
    void Update()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Debug.Log($"Touch detected at position: {touchPosition}");
        }
    }
}