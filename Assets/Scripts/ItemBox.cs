using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField] Sprite _usedSprite;
    [SerializeField] GameObject _item;
    [SerializeField] Vector2 _itemLaunchVelocity;
    SpriteRenderer _spriteRenderer;
    bool _used;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_item != null)
            _item.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (_used)
            return;

        var player = other.collider.GetComponent<Player>();
        if (player == null)
            return;

        if (other.contacts[0].normal.y > 0)
        {
            _spriteRenderer.sprite = _usedSprite;
            if (_item != null)
            {
                _used = true;
                _item.SetActive(true);
                var itemRigidbody = _item.GetComponent<Rigidbody2D>();
                if (itemRigidbody != null)
                    itemRigidbody.velocity = _itemLaunchVelocity;
            }
        }
    }
}