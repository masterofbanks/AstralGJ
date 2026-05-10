using UnityEngine;

public class HitboxBehavior : MonoBehaviour
{
    public GameObject DrinkingSFX;
    public void UpdatePosition(Vector3 position)
    {
        transform.position = position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wine"))
        {
            GameManager.Instance.MakeDrunker(1.0f);
            Instantiate(DrinkingSFX, transform.position, Quaternion.identity);  
            Destroy(collision.gameObject);
        }
    }
}
