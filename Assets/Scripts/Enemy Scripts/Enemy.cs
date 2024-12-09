using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public Animator enemyAnimator;
    public SpriteRenderer enemySpriteRenderer;
    protected BehaviourTree enemyBehaviourTree;

    protected PlayerController playerController =>
        GameManager.instance.blackBoard.GetValue("PlayerController", out PlayerController _controller) ? _controller : null;  

    public abstract void OnDamage(float amount);

    public abstract float InflictDamage(float damateAmount);

    public abstract void InitBehaviourTree();
    
}
