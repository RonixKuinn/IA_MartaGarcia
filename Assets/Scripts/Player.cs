using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    private NavMeshAgent _playerAgent;

    void Awake()
    {
        _playerAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            SetDestinationPoint();
        }
    }

    void SetDestinationPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.layer == 3)
            {
                _playerAgent.destination = hit.point;
                //_playerAgent.SetDestination(hit.point);
            }
        }
    }
}
