using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArea : MonoBehaviour
{
    [SerializeField] private GameObject areaForSpawnEnemies;
    private Bounds bounds;
    [SerializeField] private GameObject patrolPointPref;
    [SerializeField] private int numOfPatrolPoints = 4;
    private Transform[] patrolPointArray;

    [SerializeField] private float timeSpanWait;
    [SerializeField] private int maxNumEnemyOnArea;
    [SerializeField] private GameObject enemyPref;
    public int numOfEnemyOnAreaNow = 0;

    void Start()
    {
        patrolPointArray = new Transform[numOfPatrolPoints];
        bounds = areaForSpawnEnemies.GetComponent<Renderer>().bounds;
        GameObject patrolPointsСollection = new GameObject();
        patrolPointsСollection.name = "PatrolPointsСollection";
        for (int i = 0; i < numOfPatrolPoints; i++)
        {
            var patrolPoint = Instantiate(patrolPointPref,
                new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.max.y,
                    Random.Range(bounds.min.z, bounds.max.z)), Quaternion.identity, patrolPointsСollection.transform);

            patrolPointArray[i] = patrolPoint.transform;
        }

        StartCoroutine("SpawnEnemy");
    }

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            
            if (numOfEnemyOnAreaNow < maxNumEnemyOnArea)
            {
                var enemy = Instantiate(enemyPref,
                    new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.max.y,
                        Random.Range(bounds.min.z, bounds.max.z)), Quaternion.identity);

                enemy.GetComponent<EnemyController>().patrolPointsArray = patrolPointArray;

                numOfEnemyOnAreaNow++;
            }

            yield return new WaitForSeconds(timeSpanWait);
        }
    }
}