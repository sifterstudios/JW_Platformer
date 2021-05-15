using UnityEngine;
using UnityEngine.Events;

public class ToggleSwitch : MonoBehaviour
{
    [SerializeField] UnityEvent _onLeft;
    [SerializeField] UnityEvent _onRight;
    [SerializeField] UnityEvent _onCenter;
    [SerializeField] Sprite _leftSprite;
    [SerializeField] Sprite _rightSprite;
    [SerializeField] Sprite _centerSprite;
    ToggleDirection _currentDirection;

    SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
            SetToggleDirectionPosition(ToggleDirection.Right);
        else if (!wasOnRight && playerWalkingLeft)
            SetToggleDirectionPosition(ToggleDirection.Left);
    }

    void SetToggleDirectionPosition(ToggleDirection direction)
    {
        if (_currentDirection == direction)
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