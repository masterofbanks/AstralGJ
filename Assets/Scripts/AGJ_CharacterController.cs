using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public enum MovementBehavior 
{
    Normal,
    Orbiting
}

public class AGJ_CharacterController : MonoBehaviour, IWineCollisionListener
{
    [SerializeField] private float speedCap;
    public float SpeedCap { get { return speedCap; } }

    [SerializeField] private float speedCapGrowth;
    [Space(10)]
    
    [SerializeField] private float acceleration;
    [SerializeField] private float accelerationGrowth;
    [Space(10)]
    
    [SerializeField] private float raycastDistance = 2f;
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private Vector2 direction;
    [SerializeField] private Vector2 steerForce;
    [SerializeField] private float steerStrength;
    [SerializeField] private float vehicleRotationSpeed = 5f;
    [SerializeField] private float vehicleRotationAcceleration = 5f;
    [SerializeField] private float vehicleRotationDecay = 5f;
    [SerializeField] private Transform sprite;



    //A chain of responsibility that governs how the character reacts to certain obstacles in the environment
    [SerializeField] private List<IEnvironmentalInteractionHandler> environmentalInteractionHandlers = new();
    [SerializeField] private MovementBehavior movementBehavior = MovementBehavior.Normal;

    [Space(15)]
    [SerializeField] private Transform orbitTarget = null;
    [SerializeField] private Transform previousOrbitTarget = null;
    [SerializeField] private float orbitSpeed = 4f;
    public float OrbitSpeed { get { return orbitSpeed; } }
    [SerializeField] private float orbitSpeedGrowth = 1f;
    [SerializeField] private float orbitLaunchSpeedMult = .75f;
    [SerializeField] private float previousMagnitude;

    [Header("Parry Mechanics")]
    [SerializeField] private float ParryForce = 15f;
    [SerializeField] private GameObject Hitbox;
    [SerializeField] private GameObject ExplosionVFX;
    [SerializeField] private LayerMask OrbitLayer;
    private HitboxBehavior hitboxBehaviorScript;

    [Header("Reign Stuff")]
    [SerializeField] private LineRenderer TopReign;
    [SerializeField] private Transform TopRight;
    [SerializeField] private Transform TopLeft;
    [Space(10)]
    [SerializeField] private LineRenderer BottomReign;
    [SerializeField] private Transform BottomRight;
    [SerializeField] private Transform BottomLeft;


    
    [Space(20)]
    [SerializeField] private Collider2D environmentalDetector;
    private List<Collider2D> collisions = new();


    void Start()
    {
        if(rb2D == null)
        {
            rb2D = GetComponent<Rigidbody2D>();
            Debug.Assert(rb2D, $"{nameof(AGJ_CharacterController)} needs a RigidBody2D component.");
        }

        Debug.Assert(sprite, $"{nameof(AGJ_CharacterController)} needs a reference to its sprite, which should be a child.");

        environmentalInteractionHandlers.Add(new EIH_OrbitHandler());
        GameObject hitbox = Instantiate(Hitbox, transform.position, Quaternion.identity, transform);
        hitboxBehaviorScript = hitbox.GetComponent<HitboxBehavior>();

        hitboxBehaviorScript.AddWineCollisionListener(this);
    }

    private void Update()
    {
        steerForce.x = 0;
        steerForce.y = 0;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && movementBehavior == MovementBehavior.Orbiting)
        {
            Vector2 orbitTargetToMe = (Vector2)transform.position - (Vector2)orbitTarget.position;
            orbitTargetToMe.Normalize();

            //Get a perpendicular vector
            direction = new Vector2(-orbitTargetToMe.y, orbitTargetToMe.x);

            ActivateNormalMovement();
        }

        if(Keyboard.current.wKey.isPressed && movementBehavior == MovementBehavior.Normal)
        {
            steerForce = transform.up.normalized * steerStrength;
        }
        if(Keyboard.current.sKey.isPressed && movementBehavior == MovementBehavior.Normal)
        {
            steerForce = transform.up.normalized * -steerStrength;
        }
        
        bool didRotate = false;
        if(Keyboard.current.aKey.isPressed && movementBehavior == MovementBehavior.Normal)
        {
            vehicleRotationSpeed += vehicleRotationAcceleration * Time.deltaTime;
            didRotate = true;
        }
        if(Keyboard.current.dKey.isPressed && movementBehavior == MovementBehavior.Normal)
        {
            vehicleRotationSpeed -= vehicleRotationAcceleration * Time.deltaTime;
            didRotate = true;
        }

