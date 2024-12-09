using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat1 : Enemy
{
    public List<Transform> patrolPoints;
    
    public int attackProb;
    float currentHp;
    float maxHp = 30;
    public bool isDead = false;

    public ParticleSystem deathParticle;

    [Header("Conditions")]
    public bool blockCoroutine = false;
    public bool isInAttack = false;

    private void Start()
    {
        currentHp = maxHp;
        InitBehaviourTree();
        GameManager.instance.blackBoard.SetValue("Bat1", this);
        SortedSelectorNode mainSelector = new SortedSelectorNode("MainSelector");

        Leaf patrolStrategy = new Leaf("PatrolStrategy", new PatrolStrategyOne(patrolPoints),20);

        mainSelector.AddChild(patrolStrategy);

        SequenceNode attackSequence = new SequenceNode("PatrolSequence",10);

        mainSelector.AddChild(attackSequence);

        Leaf attackCondition = new Leaf("AttackCondition", new ConditionOne(() =>
        { 
            return attackProb > 60 && !playerController.isDead;
        }));

        Leaf attackStrategy = new Leaf("AttackStrategy", new AttackStrategyOne());

        Leaf turnBackStrategy = new Leaf("TurnBackStrategy", new TurnBackToPatrolStrategyOne(patrolPoints));

        attackSequence.AddChild(attackCondition);
        attackSequence.AddChild(attackStrategy);
        attackSequence.AddChild(turnBackStrategy);

        enemyBehaviourTree.AddChild(mainSelector);


    }

    private void FixedUpdate()
    {
        enemyBehaviourTree.Process();
    }

    private void Update()
    {
        if (!blockCoroutine)
        {
            StartCoroutine(GenerateNumberForAttack());
        } 

        if(currentHp <= 0)
        {
            isDead = true;
        }

        if (isDead)
        {
            deathParticle.transform.position = transform.position;
            deathParticle.Play();

            Destroy(gameObject);
        }
    }
    public override float InflictDamage(float damageAmount)
    {
        return damageAmount;
    }

    public override void OnDamage(float amount)
    {
        currentHp -= amount;
    }

    public override void InitBehaviourTree()
    {
         enemyBehaviourTree = new BehaviourTree("Bat");
    }

    private IEnumerator GenerateNumberForAttack()
    {
        attackProb = Random.Range(0, 100);
        blockCoroutine = true;
        yield return new WaitForSeconds(1f);
        blockCoroutine = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerController>(out PlayerController _controller))
        {
            if (!_controller.isInDash && isInAttack)
            {
                _controller.OnDamage(InflictDamage(10f));
                _controller.isInDamageState = true;
            }  
        }
    }
}
