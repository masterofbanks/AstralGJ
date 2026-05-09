using UnityEngine;

public class AsteroidBehavior : MonoBehaviour
{
    [Header("Physics Values")]
    [SerializeField] private float RotationSpeed = 5f;
    public float speed = 10f;


    //components
    private Rigidbody2D _rb2D;
    private Transform _target;

    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>(); 
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb2D.angularVelocity = RotationSpeed;
        _target = GameManager.Instance.Player.GetComponent<Transform>();

    }

    private void Update()
    {
        Vector2 directionToTravel = ((Vector2)_target.position - _rb2D.position).normalized;
        _rb2D.linearVelocity = directionToTravel * speed;


    }
}
