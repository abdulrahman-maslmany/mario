using UnityEngine;

public class PlayerInputSimulation : MonoBehaviour
{
    public bool MoveLeft { get; set; }
    public bool MoveRight { get; set; }
    public bool JumpPressed { get; set; }
    public bool AttackPressed { get; set; }

    public void OnLeftButtonDown() => MoveLeft = true;
    public void OnLeftButtonUp() => MoveLeft = false;

    public void OnRightButtonDown() => MoveRight = true;
    public void OnRightButtonUp() => MoveRight = false;

    public void OnJumpButtonDown() => JumpPressed = true;
    public void OnJumpButtonUp() => JumpPressed = false;

    public void OnAttackButtonDown() => AttackPressed = true;
    public void OnAttackButtonUp() => AttackPressed = false;

    // إعادة الضغطة بعد التنفيذ
    public void ResetJump() => JumpPressed = false;
    public void ResetAttack() => AttackPressed = false;
}