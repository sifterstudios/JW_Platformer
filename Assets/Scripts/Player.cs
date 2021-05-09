using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float jumpForce = 500f;

    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal") * movementSpeed;
        var rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(horizontal, rb.velocity.y);

        var animator = GetComponent<Animator>();
        bool walking = horizontal != 0;
        animator.SetBool("Walk", walking);

        if (horizontal != 0)
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.flipX = horizontal < 0;
        }

        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector2.up * jumpForce);
        }
    }
}