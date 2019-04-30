using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DinoController : MonoBehaviour
{
    [Header("Prefab Settings")] [SerializeField]
    private NavMeshAgent agent;

    [SerializeField] private GameObject partForRotation;
    [NonSerialized] public HPController HPOfAttackedTarget;
    [SerializeField] public Animator dinoAnimator;

    private static readonly int Walk = Animator.StringToHash("walk");
    private static readonly int Attack = Animator.StringToHash("attack");

    [Header("Game Settings")] 
    [SerializeField] public int damage;

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

        InvokeRepeating("UpdateTarget", 0, 0.5f);
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit);
            if (hit.collider.CompareTag("Enemy") && 
                Vector3.Distance(transform.position, hit.collider.transform.position) <= attackDistanse)
            {
                Debug.Log(hit.collider.name);
                HPOfAttackedTarget = hit.collider.gameObject.GetComponent<HPController>();
                AttackEnemy();
            }
            else
            {
                Vector3 targetPositionRay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 direction = targetPositionRay - transform.position;

                partForRotation.transform.localRotation =
                    direction.x >= 0 ? new Quaternion(0, 0, 0, 0) : new Quaternion(0, 1, 0, 0);
                agent.SetDestination(hit.point);
            }
        }

        if (agent.desiredVelocity.Equals(Vector3.zero))
        {
            dinoAnimator.SetBool(Walk, false);
        }
        else
        {
            dinoAnimator.SetBool(Walk, true);
        }
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
        StartCoroutine("AttackCoroutine");
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
        Gizmos.DrawWireSphere(transform.position, openEggDistance);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistanse);
        
    }

    public void Death()
    {
        gameController.DinoDeathReport();
        Destroy(gameObject);
    }
}