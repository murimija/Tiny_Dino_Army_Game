using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DinoController : MonoBehaviour
{
    [Header("Prefab Settings")] [SerializeField]
    private NavMeshAgent agent;

    [SerializeField] private GameObject partForRotation;
    private HPController HPOfAttackedTarget;
    [SerializeField] public Animator dinoAnimator;

    private static readonly int Walk = Animator.StringToHash("walk");
    private static readonly int Attack = Animator.StringToHash("attack");

    [Header("Game Settings")] [SerializeField]
    public int damage;

    [SerializeField] private float openEggDistance;
    [SerializeField] private float attackWaitTime;
    [SerializeField] private float attackDistanse;

    private GameController gameController;
    private GameObject[] eggs;
    private float shortestDistanceToEdd;
    private GameObject target;
    private bool isOpeningEgg;


    private void Start()
    {
        gameController = GameController.instance;
        gameController.UpdateAliveDino();

        InvokeRepeating(nameof(UpdateTarget), 0, 0.5f);
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit);
            if (hit.collider.CompareTag("Enemy") &&
                Vector3.Distance(transform.position, hit.collider.transform.position) <= attackDistanse)
            {
                HPOfAttackedTarget = hit.collider.gameObject.GetComponent<HPController>();
                AttackEnemy();
            }
            else
            {
                var targetPositionRay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var direction = targetPositionRay - transform.position;

                partForRotation.transform.localRotation =
                    direction.x >= 0 ? new Quaternion(0, 0, 0, 0) : new Quaternion(0, 1, 0, 0);
                agent.SetDestination(hit.point);
            }
        }

        dinoAnimator.SetBool(Walk, !agent.desiredVelocity.Equals(Vector3.zero));
    }

    private void AttackEnemy()
    {
        dinoAnimator.SetTrigger(Attack);
        HPOfAttackedTarget.takeDamage(damage);
    }

    private void StartOpenEgg()
    {
        HPOfAttackedTarget = target.GetComponent<HPController>();
        isOpeningEgg = true;
        StartCoroutine(nameof(AttackCoroutine));
    }

    private IEnumerator AttackCoroutine()
    {
        while (isOpeningEgg)
        {
            HPOfAttackedTarget.takeDamage(damage);
            yield return new WaitForSeconds(attackWaitTime);
        }
    }

    private void UpdateTarget()
    {
        eggs = GameObject.FindGameObjectsWithTag("Egg");
        shortestDistanceToEdd = Mathf.Infinity;

        GameObject nearestEgg = null;

        foreach (var egg in eggs)
        {
            var distanceToEgg = Vector3.Distance(transform.position, egg.transform.position);
            if (!(distanceToEgg < shortestDistanceToEdd)) continue;
            shortestDistanceToEdd = distanceToEgg;
            nearestEgg = egg;
        }


        if (nearestEgg != null && shortestDistanceToEdd <= openEggDistance && !isOpeningEgg)
        {
            target = nearestEgg;

            StartOpenEgg();
        }
        else
        {
            target = null;
            isOpeningEgg = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        var position = transform.position;
        Gizmos.DrawWireSphere(position, openEggDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, attackDistanse);
    }

    public void Death()
    {
        gameController.DinoDeathReport();
        Destroy(gameObject);
    }
}