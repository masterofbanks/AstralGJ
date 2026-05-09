using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager _instance;
    public static EnemyManager Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject enemyManagerObject = Instantiate(Resources.Load<GameObject>("EnemyManager"));
                _instance = enemyManagerObject.GetComponent<EnemyManager>();

            }
            return _instance;   
        }
    }


    public EnemyFactory currentEnemyFactory;

    

    private void Awake()
    {
        currentEnemyFactory = new AsteroidFactory();
    }

    public void ConstructEnemy(Vector3 position)
    {
        currentEnemyFactory.SpawnEnemy(position, Quaternion.identity);
    }

}
