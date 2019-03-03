using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI : MonoBehaviour
{
    public GameObject enemy;
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

    public Text livesDisplay;

    public int lives = 3;

    public GameObject basicAttackRadius;

    int recharge = 0;
    int recharge2 = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        remainingJumps = maxJumps;
        animator = GetComponent<Animator>();
        healthText.text = health.ToString();
        knockbackForce = new Vector2(0, 0);

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
        }
        else
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
        livesDisplay.text = " ";
        for (int i = 0; i < lives; i++)
        {
            livesDisplay.text += "* ";
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (recharge > 0)
        {
            recharge--;
        }
        if (recharge2 > 0)
        {
            recharge2--;
        }
        //basic movement
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        rb.velocity = new Vector2(horizontalMovement * speed, rb.velocity.y);

        //flip character
        if (!facingRight && horizontalMovement < 0 && !xScaleFreeze)
        {
            flip();
        }
        else if (facingRight && horizontalMovement > 0 && !xScaleFreeze)
        {
            flip();
        }

        rb.AddForce(knockbackForce);

        if (knockbackForce.x >= 1)
        {
            if (knockbackForce.y >= 1)
                knockbackForce = new Vector2(knockbackForce.x - 1, knockbackForce.y - 1);
            else if (knockbackForce.y <= -1)
                knockbackForce = new Vector2(knockbackForce.x - 1, knockbackForce.y + 1);
            else
                knockbackForce = new Vector2(knockbackForce.x - 1, 0);
        }
        else if (knockbackForce.x <= 1)
        {
            if (knockbackForce.y >= 1)
                knockbackForce = new Vector2(knockbackForce.x + 1, knockbackForce.y - 1);
            else if (knockbackForce.y <= -1)
                knockbackForce = new Vector2(knockbackForce.x + 1, knockbackForce.y + 1);
            else
                knockbackForce = new Vector2(knockbackForce.x + 1, 0);
        }
        else
        {
            if (knockbackForce.y >= 1)
                knockbackForce = new Vector2(0, knockbackForce.y - 1);
            else if (knockbackForce.y <= -1)
                knockbackForce = new Vector2(0, knockbackForce.y + 1);
            else
                knockbackForce = new Vector2(0, 0);
        }
    }

    void Update()
    {
        //check if fallen off
        if (transform.position.y <= -20)
        {
            Kill();
            //transform.position = new Vector2(0, 15);
        }

        //determine move

        if (enemy.transform.position.y - transform.position.y < 1 && enemy.transform.position.y - transform.position.y > -1 && Time.timeScale > 0)
        {
            
            if (Mathf.Abs(enemy.transform.position.x - transform.position.x) > 5)
            {
                print((enemy.transform.position - transform.position).magnitude);
                if (enemy.transform.position.x > transform.position.x)
                {
                    horizontalMovement = 1;
                    if (facingRight && !xScaleFreeze)
                    {
                        flip();
                    }
                }
                else if (enemy.transform.position.x < transform.position.x)
                {
                    horizontalMovement = -1;
                    if (!facingRight && !xScaleFreeze)
                    {
                        flip();
                    }
                }
            }
            else
            {
                if (horizontalMovement > 0)
                {
                    horizontalMovement = 0;
                }
                else if (horizontalMovement < 0)
                {
                    horizontalMovement = 0;
                }

            }
            if (Mathf.Abs(enemy.transform.position.x - transform.position.x) < 6 && recharge2 == 0)
            {
                basicAttackRadius.SetActive(true);
                recharge2 = 10;
                animator.SetTrigger("basicAttack");
                Invoke("StopBasicAttack", 2f);
            }
            if (recharge == 0)
            {
                animator.SetTrigger("powerAttack");
                Invoke("shootBall", 0.4f);
                freezeXScale();
                Invoke("freezeXScale", 0.5f);
                recharge = 40;
            }

        }
        else if (enemy.transform.position.y > transform.position.y && remainingJumps > 0 && Time.timeScale > 0)
        {
            if (isGrounded || isOnPlatform)
            {
                animator.SetTrigger("jump");
                Invoke("jump", 0.1f);
            }
            else
            {
                animator.SetTrigger("double jump");
                Invoke("jump", 0.1f);
            }
        }
        else if (enemy.transform.position.y < transform.position.y && Time.timeScale > 0)
        {
            platformDrop();
            Invoke("platformDrop", 0.4f);
        }


        //move

        //if (Input.GetKeyDown(controls["left"]) && Time.timeScale > 0)
        //{
        //    horizontalMovement += -1;
        //}
        //if (Input.GetKeyDown(controls["right"]) && Time.timeScale > 0)
        //{
        //    horizontalMovement += 1;
        //}
        //if (Input.GetKeyUp(controls["left"]) && Time.timeScale > 0)
        //{
        //    horizontalMovement -= -1;
        //}
        //if (Input.GetKeyUp(controls["right"]) && Time.timeScale > 0)
        //{
        //    horizontalMovement -= 1;
        //}

        if (horizontalMovement != 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
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
            }
            else
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

    void StopBasicAttack()
    {
        basicAttackRadius.SetActive(false);
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
            knockbackForce = transform.right * collision.gameObject.GetComponent<FireballController>().direction * health * 25;
            takeDamage(10);
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
        if (health >= 250 && Random.Range(0, 100) < 25 + health / 250)
        {
            knockbackForce = (transform.right * 8 + transform.up) * 500;
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
            Invoke("KoolKill", 0.5f);
        }
    }

    void KoolKill()
    {
        knockbackForce = (transform.right * 10 * knockbackForce.magnitude / Mathf.Abs(knockbackForce.magnitude) + transform.up) * 500;
        Invoke("resetKnockbackForce", 0.5f);
        Kill();
    }

    void Respawn()
    {
        livesDisplay.text = " ";
        for (int i = 0; i < lives; i++)
        {
            livesDisplay.text += "* ";
        }
        health = 0;
        healthText.text = health.ToString();
        transform.localPosition = new Vector2(0, 7);
    }

    void resetKnockbackForce()
    {
        knockbackForce = new Vector2(0, 0);
        gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
    }

    void Kill()
    {
        resetKnockbackForce();
        lives--;
        Respawn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "basicattackradius" && collision.gameObject.name != gameObject.name)
        {
            takeDamage(15);
            knockbackForce = (transform.position - collision.gameObject.transform.position) * health;
            Invoke("resetKnockbackForce", 0.5f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "SceneController")
        {
            Kill();
        }
    }
}