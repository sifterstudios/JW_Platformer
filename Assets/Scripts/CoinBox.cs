using UnityEngine;

public class CoinBox : MonoBehaviour
{
    [SerializeField] int _totalCoins = 3;
    [SerializeField] Sprite _usedSprite;

    int _remainingCoins;
    SpriteRenderer _spriteRenderer;

    void Start()
    {
        _remainingCoins = _totalCoins;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        var player = other.collider.GetComponent<Player>();
        if (player == null)
            return;

        if (other.contacts[0].normal.y > 0 && _remainingCoins > 0)
        {
            Coin.CoinsCollected++;
            _remainingCoins--;
        }

        if (_remainingCoins <= 0) _spriteRenderer.sprite = _usedSprite;
    }
}