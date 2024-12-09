using UnityEngine;
using AdvancedStateHandling;
using Cinemachine;
using System;


public class PlayerController : MonoBehaviour
{
    AdvancedStateMachine stateMachine;
    public Animator charAnimator;
    public float direction;
    public Rigidbody2D rb;
    public float moveSpeed;
    float currentHp;
    float maxHp = 100;
    public Healthbar characterHealthBar;
    public Vector2 damageDirection;
    

    [Header("Conditions")]
    public bool isJumped;
    public bool jump = false;
    public bool isInDash = false;
    public bool isFacingRight = true;
    public bool isInDamageState = false;
    public bool isDead;
    public bool canAttack;

    void Start()
    {
        GameManager.instance.blackBoard.SetValue("PlayerController", this);
        currentHp = maxHp;
        characterHealthBar.SetCurrentHealth(currentHp);

        stateMachine = new AdvancedStateMachine();

        var moveState = new MoveState(this);
        var jumpState = new JumpState(this);
        var dashState = new DashState(this);
        var damageState = new DamageState(this);
        var deadState = new DeadState(this);
        var attackState = new AttackState(this);

        Add(moveState, jumpState, new FuncPredicate(() => jump));
        Add(jumpState, moveState, new FuncPredicate(() => !isJumped && rb.linearVelocity.y == 0));
        Add(moveState, dashState, new FuncPredicate(() => isInDash));
        Add(jumpState, dashState, new FuncPredicate(() => isInDash));
        Add(dashState, moveState, new FuncPredicate(() => !isInDash));
        Add(dashState, jumpState, new FuncPredicate(() => !isInDash));
        

        Any(damageState, new FuncPredicate(() => isInDamageState));
        Add(damageState, moveState, new FuncPredicate(() => !isInDamageState && !isJumped));
        Add(damageState, jumpState, new FuncPredicate(() => !isInDamageState && isJumped));

        Any(deadState, new FuncPredicate(() => isDead));

        Any(attackState, new FuncPredicate(() => canAttack && !isInDash));

        Add(attackState, moveState, new FuncPredicate(() => !isJumped && !canAttack));
        Add(attackState, jumpState, new FuncPredicate(() => isJumped && !canAttack));

        stateMachine.currentState = moveState;
   
    }

    private void Add(IState from, IState to,IPredicate predicate)
    {
        stateMachine.AddTransition(from, to, predicate);
    }

    private void Any(IState to, IPredicate predicate)
    {
        stateMachine.AddTransitionFromAnytate(to, predicate);
    }

    void Update()
    {
        AnimatorStateInfo currenState = charAnimator.GetCurrentAnimatorStateInfo(0);
        stateMachine.Update();
        Direction();
        if (!currenState.IsName("dash") && !currenState.IsName("death"))
        {
            SetRotation();
        }
        if(currentHp <= 0)
        {
            isDead = true;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            canAttack = true;
        }
        
        AnimationController();
    }


    private void Direction()
    {
        direction = Input.GetAxisRaw("Horizontal");
    }

    public void SetRotation()
    {
        if(isFacingRight && direction < 0)
        {
            transform.localScale = new Vector2(-1, 1);
            isFacingRight = false;

        }else if(!isFacingRight && direction > 0)
        {
            transform.localScale = new Vector2(1, 1);
            isFacingRight = true;
        }
    }

    private void AnimationController()
    {
        charAnimator.SetBool("isInDash", isInDash);
        charAnimator.SetFloat("direction", direction);
        charAnimator.SetBool("isJumped", isJumped);
        charAnimator.SetBool("isDead", isDead);
        charAnimator.SetBool("attack", canAttack);
    }

    public void OnDamage(float damageAmount)
    {
        currentHp -= damageAmount;
        characterHealthBar.SetCurrentHealth(currentHp);
    }

    public float InflictDamage(float damageAmount)
    {
        return damageAmount;
    }

   
}
