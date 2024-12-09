using AdvancedStateHandling;
using Cinemachine;
using System.Collections;
using UnityEngine;

public class MainCharacterState : IState
{

    protected PlayerController characterController;

    public MainCharacterState(PlayerController controller)
    {
        this.characterController = controller;
    }

    public virtual void OnStart()
    {
        return;
    }

    public virtual void OnExit()
    {
        return;
    }

    public virtual void Update()
    {
        return;

    }
}

public class MoveState : MainCharacterState
{

    public MoveState(PlayerController controller) : base(controller)
    {
    }
    
    public override void OnStart()
    {
        base.OnStart();

    }
    public override void OnExit()
    {
        base.OnExit();
    }
    public override void Update()
    {
        base.Update();
        Debug.Log("MoveState");
        characterController.rb.linearVelocity = new Vector2(characterController.direction * characterController.moveSpeed, characterController.rb.linearVelocity.y);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            characterController.jump = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            characterController.isInDash = true;
        }
    }
}

public class JumpState : MainCharacterState
{
    float jumpForce = 15f;

    public JumpState(PlayerController controller) : base(controller)
    {
    }
  
    public override void OnStart()
    {
        base.OnStart();
        if (!characterController.isJumped)
        {
            characterController.rb.linearVelocity = new Vector2(characterController.rb.linearVelocity.x, jumpForce);
        }
        
    }

    public override void OnExit()
    {
        base.OnExit();
        characterController.jump = false;
    }
    public override void Update()
    {
        base.Update();
        Debug.Log("JumpState");
        characterController.rb.linearVelocity =
            new Vector2(characterController.direction * characterController.moveSpeed, characterController.rb.linearVelocity.y);
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            characterController.isInDash = true;
        }
    }
}

public class DashState : MainCharacterState
{

    float dashForce = 50f;
    float duration = 0.1f;
    float slowTimePeriod = 0.05f;
    bool isTimeSlowed = false;
    public DashState(PlayerController controller) : base(controller)
    {

    }
  
    public override void OnStart()
    {
        base.OnStart();
        Time.timeScale = 0.4f;
        characterController.rb.linearVelocity
            = new Vector2(
                characterController.direction == 0 ? (characterController.isFacingRight ? dashForce : -1 * dashForce) : 
                characterController.direction * dashForce, characterController.rb.linearVelocity.y);

    }

    public override void OnExit()
    {
        base.OnExit();
        isTimeSlowed = false;
        slowTimePeriod = 0.05f;
    }

    public override void Update()
    {
        base.Update();
        Debug.Log("DashState");
        slowTimePeriod -= Time.deltaTime;

        if (slowTimePeriod <= 0)
        {
            if (!isTimeSlowed)
            {
                isTimeSlowed = true;
                characterController.StartCoroutine(Timer());
                slowTimePeriod = 0.05f;
                Time.timeScale = 1f;
            }
            
         }
        
        
    }   
    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(duration);
        characterController.isInDash = false;

    }
}

public class DamageState : MainCharacterState
{
    float force = 5f;
    float yForce = 5f;
    float timer = 0.3f;

    CinemachineBasicMultiChannelPerlin channel =>
        GameManager.instance.blackBoard.GetValue("Channel", out CinemachineBasicMultiChannelPerlin _channel) ? _channel : null;  

    public DamageState(PlayerController controller) : base(controller)
    {
    }
    public override void OnStart()
    {
        base.OnStart();
        characterController.rb.AddForce(new Vector2(characterController.isFacingRight ? force : -1 * force, yForce),ForceMode2D.Impulse);
        characterController.StartCoroutine(ShakeCamera());
    }


    public override void OnExit()
    {
        base.OnExit();
        characterController.rb.linearVelocity = Vector2.zero;
    }

    public override void Update()
    {
        base.Update();
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            timer = 0.3f;
            characterController.isInDamageState = false;
        }
    }

    private IEnumerator ShakeCamera()
    {
        channel.m_AmplitudeGain = 2f;
        yield return new WaitForSeconds(0.2f);
        channel.m_AmplitudeGain = 0f;
    }

}

public class DeadState : MainCharacterState
{
    public DeadState(PlayerController controller) : base(controller)
    {
    }
    public override void OnStart()
    {
        base.OnStart();

    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void Update()
    {
        base.Update();
    }
}

public class AttackState : MainCharacterState
{
    AnimatorStateInfo stateInfo;
    public AttackState(PlayerController controller) : base(controller)
    {
    }
    public override void OnStart()
    {
        base.OnStart();
        characterController.rb.linearVelocity = Vector2.zero;
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void Update()
    {
        base.Update();
        Debug.Log("Attack");
        stateInfo = characterController.charAnimator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("attack"))
        {
            if(stateInfo.normalizedTime >= 1)
            {
                characterController.canAttack = false;
            }
        }
    }
}