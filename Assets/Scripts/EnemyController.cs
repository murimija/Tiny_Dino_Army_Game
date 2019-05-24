using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [Header("Game settings")] [SerializeField]
    private int damage;

    [SerializeField] private float attackRange;
    [SerializeField] private float observeRange;

    [Header("Prefab settings")] [SerializeField]
    private Animator animator;

    [NonSerialized] public Transform[] patrolPointsArray;

    [SerializeField] private float attackWaitTime;
    [SerializeField] private GameObject partForRotation;
    [SerializeField] private GameObject exclamationMarkPref;
    [SerializeField] private Transform exclMarkSpawnPos;

    private int numOfActualPatrolPoint;
    private NavMeshAgent enemyAgent;
    private GameObject target;
    private HPController HPOfAttackedTarget;
    private float timeToUpdate;
    private float MinDistToPatrollPoint;
    private Vector3 nextPatrollPoint;
    
    private static readonly int IsAttack = Animator.StringToHash("isAttack");
    private static readonly int TakeDamage = Animator.StringToHash("takeDamage");
    private GameObject[] dinos;
    private float shortestDistance;

    private GameController gameController;

    private const float updateStateTime = 0.5f;

    private enum State
    {
        Attack,
        Follow,
        Patrol
    }

    private State currentState;

    private void ChangeState(State newState)
    {
        if (currentState == newState) return;

        // ReSharper disable once SwitchStatementMissingSomeCases
        switch (newState)
        {
            case State.Patrol:
                currentState = State.Patrol;
                StartCoroutine(nameof(Patrol));
                break;
            case State.Follow:
                currentState = State.Follow;
                StartCoroutine(nameof(Follow));
                break;
            default:
                currentState = State.Attack;
                StartCoroutine(nameof(Attack));
                break;
        }
    }

    private void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        MinDistToPatrollPoint = enemyAgent.stoppingDistance;

        gameController = GameController.instance;

        StartCoroutine(nameof(UpdateTargetAndState));
    }

    private IEnumerator Patrol()
    {
        while (currentState == State.Patrol)
        {
            if (enemyAgent.remainingDistance <= MinDistToPatrollPoint)
            {
                nextPatrollPoint = patrolPointsArray[Random.Range(0, patrolPointsArray.Length)].position;
                enemyAgent.SetDestination(nextPatrollPoint);

                partForRotation.transform.localRotation =
                    (nextPatrollPoint - transform.position).x >= 0
                        ? new Quaternion(0, 1, 0, 0)
                        : new Quaternion(0, 0, 0, 0);
            }

            yield return new WaitForSeconds(updateStateTime);
        }
    }

    private IEnumerator Follow()
    {
        Destroy(
            Instantiate(exclamationMarkPref, exclMarkSpawnPos.position, Quaternion.identity,
                gameObject.transform), 1f);

        while (currentState == State.Follow && enemyAgent!= null)
        {
            enemyAgent.SetDestination(target.transform.position);
            yield return new WaitForSeconds(updateStateTime);
        }
    }

    private IEnumerator Attack()
    {
        HPOfAttackedTarget = target.GetComponent<HPController>();

        animator.SetBool(IsAttack, true);

        while (currentState == State.Attack)
        {
            if (HPOfAttackedTarget != null)
            {
                HPOfAttackedTarget.takeDamage(damage);
                target.GetComponent<DinoController>().dinoAnimator.SetTrigger(TakeDamage);
            }

            yield return new WaitForSeconds(attackWaitTime);
        }

        animator.SetBool(IsAttack, false);
    }

    private IEnumerator UpdateTargetAndState()
    {
        while (true)
        {
            shortestDistance = Mathf.Infinity;
            GameObject nearestDino = null;
            foreach (var dino in gameController.listOfDino)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, dino.transform.position);
                if (!(distanceToEnemy < shortestDistance)) continue;
                shortestDistance = distanceToEnemy;
                nearestDino = dino;
            }

            if (nearestDino != null && shortestDistance <= attackRange)
            {
                target = nearestDino;
                ChangeState(State.Attack);
            }
            else if (nearestDino != null && shortestDistance <= observeRange)
            {
                target = nearestDino;
                ChangeState(State.Follow);
            }
            else
            {
                target = null;
                ChangeState(State.Patrol);
            }
            yield return new WaitForSeconds(updateStateTime);
        }
        // ReSharper disable once IteratorNeverReturns
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