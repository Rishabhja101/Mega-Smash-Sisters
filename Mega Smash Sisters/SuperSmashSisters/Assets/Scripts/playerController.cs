using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{

    public float speed;
    public float jumpForce;

    private float moveInput;
    private Rigidbody2D rb;

    private bool facingRight = true;

    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    private bool isOnPlatform;
    public LayerMask whatIsPlatform;

    public int extraJumps = 2;
    private int remainingExtraJumps;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        remainingExtraJumps = extraJumps;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //basic movement
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        isOnPlatform = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsPlatform);

        moveInput = Input.GetAxisRaw("Horizontal");

        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        //flip[ character
        if (!facingRight && moveInput > 0)
        {
            flip();
        } else if (facingRight && moveInput < 0)
        {
            flip();
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && remainingExtraJumps > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            remainingExtraJumps--;
        }

        if ((isGrounded || isOnPlatform) && remainingExtraJumps < extraJumps)
        {
            remainingExtraJumps = extraJumps;
        }

        if (Input.GetKeyDown(KeyCode.S) && isOnPlatform)
        {
            rb.position = new Vector2(rb.position.x, rb.position.y - 1.5f);
        }
    }

    void flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}
