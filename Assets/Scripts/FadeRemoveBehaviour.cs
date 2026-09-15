using UnityEngine;
using UnityEngine.Animations;

public class FadeRemoveBehaviour : StateMachineBehaviour
{
    public float fadeTime = 0.5f;
    private float timer = 0f;
    private SpriteRenderer spriteRenderer;
    private GameObject objToRemove;
    private Color startColor;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0f;
        spriteRenderer = animator.GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
        objToRemove = animator.gameObject;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        float newAlpha = startColor.a * (1 - (timer / fadeTime));
        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);

        if (timer >= fadeTime)
        {
            Destroy(objToRemove);
        }
    }
}