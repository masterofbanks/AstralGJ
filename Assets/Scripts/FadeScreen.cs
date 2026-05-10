using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeScreen : MonoBehaviour
{
    [SerializeField] private float fadeSpeed;
    [SerializeField] private Image image;
    [SerializeField] private bool startWithFadeOut;
    private float fadeProgress;

    private void Awake()
    {
        if (startWithFadeOut)
        {
            StartFadeOut(fadeSpeed);
        }
    }

    public void StartFadeIn(float speed)
    {
        fadeSpeed = speed;

        StartCoroutine(FadeInRoutine());
    }
    public void StartFadeOut(float speed)
    {
        fadeSpeed = speed;

        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        fadeProgress = 0;

        while(fadeProgress < 1)
        {
            fadeProgress += Time.deltaTime * fadeSpeed;
            image.color = new Color(1,1,1,fadeProgress);

            yield return null;
        }
    }

    private IEnumerator FadeOutRoutine()
    {
        fadeProgress = 1;

        while(fadeProgress > 0)
        {
            fadeProgress -= Time.deltaTime * fadeSpeed;
            image.color = new Color(1,1,1,fadeProgress);

            yield return null;
        }
    }
}
