using System;
using UnityEngine;

[Serializable]
public abstract class EnemyFactory
{
    protected GameObject enemyPrefab;
    protected string NameOfEnemy;
    public abstract GameObject SpawnEnemy(Vector3 position, Quaternion rotation);
    public abstract void ChangeEnemyType(string newName);
}

[Serializable]
public class AsteroidFactory : EnemyFactory
{
    public AsteroidFactory()
    {
        NameOfEnemy = "Asteroid";
        enemyPrefab = Resources.Load<GameObject>(NameOfEnemy);
    }

    public override void ChangeEnemyType(string newName)
    {
        NameOfEnemy = newName;
        enemyPrefab = Resources.Load<GameObject>(NameOfEnemy);
    }

    public override GameObject SpawnEnemy(Vector3 position, Quaternion rotation)
    {
        return GameObject.Instantiate(enemyPrefab, position, rotation);
    }
}
