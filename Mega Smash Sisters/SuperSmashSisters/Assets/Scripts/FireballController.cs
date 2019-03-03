using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    public float direction = 1;
    public float speed = 1.3f;
    public string col = "";

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(transform.position.x + direction * speed, transform.position.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name != col)
            Invoke("Die", 0f);
       // else
       //     gameObject.GetComponent<CircleCollider2D>().isTrigger = true;
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == col)
        {
            gameObject.GetComponent<CircleCollider2D>().isTrigger = false;
        }
        print(col);
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
