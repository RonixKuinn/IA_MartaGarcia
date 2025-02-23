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
        Searching,
        Waiting,
        Attacking
    }

    // ······ Cosas generales ······//
    public EnemyState currentState;
    private NavMeshAgent _AIAgent;
    private Transform _playerTransform;

    // ······ Cosas patrulla ······//
    [SerializeField] Transform[] _patrolPoints; // Lista de puntos de patrullaje
    private int _currentPatrolIndex = 0; // Índice del punto actual en la secuencia
    private float _waitTime = 5f; // Tiempo de espera en cada punto
    private float _waitTimer = 0f; // Temporizador para contar los 5 segundos

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
        MoveToNextPatrolPoint(); // Iniciar patrullaje desde el primer punto
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

            case EnemyState.Waiting:
                Waiting();
                break;

            case EnemyState.Attacking:
                Attacking();
                break;
        }
    }

    void Patrol()
    {
        if (OnRange())  // Si el jugador está en rango, cambiar a Chasing
        {
            currentState = EnemyState.Chasing;
            return;
        }

        // Si el enemigo ha llegado al punto de patrullaje y no hay jugador cerca, esperar
        if (_AIAgent.remainingDistance < 0.5f)
        {
            // Cambiar a Waiting para esperar antes de continuar al siguiente punto
            currentState = EnemyState.Waiting;
        }
    }

    void Chase()
    {
        if (!OnRange())  // Si el jugador se aleja, vuelve al patrullaje
        {
            currentState = EnemyState.Patrolling;
            MoveToNextPatrolPoint();  // Continuar al siguiente punto de patrullaje
        }

        _AIAgent.destination = _playerTransform.position;  // El enemigo sigue al jugador
    }

    void Search()
    {
        if (OnRange())  // Si el jugador está dentro del rango, comienza a perseguirlo
        {
            currentState = EnemyState.Chasing;
        }

        // Aquí podrías agregar la lógica para buscar al jugador en un área determinada
    }

    bool OnRange()
    {
        Vector3 directionToPlayer = _playerTransform.position - transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        if (distanceToPlayer > _visionRange)  // Si el jugador está fuera del rango
        {
            return false;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer))
        {
            if (hit.collider.CompareTag("Player"))  // Si la línea de visión toca al jugador
            {
                _playerLastPosition = _playerTransform.position;
                return true;
            }
        }

        return false;
    }

    void MoveToNextPatrolPoint()
    {
        if (_patrolPoints.Length == 0)
            return;

        _AIAgent.destination = _patrolPoints[_currentPatrolIndex].position;
        _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;  // Ciclo a través de los puntos
    }

    void Waiting()
    {
        // Si el jugador está cerca, no espera y continúa patrullando
        if (OnRange())
        {
            currentState = EnemyState.Chasing;
            return;
        }

        _waitTimer += Time.deltaTime;  // Incrementa el temporizador de espera

        // Si el enemigo ha esperado 5 segundos, continúa al siguiente punto
        if (_waitTimer >= _waitTime)
        {
            _waitTimer = 0f;  // Reiniciar el temporizador
            currentState = EnemyState.Patrolling;
            MoveToNextPatrolPoint();  // Mover al siguiente punto de patrullaje
        }
    }

    void Attacking()
    {
        // Aquí iría la lógica de ataque
    }

    void OnDrawGizmos()
    {
        foreach (Transform point in _patrolPoints)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(point.position, 1);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _visionRange);
    }
}
/*using System.Collections;
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
        Searching,
        Waiting,
        Attaking
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
    // ······ Cosas busqueda ······//
    float _searchTimer;
    float _searchWaitTime = 15;
    float _searchRadious = 10;


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
            //---------------------------
            case EnemyState.Chasing:
                Chase();
            break;
            //---------------------------
            case EnemyState.Searching:
                Search();
            break;
            //---------------------------
            case EnemyState.Searching:
                Waiting();
            break;
            //---------------------------
            case EnemyState.Searching:
                Attaking();
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
            currentState = EnemyState.Searching;
        }

        _AIAgent.destination = _playerTransform.position;
    }

    void Search()
    {
        if(OnRange())
        {
            currentState = EnemyState.Chasing;
        }
        _searchTimer += Time.deltaTime;

        if(_searchTimer < _searchWaitTime)
        {
            if(_AIAgent.remainingDistance < 0.5f)
            {
                Vector3 randomPoint;
                if(RandomSearchPoint(_playerLastPosition, _searchRadious, out randomPoint))
                {
                    _AIAgent.destination = randomPoint;
                }
            }
        }
        else
        {
            currentState = EnemyState.Patrolling;
            _searchTimer = 0;
        }
    }

    bool RandomSearchPoint(Vector3 center, float radius, out Vector3 point)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * radius;
        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomPoint, out hit, 4, NavMesh.AllAreas))
        {
            point = hit.position;
            return true;
        }
        point = Vector3.zero;
            return false;
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
        }

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

    void Waiting()
    {

    }

    void Attaking()
    {

    }
}*/