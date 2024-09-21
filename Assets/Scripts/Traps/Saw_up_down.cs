using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw_up_down : MonoBehaviour
{
    [SerializeField] private float movementDistance;
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    private bool movingUp;
    private float upEdge;
    private float downEdge;

    private void Awake()
    {
        // Change to modify the movement on the y-axis
        upEdge = transform.position.y + movementDistance; // Higher position (upward)
        downEdge = transform.position.y - movementDistance; // Lower position (downward)
    }

    private void Update()
    {
        if (movingUp)
        {
            // Move upward (y-axis) and check upper boundary
            if (transform.position.y < upEdge)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + speed * Time.deltaTime, transform.position.z);
            }
            else
            {
                movingUp = false;
            }
        }
        else
        {
            // Move downward (y-axis) and check lower boundary
            if (transform.position.y > downEdge)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - speed * Time.deltaTime, transform.position.z);
            }
            else
            {
                movingUp = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Health>().TakeDamage(damage);
        }
    }
}
