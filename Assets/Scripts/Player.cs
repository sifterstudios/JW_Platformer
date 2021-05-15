using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] int _playerNumber = 1;
    [Header("Movement")] [SerializeField] float _movementSpeed = 5f;
    [SerializeField] float _slipFactor = 1f;
    [SerializeField] float _airMovementSpeed = 1.2f;
    [Header("Jump")] [SerializeField] float _jumpVelocity = 500f;
    [SerializeField] int _maxJumps = 2;
    [SerializeField] Transform _feet;
    [SerializeField] float _downpull = 5f;
    [SerializeField] float _maxJumpDuration = 0.1f;
    Animator _animator;

    AudioSource _audioSource;
    Collider2D _colliderHit;
    float _falltimer;
    float _horizontal;
    string _horizontalAxis;
    bool _isGrounded;
    string _jumpButton;
    int _jumpsRemaining;
    float _jumpTimer;
    int _layerMask;
    Rigidbody2D _rb;
    SpriteRenderer _spriteRenderer;
    Vector3 _startPosition;

    public int PlayerNumber => _playerNumber;

    void Start()
    {
        _jumpButton = $"P{_playerNumber}Jump";
        _horizontalAxis = $"P{_playerNumber}Horizontal";
        _layerMask = LayerMask.GetMask("Default");

        _startPosition = transform.position;
        _jumpsRemaining = _maxJumps;
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
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
            _jumpsRemaining = _maxJumps;
        }
        else
        {
            _falltimer += Time.deltaTime;
            var downForce = _downpull * _falltimer * _falltimer;
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y - downForce);
        }
    }

    void ContinueJump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, _jumpVelocity);
        _falltimer = 0;
    }

    bool ShouldContinueJump()
    {
        return Input.GetButton(_jumpButton) && _jumpTimer <= _maxJumpDuration;
    }

    void Jump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, _jumpVelocity);
        _jumpsRemaining--;
        _falltimer = 0;
        _jumpTimer = 0;
        if (_audioSource != null)
            _audioSource.Play();
    }

    bool ShouldStartJump()
    {
        return Input.GetButtonDown(_jumpButton) && _jumpsRemaining > 0;
    }

    void CheckGroundType()
    {
        if (_colliderHit != null)
        {
            if (_colliderHit.CompareTag("Slippery"))
                SlipHorizontal();
            else
                MoveHorizontal();
        }
        else
        {
            MoveInAir();
        }
    }

    void MoveInAir()
    {
        _rb.velocity = new Vector2(_horizontal * _airMovementSpeed, _rb.velocity.y);
    }

    void MoveHorizontal()
    {
        _rb.velocity = new Vector2(_horizontal * _movementSpeed, _rb.velocity.y);
    }

    void SlipHorizontal()
    {
        var desiredVelocity = new Vector2(_horizontal * _movementSpeed, _rb.velocity.y);
        var smoothedVelocity = Vector2.Lerp(
            _rb.velocity,
            desiredVelocity,
            Time.deltaTime / _slipFactor);

        _rb.velocity = smoothedVelocity;
    }

    void ReadHorizontalInput()
    {
        _horizontal = Input.GetAxis(_horizontalAxis) * _movementSpeed;
    }

    void UpdateSpriteDirection()
    {
        if (_horizontal != 0) _spriteRenderer.flipX = _horizontal < 0;
    }

    void UpdateAnimator()
    {
        var walking = _horizontal != 0;
        _animator.SetBool("Walk", walking);
        _animator.SetBool("Jump", ShouldContinueJump());
    }

    void UpdateIsGrounded()
    {
        _colliderHit = Physics2D.OverlapCircle(_feet.position, 0.1f, _layerMask);
        _isGrounded = _colliderHit != null;
    }

    internal void ResetToStart()
    {
        _rb.position = _startPosition;
    }

    public void TeleportTo(Vector3 position)
    {
        _rb.position = position;
        _rb.velocity = Vector2.zero;
    }
}