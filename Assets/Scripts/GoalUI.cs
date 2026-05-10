using TMPro;
using UnityEngine;

public class GoalUI : MonoBehaviour
{
    private VictoryTrigger victoryTrigger;
    private AGJ_CharacterController player;
    [SerializeField] public TextMeshProUGUI speedText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        victoryTrigger = FindAnyObjectByType<VictoryTrigger>();
        if( victoryTrigger == null)
        {
            Debug.LogError("\n\n GoalUI could not find a VictoryTrigger in the scene.");
            Debug.Break();
        }
        
        player = FindAnyObjectByType<AGJ_CharacterController>();
        if( player == null)
        {
            Debug.LogError("\n\n GoalUI could not find an AGJ_CharacterController in the scene.");
            Debug.Break();
        }
        
        if( speedText == null)
        {
            Debug.LogError("\n\n GoalUI needs a text component assigned to it that represents the speed of the player.");
            Debug.Break();
        }
    }

    // Update is called once per frame
    void Update()
    {
        speedText.text = $"Current Speed: {player.OrbitSpeed}\r\nTarget Speed: {victoryTrigger.VictorySpeedThreshold}";
    }
}
