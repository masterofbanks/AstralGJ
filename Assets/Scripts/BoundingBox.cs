using Unity.VisualScripting;
using UnityEngine;

public enum BoundingBoxDirection
{
    Horizontal,
    Vertical
}

public class BoundingBox : MonoBehaviour
{

    [SerializeField] BoundingBox connectedBox;
    [SerializeField] BoundingBoxDirection direction;
    [SerializeField] float inactiveTimer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(connectedBox == null)
        {
            Debug.LogWarning("BoundingBox objects only work if there is a connected bounding box.");
            return;
        }
        else if(connectedBox.direction != direction)
        {
            Debug.LogWarning("BoundingBox objects only work if there is a connected bounding box, and they both share the same direction.");
            return;
        }

        if (inactiveTimer <= 0 && collision.gameObject.tag == "PlayerHitbox")
        {
            if (direction == BoundingBoxDirection.Horizontal)
            {
                collision.transform.parent.position = new Vector2(collision.transform.position.x, connectedBox.transform.position.y);
            }
            else
            {
                collision.transform.parent.position = new Vector2(connectedBox.transform.position.x, collision.transform.position.y);
            }

            connectedBox.inactiveTimer = 1f;
        }
    }

    private void Update()
    {
        if(inactiveTimer > 0)
        {
            inactiveTimer -= Time.deltaTime;
        }
    }
}
