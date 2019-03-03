using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject fireballprefab;
    public GameObject shooterLoc;

    private bool isOnPlatform = false;

    public int maxJumps;
    private int remainingJumps;

    private float nextTimeToCheckIfGrounded = 0f;

    private GameObject currentPlatform;

    private Animator animator;

    public Text healthText;
    private int health = 0;

    private Vector2 knockbackForce;

    public AudioSource soundEffects;
    public AudioClip fire;
    public AudioClip hit;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        remainingJumps = maxJumps;
        animator = GetComponent<Animator>();
        healthText.text = health.ToString();
        knockbackForce = new Vector2(0,0);

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

        rb.AddForce(knockbackForce);
    }

    void Update()
    {
        //check if fallen off
        if (transform.position.y <= -20)
        {
            transform.position = new Vector2(0, 15); ;
        }

        //move
        if (Input.GetKeyDown(controls["left"]) && Time.timeScale > 0)
        {
            horizontalMovement += -1;
        }
        if (Input.GetKeyDown(controls["right"]) && Time.timeScale > 0)
        {
            horizontalMovement += 1;
        }
        if (Input.GetKeyUp(controls["left"]) && Time.timeScale > 0)
        {
            horizontalMovement -= -1;
        }
        if (Input.GetKeyUp(controls["right"]) && Time.timeScale > 0)
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
        if (Input.GetKeyDown(controls["jump"]) && remainingJumps > 0 && Time.timeScale > 0)
        {
            if (isGrounded || isOnPlatform)
            {
                animator.SetTrigger("jump");
                Invoke("jump", 0.1f);
            } else
            {
                animator.SetTrigger("double jump");
                Invoke("jump", 0.1f);
            }
        }

        if ((isGrounded || isOnPlatform) && remainingJumps < maxJumps && Time.time >= nextTimeToCheckIfGrounded &&
            rb.velocity.y == 0)
        {
            remainingJumps = maxJumps;
        }

        //drop
        if (Input.GetKeyDown(controls["drop"]) && isOnPlatform && Time.timeScale > 0)
        {
            platformDrop();
            Invoke("platformDrop", 0.4f);
        }

        //attack
        if (Input.GetKeyDown(controls["power attack"]) && Time.timeScale > 0)
        {
            animator.SetTrigger("powerAttack");
            Invoke("shootBall", 0.4f);
            freezeXScale();
            Invoke("freezeXScale", 0.5f);
        }
        else if (Input.GetKeyDown(controls["basic attack"]) && Time.timeScale > 0)
        {
            animator.SetTrigger("basicAttack");
        }

        //taunt
        if (Input.GetKeyDown(controls["taunt"]) && Time.timeScale > 0)
        {
            animator.SetTrigger("taunt");
        }
    }

    void jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        remainingJumps--;
        nextTimeToCheckIfGrounded = Time.time + .15f;
    }

    void shootBall()
    {
        GameObject fireball = Instantiate(fireballprefab);
        fireball.transform.position = new Vector2(shooterLoc.transform.transform.position.x + 5 * transform.localScale.x - 0.8f, shooterLoc.transform.position.y);
        fireball.GetComponent<FireballController>().direction = transform.localScale.x;
        fireball.GetComponent<FireballController>().col = gameObject.name;
        soundEffects.clip = fire;
        soundEffects.Play();
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

        if (collision.gameObject.tag == "fireball")
        {
            takeDamage(10);
            knockbackForce = transform.right * collision.gameObject.GetComponent<FireballController>().direction * health * 25;
            Invoke("resetKnockbackForce", 0.5f);
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

    public void takeDamage(int damage)
    {
        health += damage;
        healthText.text = health.ToString();
        soundEffects.clip = hit;
        soundEffects.Play();
    }

    void resetKnockbackForce()
    {
        knockbackForce = new Vector2(0,0);
    }
}
