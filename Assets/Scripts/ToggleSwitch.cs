using UnityEngine;
using UnityEngine.Events;

public class ToggleSwitch : MonoBehaviour
{
    [SerializeField] UnityEvent _onLeft;
    [SerializeField] UnityEvent _onRight;
    [SerializeField] Sprite _leftSprite;
    [SerializeField] Sprite _rightSprite;

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
            SetPosition(true);
        else if (!wasOnRight && !playerWalkingLeft)
            SetPosition(false);
    }

    void SetPosition(bool right)
    {
        if (right)
        {
            _spriteRenderer.sprite = _rightSprite;
            _onRight.Invoke();
        }
        else
        {
            _spriteRenderer.sprite = _leftSprite;
            _onLeft.Invoke();
        }
    }
}