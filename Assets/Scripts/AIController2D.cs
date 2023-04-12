using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AIController2D : MonoBehaviour, IDamagable
{
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float speed;
    [SerializeField] float jumpHeight;
    [SerializeField] float doubleJumpHeight;
    [SerializeField, Range(1, 5)] float fallRateMultiplier;
    [SerializeField, Range(1, 5)] float lowJumpRateMultiplier;

    [Header("Ground")]
    [SerializeField] Transform groundTransform;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] float groundRadius;

    [Header("AI")]
    [SerializeField] Transform[] waypoints;
    [SerializeField] float rayDistance = 15f;
    [SerializeField] string enemyTag;
    [SerializeField] LayerMask raycastLayerMask;

    public float health = 100;

    // New Variables Created for New Code
    [SerializeField] Transform playerTransform;
    [SerializeField, Range(1, 3)] int attackDistance;
    public float attackDuration = 2.0f;


    Rigidbody2D rb;

    Vector2 velocity = Vector2.zero;
    bool faceRight = true;
    float groundAngle = 0;
    Transform targetWaypoint = null;
    GameObject enemy = null;

    enum State
    {
        IDLE,
        PATROL,
        CHASE,
        ATTACK
    }

    State state = State.IDLE;
    float stateTimer = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // update ai
        CheckEnemySeen();

        Vector2 direction = Vector2.zero;
        switch (state)
        {
            case State.IDLE:
                if (enemy != null) state = State.CHASE;
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    SetNewWaypointTarget();
                    state = State.PATROL;
                }
                break;
            case State.PATROL:
                {
                    if (enemy != null) state = State.CHASE;
                    direction.x = Mathf.Sign(targetWaypoint.position.x - transform.position.x);
                    float dx = Mathf.Abs(targetWaypoint.position.x - transform.position.x);
                    if (dx <= 0.25f)
                    {
                        state = State.IDLE;
                        stateTimer = 1;
                    }
                }
                break;
            case State.CHASE:
                {
                    // If Enemy isn't seen, the state switches back to Idle and resets the timer
                    if (enemy == null)
                    {
                        state = State.IDLE;
                        stateTimer = 1;
                        break;
                    } 
                    float dx = Mathf.Abs(enemy.transform.position.x - transform.position.x);
                    // if the dx variable is less than 1, the state will switch to Attack and the attack animation will play
                    if (dx <= 1f)
                    {
                        state = State.ATTACK;
                        animator.SetTrigger("Attack");
                    }
                    else 
                    {
                        // if the enemy isn't in range, the AI will go after the enemy if it's in view
                        direction.x = Mathf.Sign(enemy.transform.position.x - transform.position.x);
                    }
                }
                break;
            case State.ATTACK:
                // the state will switch back to chase once the attack animation is finished playing
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
                {
                    state = State.CHASE;
                }
                break;
            default:
                break;
        }


        bool onGround = UpdateGroundCheck();

        // get direction input

        // transform direction to slope space
        direction = Quaternion.AngleAxis(groundAngle, Vector3.forward) * direction;
        Debug.DrawRay(transform.position, direction, Color.green);

        velocity.x = direction.x * speed;

        // set velocity
        if (onGround)
        {
            if (velocity.y < 0) velocity.y = 0;
            //if (Input.GetButtonDown("Jump"))
            //{
            //    velocity.y += Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
            //    StartCoroutine(DoubleJump());
            //    animator.SetTrigger("Jump");
            //}
        }

        // adjust gravity for jump
        float gravityMultiplier = 1;
        if (!onGround && velocity.y < 0) gravityMultiplier = fallRateMultiplier;
        if (!onGround && velocity.y > 0 && !Input.GetButton("Jump")) gravityMultiplier = lowJumpRateMultiplier;

        velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;

        // move character
        rb.velocity = velocity;

        // flip character to face direction of movement (velocity)
        if (velocity.x > 0 && !faceRight) Flip();
        if (velocity.x < 0 && faceRight || velocity.x > 0 && !faceRight) Flip();

        // update animator
        animator.SetFloat("Speed", Mathf.Abs(velocity.x));
        animator.SetBool("Fall", !onGround && velocity.y < -0.1f);
    }

    IEnumerator DoubleJump()
    {
        // wait a little after the jump to allow a double jump
        yield return new WaitForSeconds(0.01f);
        // allow a double jump while moving up
        while (velocity.y > 0)
        {
            // if "jump" pressed add jump velocity
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y += Mathf.Sqrt(doubleJumpHeight * -2 * Physics.gravity.y);
                break;
            }
            yield return null;
        }
    }

    private bool UpdateGroundCheck()
    {
        // check if the character is on the ground
        Collider2D collider = Physics2D.OverlapCircle(groundTransform.position, groundRadius, groundLayerMask);
        if (collider != null)
        {
            RaycastHit2D raycastHit = Physics2D.Raycast(groundTransform.position, Vector2.down, groundRadius, groundLayerMask);
            if (raycastHit.collider != null)
            {
                // get the angle of the ground (angle between up vector and ground normal)
                groundAngle = Vector2.SignedAngle(Vector2.up, raycastHit.normal);
                Debug.DrawRay(raycastHit.point, raycastHit.normal, Color.red);
            }
        }

        return (collider != null);
    }

    private void Flip()
    {
        faceRight = !faceRight;
        spriteRenderer.flipX = !faceRight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundTransform.position, groundRadius);
    }

    private void SetNewWaypointTarget()
    {
        Transform waypoint = null;
        do
        {
            waypoint = waypoints[Random.Range(0, waypoints.Length)];
        } while (waypoint == targetWaypoint);

        targetWaypoint = waypoint;
    }

    // Updated Enemy Seen Code
    private void CheckEnemySeen()
    {
        enemy = null;
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, ((faceRight) ? Vector2.right : Vector2.left), rayDistance, raycastLayerMask);
        if (raycastHit.collider != null && raycastHit.collider.gameObject.CompareTag(enemyTag))
        {
            enemy = raycastHit.collider.gameObject;
            Debug.DrawRay(transform.position, ((faceRight) ? Vector2.right : Vector2.left) * rayDistance, Color.red);
        }
    }

    public void Damage(int damage)
    {
        health -= damage;
        print(health);
    }
}
