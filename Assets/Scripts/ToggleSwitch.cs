using UnityEngine;

public class ToggleSwitch : MonoBehaviour
{
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
            _spriteRenderer.sprite = _rightSprite;
        else if (!wasOnRight && !playerWalkingLeft)
            _spriteRenderer.sprite = _leftSprite;
    }
}