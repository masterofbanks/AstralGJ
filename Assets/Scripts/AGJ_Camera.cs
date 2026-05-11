using System.Collections;
using UnityEngine;

public class AGJ_Camera : MonoBehaviour
{
    public static AGJ_Camera Instance;

    [SerializeField] private AGJ_CharacterController player;
    [SerializeField] private float reorientationSpeed;
    [SerializeField] private float reorientationSpeedDivisor = 50f;
    [SerializeField] private Quaternion rotation;

    [SerializeField] private Quaternion beforeRotation = Quaternion.identity;
    
    [SerializeField] private Vector3 localPosition;
    [SerializeField] private Vector3 localPosition_Original;
    [SerializeField] private Vector3 localPosition_AfterOrbit;
    
    [SerializeField] private bool following;

    private Coroutine reorientationRoutine = null;
    private Coroutine centeringRoutine = null;

    [Space(15)]
    [Header("Camera Stuff")]
    private Camera cameraComponent = null;
    private float cameraSize = 13.5f;
    [SerializeField] private float cameraSize_PlayerSpeed_Divisor = 5f;
    private float playerSpeed_InitialValue;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        localPosition = transform.position - player.transform.position;
        localPosition_Original = localPosition;

        rotation = transform.rotation;

        cameraComponent = GetComponent<Camera>();

        cameraSize = cameraComponent.orthographicSize;
        playerSpeed_InitialValue = player.SpeedCap;
    }

    // Update is called once per frame
    void Update()
    {
        if (following)
        {
            transform.SetPositionAndRotation(player.transform.position + localPosition, rotation);
        }
    }

    public void StopFollowing()
    {
        following = false;
        if(reorientationRoutine != null)
        {
            StopCoroutine(reorientationRoutine);
            reorientationRoutine = null;
        }

        centeringRoutine = StartCoroutine(CenterCamera());
    }
    public void StartFollowing()
    {
        following = true;
        if (centeringRoutine != null)
        {
            StopCoroutine(centeringRoutine);
            centeringRoutine = null;
        }

        beforeRotation = transform.rotation;

        localPosition = transform.position - player.transform.position;
        localPosition_AfterOrbit = localPosition;

        reorientationSpeed = player.SpeedCap / reorientationSpeedDivisor;
        UpdateCameraSize();

        reorientationRoutine = StartCoroutine(ReorientCamera());
    }
    
    private IEnumerator ReorientCamera()
    {
        float t = 0;

        while (t <= 1)
        {
            //Move the camera more smoothly until it reaches the correct local position
            //Shrink the temp offset until it becomes Vector3.zero
            localPosition = Vector3.Lerp(localPosition_AfterOrbit, GetLocalPositionRelativeToRightVector(), t);

            //Rotate the camera until its forward vector aligns with the player's direction
            rotation = Quaternion.Lerp(beforeRotation, player.transform.localRotation, t);

            t += Time.deltaTime * reorientationSpeed;

            yield return new WaitForEndOfFrame();
        }

        reorientationRoutine = null;
    }

    public void UpdateCameraSize()
    {
        cameraComponent.orthographicSize = cameraSize + player.SpeedCap / playerSpeed_InitialValue / cameraSize_PlayerSpeed_Divisor;
    }

    private IEnumerator CenterCamera()
    {
        float t = 0;

        while (t <= 1)
        {
            Vector3 orbitTargetPosition = Vector3.zero;
            if (player.OrbitTarget == null) yield return null;
            else orbitTargetPosition = player.OrbitTarget.position;

            localPosition = Vector3.Lerp(localPosition, Vector3.zero, t);
            localPosition.z = -10;

            transform.SetPositionAndRotation(orbitTargetPosition + localPosition, rotation);

            t += Time.deltaTime * reorientationSpeed;

            yield return null;
        }

        centeringRoutine = null;
    }

    private Vector3 GetLocalPositionRelativeToRightVector()
    {
        Vector3 localPosition = player.transform.right * localPosition_Original.magnitude;
        localPosition.z = -10;
        return localPosition;
    }
}
