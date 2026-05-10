using UnityEngine;

public class VictoryTrigger : MonoBehaviour
{
    [SerializeField] private float victorySpeedThreshold = 10;
    [SerializeField] private bool triggeredOnce = false;

    AGJ_CharacterController player;



    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Win Box")
        {
            if(player == null)
            {
                //TODO: Make a win box class that knows how to provide this data rather than get it directly like this
                player = collision.transform.parent.GetComponent<AGJ_CharacterController>();
            }

            if(!triggeredOnce && player.OrbitSpeed >= victorySpeedThreshold)
            {
                triggeredOnce = true;
                Debug.Log("You Win!");
            }
        }
    }
}
