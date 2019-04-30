using System.Collections;
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
    public int numOfEnemyOnAreaNow;

    private void Start()
    {
        patrolPointArray = new Transform[numOfPatrolPoints];
        bounds = areaForSpawnEnemies.GetComponent<Renderer>().bounds;
        var patrolPointsСollection = new GameObject();
        patrolPointsСollection.transform.SetParent(gameObject.transform);
        patrolPointsСollection.name = "PatrolPointsСollection";
        for (var i = 0; i < numOfPatrolPoints; i++)
        {
            var patrolPoint = Instantiate(patrolPointPref,
                new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.max.y,
                    Random.Range(bounds.min.z, bounds.max.z)), Quaternion.identity, patrolPointsСollection.transform);

            patrolPointArray[i] = patrolPoint.transform;
        }

        StartCoroutine(nameof(SpawnEnemy));
    }

    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            if (numOfEnemyOnAreaNow < maxNumEnemyOnArea)
            {
                var enemy = Instantiate(enemyPref,
                    new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.max.y,
                        Random.Range(bounds.min.z, bounds.max.z)), Quaternion.identity, gameObject.transform);

                enemy.GetComponent<EnemyController>().patrolPointsArray = patrolPointArray;

                numOfEnemyOnAreaNow++;
            }

            yield return new WaitForSeconds(timeSpanWait);
        }
    }
}