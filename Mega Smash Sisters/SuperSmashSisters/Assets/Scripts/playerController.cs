using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public bool isPlayerOne;
    public float speed;
    public float jumpForce;
    public Dictionary<string, KeyCode> controls;

    private float horizontalMovement = 0;
    private Rigidbody2D rb;

    private bool facingRight = true;
    private bool xScaleFreeze = false;

    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    private bool isOnPlatform = false;

    public int maxJumps;
    private int remainingJumps;

    private float nextTimeToCheckIfGrounded = 0f;

    private GameObject currentPlatform;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        remainingJumps = maxJumps;
        animator = GetComponent<Animator>();

        if (isPlayerOne)
        {
            controls = new Dictionary<string, KeyCode>()
            {
                {"jump", KeyCode.W},
                {"right", KeyCode.D},
                {"left", KeyCode.A},
                {"drop", KeyCode.S},
                {"basic attack", KeyCode.LeftShift},
                {"power attack", KeyCode.Space},
                {"taunt", KeyCode.T}
            };
        } else
        {
            controls = new Dictionary<string, KeyCode>()
            {
                {"jump", KeyCode.UpArrow},
                {"right", KeyCode.RightArrow},
                {"left", KeyCode.LeftArrow},
                {"drop", KeyCode.DownArrow},
                {"basic attack", KeyCode.Mouse0},
                {"power attack", KeyCode.Mouse1},
                {"taunt", KeyCode.Period}
            };
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //basic movement
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        rb.velocity = new Vector2(horizontalMovement * speed, rb.velocity.y);

        //flip character
        if (!facingRight && horizontalMovement > 0 && !xScaleFreeze)
        {
            flip();
        }
        else if (facingRight && horizontalMovement < 0 && !xScaleFreeze)
        {
            flip();
        }
    }

    void Update()
    {
        //move
        if (Input.GetKeyDown(controls["left"]))
        {
            horizontalMovement += -1;
        }
        if (Input.GetKeyDown(controls["right"]))
        {
            horizontalMovement += 1;
        }
        if (Input.GetKeyUp(controls["left"]))
        {
            horizontalMovement -= -1;
        }
        if (Input.GetKeyUp(controls["right"]))
        {
            horizontalMovement -= 1;
        }

        if (horizontalMovement != 0)
        {
            animator.SetBool("isRunning", true);
        } else
        {
            animator.SetBool("isRunning", false);
        }

        //jump
        if (Input.GetKeyDown(controls["jump"]) && remainingJumps > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            remainingJumps--;
            nextTimeToCheckIfGrounded = Time.time + .15f;
        }

        if ((isGrounded || isOnPlatform) && remainingJumps < maxJumps && Time.time >= nextTimeToCheckIfGrounded &&
            rb.velocity.y == 0)
        {
            remainingJumps = maxJumps;
        }

        //drop
        if (Input.GetKeyDown(controls["drop"]) && isOnPlatform)
        {
            platformDrop();
            Invoke("platformDrop", 0.4f);
        }

        //attack
        if (Input.GetKeyDown(controls["power attack"]))
        {
            animator.SetTrigger("powerAttack");
            freezeXScale();
            Invoke("freezeXScale", 0.5f);
        }

        //taunt
        if (Input.GetKeyDown(controls["taunt"]))
        {
            animator.SetTrigger("taunt");
        }
    }

    void flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 10) //platform layer
        {
            isOnPlatform = true;
            currentPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 10) //platform layer
        {
            isOnPlatform = false;
        }
    }

    void platformDrop()
    {
        currentPlatform.GetComponent<BoxCollider2D>().enabled = !currentPlatform.GetComponent<BoxCollider2D>().enabled;
    }

    void freezeXScale()
    {
        xScaleFreeze = !xScaleFreeze;
    }
}