        if (Keyboard.current.rKey.isPressed)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (!didRotate)
        {
            if(vehicleRotationSpeed > 0)
            {
                vehicleRotationSpeed -= vehicleRotationDecay * Time.deltaTime;
                vehicleRotationSpeed = Mathf.Max(vehicleRotationSpeed, 0);
            }
            else
            {
                vehicleRotationSpeed += vehicleRotationDecay * Time.deltaTime;
                vehicleRotationSpeed = Mathf.Min(vehicleRotationSpeed, 0);
            }
        }
        
        sprite.Rotate(0, 0, vehicleRotationSpeed * Time.deltaTime);
        hitboxBehaviorScript.UpdatePosition(rb2D.position);
        UpdateReignPosition();
    }

    private void FixedUpdate()
    {
        switch (movementBehavior)
        {
            case(MovementBehavior.Normal): NormalMovementBehavior(); break;
            case(MovementBehavior.Orbiting): OrbitingMovementBehavior(); break;
            default: NormalMovementBehavior(); break;
        }

        //Overlapping a specific collider attached to the player to figure out what's in front of us.
        //I want to make this movement system easy to extend, so I'll make use of
        //a Chain of Responsibility that determines how to react to different pieces
        //of the environment.
        collisions.Clear();
        environmentalDetector.Overlap(ContactFilter2D.noFilter, collisions);

        if(collisions.Count != 0)
        {
            //Which link in the chain can handle this?
            GameObject toBeHandled;
            foreach (IEnvironmentalInteractionHandler handler in environmentalInteractionHandlers)
            {
                if (handler.CanIHandleThis(collisions, out toBeHandled))
                {
                    handler.HandleThis(toBeHandled, this);
                    break;
                }
            }
        }
    }

    private void NormalMovementBehavior()
    {
        rb2D.AddForce(direction * Time.deltaTime * acceleration + steerForce);

        //Clamp the player's velocity 
        float magnitude = Mathf.Min(rb2D.linearVelocity.magnitude, speedCap);
        rb2D.linearVelocity = rb2D.linearVelocity.normalized * magnitude;
    }
    private void OrbitingMovementBehavior()
    {
        Quaternion q = Quaternion.AngleAxis(orbitSpeed, transform.forward);
        
        Vector3 newPosition = q * (rb2D.transform.position - orbitTarget.position) + orbitTarget.position;

        transform.right = newPosition - transform.position;
        rb2D.MovePosition(q * (rb2D.transform.position - orbitTarget.position) + orbitTarget.position);
        //rb2D.MoveRotation(rb2D.transform.rotation * q);

    }

    public void ActivateOrbitingMovement(Transform target) 
    {
        if (target == previousOrbitTarget) return;

        AugmentSpeed();

        movementBehavior = MovementBehavior.Orbiting;
        orbitTarget = target;
        previousOrbitTarget = orbitTarget;
        rb2D.linearVelocity = Vector2.zero;

        Quaternion offsetRotation = rb2D.transform.rotation;
        rb2D.transform.SetPositionAndRotation(rb2D.transform.position, offsetRotation);

        AGJ_Camera.Instance.StopFollowing();
    }
    public void ActivateNormalMovement()
    {
        movementBehavior = MovementBehavior.Normal;
        rb2D.linearVelocity = direction.normalized * speedCap * orbitLaunchSpeedMult;

        AGJ_Camera.Instance.StartFollowing();
    }

    


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemyScript = collision.gameObject.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                StartCoroutine(enemyScript.DeathRoutine());
            }

            Vector2 directionOfParryForce = rb2D.position - collision.contacts[0].point;
            ApplyCollisionForce(directionOfParryForce.normalized);
        }
    }

    private void ApplyCollisionForce(Vector2 direction)
    {
        rb2D.AddForce(direction * ParryForce, ForceMode2D.Impulse);
    }


    public void PlayerDeathRoutine()
    {
        AGJ_Camera.Instance.StopFollowing();
        Instantiate(ExplosionVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void UpdateReignPosition()
    {
        TopReign.SetPosition(0, TopLeft.position);
        TopReign.SetPosition(1, TopRight.position);
        BottomReign.SetPosition(0, BottomLeft.position);
        BottomReign.SetPosition(1, BottomRight.position);
    }

    public void RunOnWineCollected()
    {
        AugmentSpeed();
    }

    private void AugmentSpeed()
    {
        speedCap += speedCapGrowth;
        acceleration += accelerationGrowth;
        orbitSpeed += orbitSpeedGrowth;
    }
}
