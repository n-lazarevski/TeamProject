using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField]
    public float speed = 10f; // Speed to move the ball to the left

    [SerializeField]
    public bool startMovingToLeft = true;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Set initial velocity to move the ball to the left
        if (startMovingToLeft )
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
        }
        else
        {
             rb.velocity = new Vector2(speed, rb.velocity.y);
        }
    }

    void Update()
    {
        // Maintain constant horizontal velocity
        if (rb.velocity.x < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);

        }
        else if (rb.velocity.x > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);

        }
    }
}
