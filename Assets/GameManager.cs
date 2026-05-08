using UnityEngine;

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

    public void MakeDrunker()
    {
        if(CurrentDrunkLevel + 0.1f < 1.0f)
        {
            CurrentDrunkLevel += 0.1f;
        }

        else
        {
            CurrentDrunkLevel = 1.0f;
        }
        DrunkManager.Instance.SetDrunkness(CurrentDrunkLevel);
    }
}
