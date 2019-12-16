using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic platformer movement and
/// jumping script
/// </summary>
public class MoveJumpScript : MonoBehaviour
{
    public float speed;

    public float jumpForce;
    private Rigidbody2D rb2d;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        transform.position += new Vector3(hor, ver, 0f) * speed * Time.deltaTime;

        if (Input.GetButtonDown("Jump"))
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector3(0f, jumpForce, 0f), ForceMode2D.Impulse);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Cube"))
        {
            // rb2d.gravityScale = -1;

            rb2d.AddForce(new Vector2(-20f,1f), ForceMode2D.Force);
        }
    }
}
