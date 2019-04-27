using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public Transform[] patrolPointsArray;

    private int numOfActualPatrolPoint = 0;
    private NavMeshAgent enemyAgent;
    private Transform target;
    private float timeToUpdate = 0.5f;
    private float MinDistToPatrollPoint;

    public enum State
    {
        Patrol,
        Follow,
        Attack
    }

    public State CurrentState = State.Patrol;

    private void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        MinDistToPatrollPoint = enemyAgent.stoppingDistance;
        GotoNextPatrolPoint();
        
        StartCoroutine("CheckStateAndUpdate");
    }

    void GotoNextPatrolPoint()
    {
        enemyAgent.SetDestination(patrolPointsArray[Random.Range(0, patrolPointsArray.Length)].position);
    }

    private IEnumerator CheckStateAndUpdate()
    {
        while (true)
        {
            if (CurrentState == State.Patrol)
            {
                if (enemyAgent.remainingDistance <= MinDistToPatrollPoint)
                {
                    GotoNextPatrolPoint();
                }
            }
            else if (CurrentState == State.Follow)
            {
                Follow();
            }
            else if (CurrentState == State.Attack)
            {
                Debug.Log("Attack");
            }

            yield return new WaitForSeconds(timeToUpdate);
        }
    }

    private void Follow()
    {
        enemyAgent.SetDestination(target.position);
        if (enemyAgent.remainingDistance <= MinDistToPatrollPoint+0.2f)
        {
            CurrentState = State.Attack;
        }
    }
    
    //   private void A
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayersDino"))
        {
            target = other.transform;
            CurrentState = State.Follow;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GotoNextPatrolPoint();
        CurrentState = State.Patrol;
    }
    
    
}