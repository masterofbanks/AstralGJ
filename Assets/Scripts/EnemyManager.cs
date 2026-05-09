using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    


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
    }

    public GameObject ConstructEnemy(Transform position, Quaternion orientation)
    {
        return currentEnemyFactory.SpawnEnemy(position.position, orientation);
    }

}
