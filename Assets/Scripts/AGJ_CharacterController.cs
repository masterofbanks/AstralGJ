using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public enum MovementBehavior 
{
    Normal,
    Orbiting
}

public class AGJ_CharacterController : MonoBehaviour
{
    [SerializeField] private float speedCap;
    [SerializeField] private float raycastDistance = 2f;
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private Vector2 direction;
    [SerializeField] private float acceleration;
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
    [SerializeField] private float orbitSpeed = 10f;
    [SerializeField] private float previousMagnitude;


    void Start()
    {
        if(rb2D == null)
        {
            rb2D = GetComponent<Rigidbody2D>();
            Debug.Assert(rb2D, $"{nameof(AGJ_CharacterController)} needs a RigidBody2D component.");
        }

        Debug.Assert(sprite, $"{nameof(AGJ_CharacterController)} needs a reference to its sprite, which should be a child.");

        environmentalInteractionHandlers.Add(new EIH_OrbitHandler());
    }

    private void Update()
    {
        steerForce.x = 0;
        steerForce.y = 0;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && movementBehavior == MovementBehavior.Orbiting)
        {
            Vector2 orbitTargetToMe = (Vector2)transform.position - (Vector2)orbitTarget.position;
            orbitTargetToMe.Normalize();
            Debug.Log((Vector2)transform.position);
            Debug.Log((Vector2)orbitTarget.position);
            Debug.Log(orbitTargetToMe);
            Debug.DrawRay((Vector2)orbitTarget.position, orbitTargetToMe.normalized * 10, Color.red, 10f);

            //Get a perpendicular vector
            direction = new Vector2(-orbitTargetToMe.y, orbitTargetToMe.x);
            
            Debug.Log(orbitTargetToMe);
            Debug.DrawRay((Vector2)transform.position, direction.normalized * 10, Color.blue, 10f);
            //Debug.Break();

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
    }

    private void FixedUpdate()
    {
        switch (movementBehavior)
        {
            case(MovementBehavior.Normal): NormalMovementBehavior(); break;
            case(MovementBehavior.Orbiting): OrbitingMovementBehavior(); break;
            default: NormalMovementBehavior(); break;
        }

        //Raycasting to figure out what's in front of us.
        //I want to make this movement system easy to extend, so I'll make use of
        //a Chain of Responsibility that determines how to react to different pieces
        //of the environment.
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, direction, raycastDistance);
        if(hit.collider != null)
        {
            //Which link in the chain can handle this?
            foreach(IEnvironmentalInteractionHandler handler in environmentalInteractionHandlers)
            {
                if (handler.CanIHandleThis(hit.collider.gameObject))
                {
                    handler.HandleThis(hit.collider.gameObject, hit, this);
                    break;
                }
            }
        }
    }

    private void NormalMovementBehavior()
    {
        Debug.Log(direction);
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
        rb2D.linearVelocity = direction * orbitSpeed;

        AGJ_Camera.Instance.StartFollowing();
    }
}
