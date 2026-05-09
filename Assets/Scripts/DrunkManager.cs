using UnityEngine;

public class DrunkManager : MonoBehaviour
{
    private static DrunkManager _instance;
    public static DrunkManager Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject drunkManagerObject = Instantiate(Resources.Load<GameObject>("DrunkManager"));
                _instance = drunkManagerObject.GetComponent<DrunkManager>();
            }

            return _instance;
        }
    }


    [Header("Shader Values")]
    [SerializeField] private Material DrunkShaderMaterial;


    public void SetDrunkness(float drunknessLevel)
    {
        if(drunknessLevel < 0 || drunknessLevel > 1.0f)
        {
            Debug.Log($"Improper drunkness level set: {drunknessLevel}");
            return;
        }
        DrunkShaderMaterial.SetFloat("_Blend", drunknessLevel);
    }

    private void OnDestroy()
    {
        DrunkShaderMaterial.SetFloat("_Blend", 0);
    }
}
