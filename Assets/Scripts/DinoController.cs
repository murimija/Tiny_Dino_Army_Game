﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DinoController : MonoBehaviour
{
    [Header("Prefab Settings")] [SerializeField]
    private NavMeshAgent agent;

    [SerializeField] private GameObject partForRotation;
    private HPController HPOfAttackedTarget;
    [SerializeField] public Animator dinoAnimator;
    [SerializeField] public GameObject highlightPref;

    private static readonly int Walk = Animator.StringToHash("walk");
    private static readonly int Attack = Animator.StringToHash("attack");

    private bool isSelected;

    [Header("Game Settings")] [SerializeField]
    public int damage;

    [SerializeField] private float openEggDistance;
    [SerializeField] private float openEggWaitTime;
    [SerializeField] private float attackDistance;

    private int openEggDamage = 1;

    private GameController gameController;
    private GameObject[] eggs;
    private float shortestDistanceToEdd;
    private GameObject target;
    private bool isOpeningEgg;


    private void Start()
    {
        gameController = GameController.instance;
        gameController.AddDinoToList(gameObject);
        gameController.UpdateAliveDino();

        StartCoroutine(nameof(CheckNearEgg));
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
            {
                if ((hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Boss")) &&
                    Vector3.Distance(transform.position, hit.collider.transform.position) <= attackDistance &&
                    isSelected)
                {
                    HPOfAttackedTarget = hit.collider.gameObject.GetComponent<HPController>();
                    AttackEnemy();
                }
            }
        }

        dinoAnimator.SetBool(Walk, !agent.desiredVelocity.Equals(Vector3.zero));
    }

    public void GoToNewPlace(Vector3 newDestination)
    {
        agent.SetDestination(newDestination);
        var direction = newDestination - transform.position;

        partForRotation.transform.localRotation =
            direction.x >= 0 ? new Quaternion(0, 0, 0, 0) : new Quaternion(0, 1, 0, 0);
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
        StartCoroutine(nameof(OpenEggCoroutine));
    }

    private IEnumerator OpenEggCoroutine()
    {
        while (isOpeningEgg)
        {
            if (HPOfAttackedTarget != null)
            {
                HPOfAttackedTarget.takeDamage(openEggDamage);
            }
            else
            {
                target = null;
                isOpeningEgg = false;
            }

            yield return new WaitForSeconds(openEggWaitTime);
        }
    }

    private IEnumerator CheckNearEgg()
    {
        while (true)
        {
            if (!isOpeningEgg)
            {
                shortestDistanceToEdd = Mathf.Infinity;

                GameObject nearestEgg = null;

                foreach (var egg in gameController.listOfEggs)
                {
                    float distanceToEgg = Vector3.Distance(transform.position, egg.transform.position);
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

            yield return new WaitForSeconds(0.5f);
        }
        // ReSharper disable once IteratorNeverReturns
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        var position = transform.position;
        Gizmos.DrawWireSphere(position, openEggDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, attackDistance);
    }

    public void SetHighlightState(bool state)
    {
        highlightPref.SetActive(state);
        isSelected = state;
    }

    public void Death()
    {
        gameController.DinoDeathReport(gameObject);
        Destroy(gameObject);
    }
}