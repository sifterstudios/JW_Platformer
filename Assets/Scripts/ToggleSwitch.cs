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

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;
        var wasOnRight = other.transform.position.x > transform.position.x;

        _spriteRenderer.sprite = wasOnRight ? _leftSprite : _rightSprite;
    }
}