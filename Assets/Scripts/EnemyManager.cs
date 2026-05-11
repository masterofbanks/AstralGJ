using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public string enemyName = string.Empty;
    public EnemyFactory currentEnemyFactory;

    

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
        }
        currentEnemyFactory = new AsteroidFactory();

        if(enemyName !=  string.Empty)
        {
            currentEnemyFactory.ChangeEnemyType(enemyName);
        }
    }

    public GameObject ConstructEnemy(Transform position, Quaternion orientation)
    {
        return currentEnemyFactory.SpawnEnemy(position.position, orientation);
    }

}
