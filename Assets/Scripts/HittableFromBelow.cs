using UnityEngine;

public class HittableFromBelow : MonoBehaviour
{
    [SerializeField] protected Sprite _usedSprite;
    Animator _animator;

    protected virtual bool CanUse => true;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!CanUse)
            return;

        var player = other.collider.GetComponent<Player>();
        if (player == null)
            return;

        if (other.contacts[0].normal.y > 0)
        {
            PlayAnimation();
            Use();
            if (!CanUse)
                GetComponent<SpriteRenderer>().sprite = _usedSprite;
        }
    }

    void PlayAnimation()
    {
        if (_animator != null)
            _animator.SetTrigger("Use");
    }

    protected virtual void Use()
    {
        Debug.Log($"Used {gameObject.name}");
    }
}