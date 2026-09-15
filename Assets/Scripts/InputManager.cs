using UnityEngine;
using UnityEngine.InputSystem;

namespace MyGame
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        private PlayerInputActions inputActions;

        // القيم الحالية
        public Vector2 MoveInput { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool JumpHeld { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            inputActions = new PlayerInputActions();
            inputActions.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
            inputActions.Player.Move.canceled += ctx => MoveInput = Vector2.zero;

            inputActions.Player.Jump.performed += ctx => JumpPressed = true;
            inputActions.Player.Jump.canceled += ctx => JumpPressed = false;

            // للضغط المستمر (مفيد للقفز العالي)
            inputActions.Player.Jump.started += ctx => JumpHeld = true;
            inputActions.Player.Jump.canceled += ctx => JumpHeld = false;
        }

        private void OnEnable()
        {
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        // دالة لتنظيف JumpPressed بعد الاستخدام (لمنع القفز المتكرر)
        public void ConsumeJump()
        {
            JumpPressed = false;
        }
    }
}