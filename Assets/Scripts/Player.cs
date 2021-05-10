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
    Rigidbody2D _rb;
    Animator _animator;
    SpriteRenderer _spriteRenderer;
    float _horizontal;
    bool _isGrounded;

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

        MoveHorizontal();

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

    void MoveHorizontal()
    {
        if (Mathf.Abs(_horizontal) >= 1)
        {
            _rb.velocity = new Vector2(_horizontal, _rb.velocity.y);
        }
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
        var hit = Physics2D.OverlapCircle(feet.position, 0.1f, LayerMask.GetMask("Default"));
        _isGrounded = hit != null;
    }

    internal void ResetToStart()
    {
        transform.position = _startPosition;
    }
}