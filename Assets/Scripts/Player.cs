using System;
using UnityEditorInternal;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float jumpForce = 500f;
    [SerializeField] int maxJumps = 2;
    Vector3 _startPosition;
    int _jumpsRemaining;
    [SerializeField] Transform feet;

    void Start()
    {
        _startPosition = transform.position;
        _jumpsRemaining = maxJumps;
    }

    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal") * movementSpeed;
        var rb = GetComponent<Rigidbody2D>();

        if (Mathf.Abs(horizontal) >= 1)
        {
            rb.velocity = new Vector2(horizontal, rb.velocity.y);
            //print($"Velocity = {rb.velocity}");
        }

        var animator = GetComponent<Animator>();
        bool walking = horizontal != 0;
        animator.SetBool("Walk", walking);

        if (horizontal != 0)
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.flipX = horizontal < 0;
        }

        if (Input.GetButtonDown("Fire1") && _jumpsRemaining > 0)
        {
            rb.AddForce(Vector2.up * jumpForce);
            _jumpsRemaining--;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        var hit = Physics2D.OverlapCircle(feet.position, 0.1f, LayerMask.GetMask("Default"));
        if (hit != null)
        {
            _jumpsRemaining = maxJumps;
        }
    }

    internal void ResetToStart()
    {
        transform.position = _startPosition;
    }
}