using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public Vector2 DirectionOfTravel;
    public abstract IEnumerator DeathRoutine();
}


public class AsteroidBehavior : Enemy
{
    [Header("Physics Values")]
    [SerializeField] private float RotationSpeed = 5f;
    public float speed = 10f;


    [Header("Player Detection")]
    [SerializeField] private float DetectionRadius = 10f;
    [SerializeField] private LayerMask PlayerLayer;
    [SerializeField] private GameObject ExplosionSFX;
    [SerializeField] private float MaxTimeAlive = 10f;

    private bool _detectedPlayer = false;
    private bool _hitSomething;
    private float _timeAlive = 0f;
    private bool dead = false;
    //components
    private Rigidbody2D _rb2D;
    private Transform _target;
    private BoxCollider2D _boxColl;
    private Animator anime;
    private SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>(); 
        anime = GetComponent<Animator>();
        _boxColl = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();   
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb2D.angularVelocity = RotationSpeed;
        if(!GameManager.Instance.dead)
            _target = GameManager.Instance.Player.GetComponent<Transform>();

    }

    private void Update()
    {

        Collider2D hit = Physics2D.OverlapCircle(transform.position, DetectionRadius, PlayerLayer);
        _detectedPlayer = hit != null;

        if (_detectedPlayer && _target != null)
        {
            DirectionOfTravel = ((Vector2)_target.position - _rb2D.position).normalized;
            _timeAlive = 0;
        }


        if (!_hitSomething)
        {
            _rb2D.linearVelocity = DirectionOfTravel * speed;
            if (!_detectedPlayer)
            {
                if (_timeAlive < MaxTimeAlive)
                {
                    _timeAlive += Time.deltaTime;

                }

                else
                {
                    StartCoroutine(DeathRoutine());
                }

            }
            

        }


    }

    

    public override IEnumerator DeathRoutine()
    {
        if (!dead)
        {
            dead = true;
            if (_spriteRenderer.isVisible)
                Instantiate(ExplosionSFX, transform.position, Quaternion.identity);
            _rb2D.linearVelocity = Vector2.zero;
            _rb2D.angularVelocity = 0f;
            _hitSomething = true;
            anime.SetBool("Destroyed", _hitSomething);
            _boxColl.enabled = false;
            yield return new WaitForSeconds(0.35f);
            Destroy(gameObject);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerHitbox"))
        {
            AGJ_CharacterController playerScript = collision.gameObject.GetComponentInParent<AGJ_CharacterController>();
            playerScript.PlayerDeathRoutine();
            GameManager.Instance.MakePlayerDead();
        }
    }

}
