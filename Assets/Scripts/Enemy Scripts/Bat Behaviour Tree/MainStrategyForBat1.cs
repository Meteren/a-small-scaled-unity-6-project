
using UnityEngine;
using System.Collections.Generic;
using System;

public class MainStrategyForBat1 
{
    protected PlayerController playerController => 
        GameManager.instance.blackBoard.GetValue("PlayerController",out PlayerController _controller) ? _controller : null;

    protected Bat1 bat => 
        GameManager.instance.blackBoard.GetValue("Bat1", out Bat1 _bat) ? _bat : null;
}


public class ConditionOne : IStrategy
{
    Func<bool> condition { get; set; }

    public ConditionOne(Func<bool> condition)
    {
        this.condition = condition;
    }

    public Node.NodeStatus Evaluate()
    {
        if (condition())
        {
            return Node.NodeStatus.SUCCESS;
        }
        else
        {
            return Node.NodeStatus.FAILURE;
        }
    }
}

public class PatrolStrategyOne : MainStrategyForBat1, IStrategy
{
    int currentIndex = 0;
    List<Transform> patrolPoints = new List<Transform>();
    float distance = 0.1f;
    float speed = 15f;

    public PatrolStrategyOne(List<Transform> patrolPoints)
    {
        this.patrolPoints = patrolPoints;
    }

    public Node.NodeStatus Evaluate()
    {
        Debug.Log("Patrol Strategy");
        bat.transform.position =
            Vector2.MoveTowards(bat.transform.position, patrolPoints[currentIndex].transform.position, Time.deltaTime * speed);
        if (Vector2.Distance(patrolPoints[currentIndex].transform.position,bat.transform.position) < distance)
        {
            ChooseRandomPoint();
        }
        if(bat.attackProb > 60)
        {
            return Node.NodeStatus.SUCCESS;
        }

        return Node.NodeStatus.RUNNING;
    }

    private void ChooseRandomPoint()
    {
        int random = UnityEngine.Random.Range(0, patrolPoints.Count);
        if(random == currentIndex)
        {
            if(currentIndex + 1 == patrolPoints.Count)
            {
                random--;
            }
            else
            {
                random++;
            }
           
        }
        currentIndex = random;
        
    }

}

public class AttackStrategyOne : MainStrategyForBat1, IStrategy
{
    int currentIndex = 0;
    float distance = 0.1f;
    float speed = 27f;
    bool positionCaptured = false;
    Vector2 capturedPosition;
    public Node.NodeStatus Evaluate()
    {
        Debug.Log("Attack Strategy");

        if (!positionCaptured)
        {
            capturedPosition = playerController.transform.position;
            bat.isInAttack = true;
            positionCaptured = true;
        }

        bat.transform.position =
          Vector2.MoveTowards(bat.transform.position, capturedPosition, Time.deltaTime * speed);
        
        if (Vector2.Distance(capturedPosition, bat.transform.position) < distance)
        {
            positionCaptured = false;
            bat.isInAttack = false;
            return Node.NodeStatus.SUCCESS;
        }

        return Node.NodeStatus.RUNNING;
    }
}

public class TurnBackToPatrolStrategyOne : MainStrategyForBat1, IStrategy
{
    List<Transform> patrolPoints = new List<Transform>();
    float distance = 1f;
    int pointToReturn;
    bool pointChoosen = false;
    float speed = 10f;

    public TurnBackToPatrolStrategyOne(List<Transform> patrolPoints)
    {
        this.patrolPoints = patrolPoints;
    }

    public Node.NodeStatus Evaluate()
    {
        Debug.Log("Turn Back Strategy");
        if (!pointChoosen)
        {
            pointChoosen = true;
            ChoosePointToReturn();
        }
        bat.transform.position =
         Vector2.MoveTowards(bat.transform.position, patrolPoints[pointToReturn].transform.position, Time.deltaTime * speed);
        if (Vector2.Distance(patrolPoints[pointToReturn].transform.position, bat.transform.position) < distance)
        {
            pointChoosen = false;
            return Node.NodeStatus.SUCCESS;
        }

        return Node.NodeStatus.RUNNING;
    }

    private void ChoosePointToReturn()
    {
        pointToReturn = UnityEngine.Random.Range(0, patrolPoints.Count);
    }
}
