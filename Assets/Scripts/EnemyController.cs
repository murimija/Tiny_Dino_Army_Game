using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [NonSerialized] public Transform[] patrolPointsArray;
    [SerializeField] private int damage;
    [SerializeField] private float attackWaitTime;
    [SerializeField] private GameObject partForRotation;

    private int numOfActualPatrolPoint = 0;
    private NavMeshAgent enemyAgent;
    private Transform target;
    private HPController HPOfAttacedTarget;
    private float timeToUpdate = 0.5f;
    private float MinDistToPatrollPoint;
    private Vector3 nextPatrollPoint;


    public enum State
    {
        Patrol,
        Follow,
        Attack
    }

    public State CurrentState = State.Patrol;
    private static readonly int IsAttack = Animator.StringToHash("isAttack");
    private bool isAtack;
    private static readonly int TakeDamage = Animator.StringToHash("takeDamage");


    private void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        MinDistToPatrollPoint = enemyAgent.stoppingDistance;
        GotoNextPatrolPoint();

        StartCoroutine("CheckStateAndUpdate");
    }

    void GotoNextPatrolPoint()
    {
        nextPatrollPoint = patrolPointsArray[Random.Range(0, patrolPointsArray.Length)].position;
        enemyAgent.SetDestination(nextPatrollPoint);

        partForRotation.transform.localRotation =
            (nextPatrollPoint - transform.position).x >= 0
                ? new Quaternion(0, 1, 0, 0)
                : new Quaternion(0, 0, 0, 0);
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
                if (!isAtack)
                {
                    StartAttack();
                }
            }

            yield return new WaitForSeconds(timeToUpdate);
        }
    }

    private void Follow()
    {
        enemyAgent.SetDestination(target.position);
        if (enemyAgent.remainingDistance <= MinDistToPatrollPoint + 0.2f)
        {
            CurrentState = State.Attack;
        }
    }

    #region Attack

    private void StartAttack()
    {
        if (target != null)
        {
            animator.SetBool(IsAttack, true);
            HPOfAttacedTarget = target.GetComponent<HPController>();
            StartCoroutine("Attack");
            isAtack = true;
        }
    }

    IEnumerator Attack()
    {
        while (true)
        {
            if (target != null && Vector3.Distance(target.position, gameObject.transform.position) < 0.4f)
            {
                HPOfAttacedTarget.takeDamage(damage);
                target.GetComponent<DinoController>().dinoAnimator.SetTrigger("takeDamage");
            }

            else
            {
                StopAttack();
            }

            yield return new WaitForSeconds(attackWaitTime);
        }
    }

    private void StopAttack()
    {
        animator.SetBool(IsAttack, false);
        StopCoroutine("Attack");
        isAtack = false;
        GotoNextPatrolPoint();
        CurrentState = State.Patrol;
    }

    #endregion

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
        StopAttack();
    }
}