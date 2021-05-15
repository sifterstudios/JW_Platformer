using UnityEngine;
using UnityEngine.Events;

public class ToggleSwitch : MonoBehaviour
{
    [SerializeField] ToggleDirection _startingDirection = ToggleDirection.Center;

    [SerializeField] UnityEvent _onLeft;
    [SerializeField] UnityEvent _onRight;
    [SerializeField] UnityEvent _onCenter;
    [SerializeField] Sprite _leftSprite;
    [SerializeField] Sprite _rightSprite;
    [SerializeField] Sprite _centerSprite;

    [SerializeField] SpriteRenderer _spriteRenderer;
    ToggleDirection _currentDirection;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        SetToggleDirection(_startingDirection, true);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;
        var playerRigidbody = player.GetComponent<Rigidbody2D>();
        if (playerRigidbody == null)
            return;
        var wasOnRight = other.transform.position.x > transform.position.x;
        var playerWalkingRight = playerRigidbody.velocity.x > 0;
        var playerWalkingLeft = playerRigidbody.velocity.x < 0;

        if (wasOnRight && playerWalkingRight)
            SetToggleDirection(ToggleDirection.Right);
        else if (!wasOnRight && playerWalkingLeft)
            SetToggleDirection(ToggleDirection.Left);
    }

    void OnValidate()
    {
        switch (_startingDirection)
        {
            case ToggleDirection.Left:
                _spriteRenderer.sprite = _leftSprite;
                break;
            case ToggleDirection.Center:
                _spriteRenderer.sprite = _centerSprite;
                break;
            case ToggleDirection.Right:
                _spriteRenderer.sprite = _rightSprite;
                break;
        }
    }

    void SetToggleDirection(ToggleDirection direction, bool force = false)
    {
        if (!force && _currentDirection == direction)
            return;
        _currentDirection = direction;
        switch (direction)
        {
            case ToggleDirection.Left:
                _spriteRenderer.sprite = _leftSprite;
                _onLeft.Invoke();
                break;
            case ToggleDirection.Center:
                _spriteRenderer.sprite = _centerSprite;
                _onCenter.Invoke();
                break;
            case ToggleDirection.Right:
                _spriteRenderer.sprite = _rightSprite;
                _onRight.Invoke();
                break;
        }
    }

    enum ToggleDirection
    {
        Left,
        Center,
        Right
    }
}