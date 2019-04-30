using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [Header("Prefab settings")] [SerializeField]
    private Animator animator;

    [NonSerialized] public Transform[] patrolPointsArray;

    [SerializeField] private float attackWaitTime;
    [SerializeField] private GameObject partForRotation;

    [Header("Game settings")] [SerializeField]
    private int damage;

    [SerializeField] private float attackRange;
    [SerializeField] private float observeRange;

    private int numOfActualPatrolPoint;
    private NavMeshAgent enemyAgent;
    private GameObject target;
    private HPController HPOfAttackedTarget;
    private float timeToUpdate;
    private float MinDistToPatrollPoint;
    private Vector3 nextPatrollPoint;

    private static readonly int IsAttack = Animator.StringToHash("isAttack");
    private bool isAttack;
    private static readonly int TakeDamage = Animator.StringToHash("takeDamage");
    private GameObject[] dinos;
    private float shortestDistance;

    private void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        MinDistToPatrollPoint = enemyAgent.stoppingDistance;

        InvokeRepeating(nameof(UpdateTarget), 0, 0.5f);
    }

    private void GotoNextPatrolPoint()
    {
        nextPatrollPoint = patrolPointsArray[Random.Range(0, patrolPointsArray.Length)].position;
        enemyAgent.SetDestination(nextPatrollPoint);

        partForRotation.transform.localRotation =
            (nextPatrollPoint - transform.position).x >= 0
                ? new Quaternion(0, 1, 0, 0)
                : new Quaternion(0, 0, 0, 0);
    }

    private void Patrol()
    {
        if (enemyAgent.remainingDistance <= MinDistToPatrollPoint)
        {
            GotoNextPatrolPoint();
        }
    }

    private void Follow()
    {
        enemyAgent.SetDestination(target.transform.position);
    }

    #region Attack

    private void StartAttack()
    {
        isAttack = true;
        HPOfAttackedTarget = target.GetComponent<HPController>();
        StartCoroutine(nameof(Attack));

        animator.SetBool(IsAttack, true);
    }

    private IEnumerator Attack()
    {
        while (isAttack)
        {
            HPOfAttackedTarget.takeDamage(damage);
            target.GetComponent<DinoController>().dinoAnimator.SetTrigger(TakeDamage);
            yield return new WaitForSeconds(attackWaitTime);
        }
    }

    private void StopAttack()
    {
        isAttack = false;
        animator.SetBool(IsAttack, false);
    }

    #endregion

    private void UpdateTarget()
    {
        dinos = GameObject.FindGameObjectsWithTag("PlayersDino");
        shortestDistance = Mathf.Infinity;
        GameObject nearestDino = null;
        foreach (var dino in dinos)
        {
            var distanceToEnemy = Vector3.Distance(transform.position, dino.transform.position);
            if (!(distanceToEnemy < shortestDistance)) continue;
            shortestDistance = distanceToEnemy;
            nearestDino = dino;
        }

        if (nearestDino != null && shortestDistance <= attackRange && !isAttack)
        {
            target = nearestDino;
            StartAttack();
        }
        else if (nearestDino != null && shortestDistance <= observeRange)
        {
            target = nearestDino;
            StopAttack();
            enemyAgent.speed = 0.35f;
            Follow();
        }
        else
        {
            target = null;
            StopAttack();
            enemyAgent.speed = 0.2f;
            Patrol();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        var position = transform.position;
        Gizmos.DrawWireSphere(position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position, observeRange);
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}