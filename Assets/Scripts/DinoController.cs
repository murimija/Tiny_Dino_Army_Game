using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DinoController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;


    [SerializeField] private GameObject partForRotation;
    public HPController HPOfAttackedTarget;

    [SerializeField] private int damage;
    [SerializeField] public Animator dinoAnimator;
    private static readonly int Walk = Animator.StringToHash("walk");

    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 targetPositionRay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = targetPositionRay - transform.position;

            partForRotation.transform.localRotation =
                direction.x >= 0 ? new Quaternion(0, 0, 0, 0) : new Quaternion(0, 1, 0, 0);

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
            {
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

    private void OnTriggerEnter(Collider other)
    {
//        Debug.Log("Enter");
        if (other.tag != "Egg") return;
        HPOfAttackedTarget = other.GetComponent<HPController>();
        StartCoroutine("AttackCoroutine");
    }

    private IEnumerator AttackCoroutine()
    {
        while (HPOfAttackedTarget != null)
        {
            HPOfAttackedTarget.takeDamage(damage);
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        HPOfAttackedTarget = null;
//        Debug.Log("Exit");
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}