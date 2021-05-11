using UnityEngine;
using UnityEngine.Events;

public class PushButtonSwitch : MonoBehaviour
{
    [SerializeField] Sprite pressedSprite;
    [SerializeField] UnityEvent onPressed;
    [SerializeField] UnityEvent onReleased;

    Sprite _releasedSprite;
    SpriteRenderer _spriteRenderer;
    [SerializeField] int playerNumber;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _releasedSprite = _spriteRenderer.sprite;
        BecomeReleased();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null || player.PlayerNumber != playerNumber)
            return;
        BecomePressed();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null || player.PlayerNumber != playerNumber)
            return;
        BecomeReleased();
    }

    void BecomePressed()
    {
        _spriteRenderer.sprite = pressedSprite;
        onPressed?.Invoke();
    }

    void BecomeReleased()
    {
        _spriteRenderer.sprite = _releasedSprite;
        onReleased?.Invoke();
    }
}