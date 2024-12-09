using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
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

    // ······ Cosas generales ······//
    public EnemyState currentState;
    private NavMeshAgent _AIAgent;
    private Transform _playerTransform;

    // ······ Cosas patrulla ······//
    [SerializeField] Transform[] _patrolPoints;
    [SerializeField] Vector2 _patrolAreaSize = new Vector2(5, 5);
    [SerializeField] Transform _patrolAreaCenter;
 
    // ······ Cosas detección ······//
    [SerializeField] float _visionRange = 5;
    [SerializeField] float _visionAngle = 120;
    private Vector3 _playerLastPosition;


    void Awake()
    {
        _AIAgent = GetComponent<NavMeshAgent>();
        _playerTransform = GameObject.FindWithTag("Player").transform;
    }

    void Start()
    {
        currentState = EnemyState.Patrolling;
        SetRandomPatrolPoint();
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
        if(OnRange())
        {
            currentState = EnemyState.Chasing;
        }

        if(_AIAgent.remainingDistance < 0.5f)
        {
          SetRandomPatrolPoint();  
        }
    }

    void Chase()
    {
        if(!OnRange())
        {
            currentState = EnemyState.Patrolling;
        }

        _AIAgent.destination = _playerTransform.position;
    }

    void Search()
    {

    }

    bool OnRange()
    {
        Vector3 directionToPlayer = _playerTransform.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        if(_playerTransform.position == _playerLastPosition)
        {
            return true;
        }
        if(distanceToPlayer > _visionRange)
        {
            return false;
        }

        if(angleToPlayer > _visionRange)
        {
            return false;
        }

        RaycastHit hit;
        if(Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer))
        {
            if(hit.collider.CompareTag("Player"))
            {
                _playerLastPosition = _playerTransform.position;
                return true;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    void SetRandomPatrolPoint()
    {
        //_AIAgent.destination = _patrolPoints[Random.Range(0, _patrolPoints.Length)].position;
        float RandomX = Random.Range(-_patrolAreaSize.x * 5f, _patrolAreaSize.x * 5f);
        float RandomZ = Random.Range(-_patrolAreaSize.y * 5f, _patrolAreaSize.y * 5f);

        Vector3 randomPoint = new Vector3(RandomX, 0, RandomZ) + _patrolAreaCenter.position;

        _AIAgent.destination = randomPoint;
    }

    void OnDrawGizmos()
    {
        /*foreach(Transform point in _patrolPoints)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(point.position, 1);
        }*/

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_patrolAreaCenter.position, new Vector3(_patrolAreaSize.x, 1, _patrolAreaSize.y));

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _visionRange);

        Gizmos.color = Color.yellow;
        Vector3 fovLine1 = Quaternion.AngleAxis(_visionAngle * 0.5f, transform.up) * transform.forward * _visionRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(_visionAngle * 0.5f, transform.up) * transform.forward * _visionRange;
        Gizmos.DrawLine(transform.position, transform.position + fovLine1);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2);
    }
}
