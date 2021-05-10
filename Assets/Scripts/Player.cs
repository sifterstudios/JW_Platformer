using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")] [SerializeField] float movementSpeed = 5f;
    [SerializeField] float slipFactor = 1f;
    [SerializeField] float airMovementSpeed = 1.2f;
    [Header("Jump")] [SerializeField] float jumpVelocity = 500f;
    [SerializeField] int maxJumps = 2;
    [SerializeField] Transform feet;
    [SerializeField] float downpull = 5f;
    [SerializeField] float maxJumpDuration = 0.1f;


    Vector3 _startPosition;
    int _jumpsRemaining;
    float _falltimer;
    float _jumpTimer;
    Rigidbody2D _rb;
    Animator _animator;
    SpriteRenderer _spriteRenderer;
    float _horizontal;
    bool _isGrounded;
    Collider2D _colliderHit;

    void Start()
    {
        _startPosition = transform.position;
        _jumpsRemaining = maxJumps;
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        UpdateIsGrounded();

        ReadHorizontalInput();

        CheckGroundType();

        UpdateAnimator();

        UpdateSpriteDirection();

        if (ShouldStartJump())
            Jump();
        else if (ShouldContinueJump())
            ContinueJump();

        _jumpTimer += Time.deltaTime;

        if (_isGrounded && _falltimer > 0)
        {
            _falltimer = 0;
            _jumpsRemaining = maxJumps;
        }
        else
        {
            _falltimer += Time.deltaTime;
            var downForce = downpull * _falltimer * _falltimer;
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y - downForce);
        }
    }

    void ContinueJump()
    {
        _rb.velocity = (new Vector2(_rb.velocity.x, jumpVelocity));
        _falltimer = 0;
    }

    bool ShouldContinueJump()
    {
        return Input.GetButton("Fire1") && _jumpTimer <= maxJumpDuration;
    }

    void Jump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, jumpVelocity);
        _jumpsRemaining--;
        _falltimer = 0;
        _jumpTimer = 0;
    }

    bool ShouldStartJump()
    {
        return Input.GetButtonDown("Fire1") && _jumpsRemaining > 0;
    }

    void CheckGroundType()
    {
        if (_colliderHit != null)
        {
            if (_colliderHit.CompareTag("Slippery"))
            {
                SlipHorizontal();
            }
            else
            {
                MoveHorizontal();
            }
        }
        else
        {
            MoveInAir();
        }
    }

    void MoveInAir()
    {
        _rb.velocity = new Vector2(_horizontal * airMovementSpeed, _rb.velocity.y);
    }

    void MoveHorizontal()
    {
        _rb.velocity = new Vector2(_horizontal * movementSpeed, _rb.velocity.y);
    }

    void SlipHorizontal()
    {
        var desiredVelocity = new Vector2(_horizontal * movementSpeed, _rb.velocity.y);
        var smoothedVelocity = Vector2.Lerp(
            _rb.velocity,
            desiredVelocity,
            Time.deltaTime / slipFactor);

        _rb.velocity = smoothedVelocity;
    }

    void ReadHorizontalInput()
    {
        _horizontal = Input.GetAxis("Horizontal") * movementSpeed;
    }

    void UpdateSpriteDirection()
    {
        if (_horizontal != 0)
        {
            _spriteRenderer.flipX = _horizontal < 0;
        }
    }

    void UpdateAnimator()
    {
        bool walking = _horizontal != 0;
        _animator.SetBool("Walk", walking);
    }

    void UpdateIsGrounded()
    {
        _colliderHit = Physics2D.OverlapCircle(feet.position, 0.1f, LayerMask.GetMask("Default"));
        _isGrounded = _colliderHit != null;
    }

    internal void ResetToStart()
    {
        transform.position = _startPosition;
    }
}