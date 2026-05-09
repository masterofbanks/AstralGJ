using UnityEngine;

public abstract class EnemyFactory
{
    protected GameObject enemyPrefab;
    protected string NameOfEnemy;
    public abstract void SpawnEnemy(Vector3 position, Quaternion rotation);
}

public class AsteroidFactory : EnemyFactory
{
    public AsteroidFactory()
    {
        NameOfEnemy = "Asteroid";
        enemyPrefab = Resources.Load<GameObject>(NameOfEnemy);
    }

    public override void SpawnEnemy(Vector3 position, Quaternion rotation)
    {
        Object.Instantiate(enemyPrefab, position, rotation);
    }
}
