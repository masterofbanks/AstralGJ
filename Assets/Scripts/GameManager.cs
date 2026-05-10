using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(this.gameObject);
        }


    }

    private void Start()
    {
        DrunkManager.Instance.SetDrunkness(0.0f);
    }

    [Header("Drunk Values")]
    [SerializeField] private float CurrentDrunkLevel = 0.0f;
    [SerializeField] private float IncreaseInDrunkness = 0.1f;

    [Header("Player Information")]
    public Transform Player;
    public bool dead { get; private set; }



    /// <summary>
    /// Over a set duration of time, make the screen drunker
    /// </summary>
    /// <param name="durationOfLerp"></param>
    public void MakeDrunker(float durationOfLerp)
    {
        float newDrunkLevel = 0;
        if (CurrentDrunkLevel + IncreaseInDrunkness < 1.0f)
        {
            newDrunkLevel = CurrentDrunkLevel + IncreaseInDrunkness;
        }

        else
        {
            newDrunkLevel = 1.0f;
        }

        StartCoroutine(LerpDrunkRoutine(durationOfLerp, CurrentDrunkLevel, newDrunkLevel));
    }

    /// <summary>
    /// Remove Any Drunkness from the screen
    /// </summary>
    public void RemoveDrunkness()
    {
        StartCoroutine(LerpDrunkRoutine(1.0f, CurrentDrunkLevel, 0));
    }


    private IEnumerator LerpDrunkRoutine(float duration, float start, float end)
    {
        float timeElapsed = 0;
        float newDrunkValue = start;
        while(timeElapsed < duration)
        {
            newDrunkValue = Mathf.Lerp(start, end, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            DrunkManager.Instance.SetDrunkness(newDrunkValue);

            yield return null;
        }

        newDrunkValue = end;
        DrunkManager.Instance.SetDrunkness(newDrunkValue);
        CurrentDrunkLevel = newDrunkValue;
    }

    public void MakePlayerDead()
    {
        dead = true;
        StartCoroutine(RestartRoutine());
    }

    IEnumerator RestartRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    
}
