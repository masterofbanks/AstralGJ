using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public bool HasTutorial = false;
    public TextMeshPro TutorialTextBox;
    public string TutorialText;
    public Transform PlayerTutorialPoint;
    private void Awake()
    {
        TutorialTextBox = GetComponent<TextMeshPro>();
        TutorialTextBox.enabled = HasTutorial;
    }

    private void Start()
    {
        if (HasTutorial)
        {
            TutorialTextBox.text = TutorialText;
        }
    }

    private void Update()
    {
        TutorialTextBox.transform.position = PlayerTutorialPoint.position;
    }
}
