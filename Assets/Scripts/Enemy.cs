using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        Patrolling,
        Chasing,
        Searching
    }

public EnemyState currentState;
private NavMeshAgent _AIAgent;
[SerializeField] Transform[] _patrolPoints;

    void Start()
    {
        
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
            break;

            case EnemyState.Chasing:
                Chase();
            break;
            case EnemyState.Searching:
                Search();
            break;
        }
    }

    void Patrol()
    { 
        if(_AIAgent.remainingDistance < 0.5f)
        {
          SetRandomPatrolPoint();  
        }
    }

    void Chase()
    {

    }

    void Search()
    {

    }

    void SetRandomPatrolPoint()
    {
        _AIAgent.destination = _patrolPoints[Random.Range(0, _patrolPoints.Length)].position;
    }
}
