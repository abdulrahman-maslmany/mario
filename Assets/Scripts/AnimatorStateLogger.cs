using UnityEngine;

namespace MyGame
{
    public class AnimatorStateLogger : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            string stateName = "Unknown";
            if (stateInfo.IsName("player_idle")) stateName = "player_idle";
            else if (stateInfo.IsName("player_walk")) stateName = "player_walk";
            else if (stateInfo.IsName("player_jump")) stateName = "player_jump";
            else if (stateInfo.IsName("player_fall")) stateName = "player_fall";
            else if (stateInfo.IsName("player_attack")) stateName = "player_attack";
            else if (stateInfo.IsName("player_bow_attack")) stateName = "player_bow_attack";
            else if (stateInfo.IsName("player_hit")) stateName = "player_hit";
            else if (stateInfo.IsName("player_death")) stateName = "player_death";
            else if (stateInfo.IsName("enemy_idle")) stateName = "enemy_idle";
            else if (stateInfo.IsName("enemy_chase")) stateName = "enemy_chase";
            else if (stateInfo.IsName("enemy_attack")) stateName = "enemy_attack";
            else if (stateInfo.IsName("enemy_hit")) stateName = "enemy_hit";
            else if (stateInfo.IsName("enemy_death")) stateName = "enemy_death";
            Debug.Log($"Animator entered state: {stateName} on {animator.gameObject.name}");
        }
    }
}