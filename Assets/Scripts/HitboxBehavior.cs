using System.Collections.Generic;
using UnityEngine;

public class HitboxBehavior : MonoBehaviour
{
    public GameObject DrinkingSFX;
    private List<IWineCollisionListener> wineCollisionListeners = new();

    public void UpdatePosition(Vector3 position)
    {
        transform.position = position;
    }

    public void AddWineCollisionListener(IWineCollisionListener listener)
    {
        wineCollisionListeners.Add(listener);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wine"))
        {
            foreach(var listener in wineCollisionListeners)
            {
                listener?.RunOnWineCollected();
            }

            GameManager.Instance.MakeDrunker(1.0f);
            Instantiate(DrinkingSFX, transform.position, Quaternion.identity);  
            Destroy(collision.gameObject);
        }
    }
}
