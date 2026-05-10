using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryTrigger : MonoBehaviour
{
    [SerializeField] private float victorySpeedThreshold = 10;
    public float VictorySpeedThreshold { get { return victorySpeedThreshold; } }
    [SerializeField] private bool triggeredOnce = false;
    private float victorySequenceProgression = 0f;
    [SerializeField] private float victorySequenceDelayBeforeFadeIn = .5f;
    [SerializeField] private float fadeTimeInSeconds = 2f;
    [SerializeField] private float victorySequenceDelayBeforeLevelLoad = 4f;
    [SerializeField] private string levelToLoad = "EthanScene";
    [SerializeField] private float timeOrbitingBeforeVictoryTriggers = 1f;
    private float timeSpentOrbiting = 0;

    AGJ_CharacterController player;
    FadeScreen fadeScreen;
    

    private void Start()
    {
        fadeScreen = FindAnyObjectByType<FadeScreen>();
        Debug.Assert(fadeScreen != null, "There needs to be a FadeScreen child of the scene's canvas for the victory manager to work.");
        if (fadeScreen == null) Debug.Break();

        timeSpentOrbiting = timeOrbitingBeforeVictoryTriggers;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Win Box")
        {
            timeSpentOrbiting += Time.fixedDeltaTime;

            if(player == null)
            {
                //TODO: Make a win box class that knows how to provide this data rather than get it directly like this
                player = collision.transform.parent.GetComponent<AGJ_CharacterController>();
            }

            if(timeSpentOrbiting >= timeOrbitingBeforeVictoryTriggers && !triggeredOnce && player.OrbitSpeed >= victorySpeedThreshold)
            {
                triggeredOnce = true;
                StartCoroutine(VictorySequence());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Win Box")
        {
            timeSpentOrbiting = 0;
        }
    }

    private IEnumerator VictorySequence()
    {
        victorySequenceProgression = 0;

        while (true)
        {
            victorySequenceProgression += Time.deltaTime;

            if(victorySequenceProgression >= victorySequenceDelayBeforeFadeIn)
            {
                victorySequenceDelayBeforeFadeIn = float.MaxValue;
                fadeScreen.StartFadeIn(1 / fadeTimeInSeconds);
            }
            
            if(victorySequenceProgression >= victorySequenceDelayBeforeLevelLoad)
            {
                victorySequenceDelayBeforeLevelLoad = float.MaxValue;
                SceneManager.LoadScene(levelToLoad);
            }

            yield return null;
        }
    }
}
