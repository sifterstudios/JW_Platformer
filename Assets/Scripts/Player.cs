using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float jumpVelocity = 500f;
    [SerializeField] int maxJumps = 2;
    [SerializeField] Transform feet;
    [SerializeField] float downpull = 5f;
    [SerializeField] float maxJumpDuration = 0.1f;


    Vector3 _startPosition;
    int _jumpsRemaining;
    float _falltimer;
    float _jumpTimer;

    void Start()
    {
        _startPosition = transform.position;
        _jumpsRemaining = maxJumps;
    }

    void Update()
    {
        var hit = Physics2D.OverlapCircle(feet.position, 0.1f, LayerMask.GetMask("Default"));
        bool isGrounded = hit != null;

        var horizontal = Input.GetAxis("Horizontal") * movementSpeed;
        var rb = GetComponent<Rigidbody2D>();

        if (Mathf.Abs(horizontal) >= 1)
        {
            rb.velocity = new Vector2(horizontal, rb.velocity.y);
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
            rb.velocity = (new Vector2(rb.velocity.x, jumpVelocity));
            _jumpsRemaining--;
            _falltimer = 0;
            _jumpTimer = 0;
        }
        else if (Input.GetButton("Fire1") && _jumpTimer <= maxJumpDuration)
        {
            rb.velocity = (new Vector2(rb.velocity.x, jumpVelocity));
            _falltimer = 0;
        }

        _jumpTimer += Time.deltaTime;

        if (isGrounded && _falltimer > 0)
        {
            _falltimer = 0;
            _jumpsRemaining = maxJumps;
        }
        else
        {
            _falltimer += Time.deltaTime;
            var downForce = downpull * _falltimer * _falltimer;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - downForce);
        }
    }

    internal void ResetToStart()
    {
        transform.position = _startPosition;
    }
}