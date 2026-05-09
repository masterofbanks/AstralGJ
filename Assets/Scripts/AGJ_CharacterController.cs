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

    //A chain of responsibility that governs how the character reacts to certain obstacles in the environment
    [SerializeField] private List<IEnvironmentalInteractionHandler> environmentalInteractionHandlers = new();

    [SerializeField] private MovementBehavior movementBehavior = MovementBehavior.Normal;
    [SerializeField] private Transform orbitTarget = null;
    [SerializeField] private float orbitSpeed = 10f;
    [SerializeField] private Vector2 previousPosition;
    [SerializeField] private float previousMagnitude;

    void Start()
    {
        if(rb2D == null)
        {
            rb2D = GetComponent<Rigidbody2D>();
            Debug.Assert(rb2D, $"{nameof(AGJ_CharacterController)} needs a RigidBody2D component.");
        }

        environmentalInteractionHandlers.Add(new EIH_OrbitHandler());
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
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
        rb2D.AddForce(direction * Time.deltaTime * acceleration);

        //Clamp the player's velocity 
        float magnitude = Mathf.Min(rb2D.linearVelocity.magnitude, speedCap);
        rb2D.linearVelocity = rb2D.linearVelocity.normalized * magnitude;
    }
    private void OrbitingMovementBehavior()
    {
        previousPosition = transform.position;
        Quaternion q = Quaternion.AngleAxis(orbitSpeed, transform.forward);
        rb2D.MovePosition(q * (rb2D.transform.position - orbitTarget.position) + orbitTarget.position);
        rb2D.MoveRotation(rb2D.transform.rotation * q);
    }

    public void ActivateOrbitingMovement(Transform target)
    {
        movementBehavior = MovementBehavior.Orbiting;
        orbitTarget = target;
        previousPosition = transform.position;
        //orbitSpeed = rb2D.linearVelocity.magnitude;
        rb2D.linearVelocity = Vector2.zero;
    }
    public void ActivateNormalMovement()
    {
        movementBehavior = MovementBehavior.Normal;
        rb2D.linearVelocity = direction * orbitSpeed;
    }
}
