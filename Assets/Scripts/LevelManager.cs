using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public bool HasTutorial = false;
    public TextMeshProUGUI TutorialTextBox;
    public string TutorialText;
    private void Awake()
    {
        TutorialTextBox.enabled = HasTutorial;
    }

    private void Start()
    {
        if (HasTutorial)
        {
            TutorialTextBox.text = TutorialText;
        }
    }


}
