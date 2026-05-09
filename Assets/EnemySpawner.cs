using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
    public void SpawnEnemyHere()
    {
        GameObject enemy = EnemyManager.Instance.ConstructEnemy(transform, Quaternion.identity);
        enemy.GetComponent<Enemy>().DirectionOfTravel = InitializeDirection(transform.right);
    }

    private Vector2 InitializeDirection(Vector2 facingDirection)
    {
        float aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        //make sure its between 0 and 2pi
        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }


        Vector2 aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;
        aimDirection.Normalize();
        return aimDirection;
    }
}
