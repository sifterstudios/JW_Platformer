using UnityEngine;

public class CoinBox : HittableFromBelow
{
    [SerializeField] int _totalCoins = 3;

    int _remainingCoins;
    SpriteRenderer _spriteRenderer;

    protected override bool CanUse => _remainingCoins > 0;

    void Start()
    {
        _remainingCoins = _totalCoins;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Use()
    {
        base.Use();

        Coin.CoinsCollected++;
        _remainingCoins--;
    }
}