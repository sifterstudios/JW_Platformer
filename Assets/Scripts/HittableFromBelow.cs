using UnityEngine;

public class HittableFromBelow : MonoBehaviour
{
    [SerializeField] protected Sprite _usedSprite;

    protected virtual bool CanUse => true;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!CanUse)
            return;

        var player = other.collider.GetComponent<Player>();
        if (player == null)
            return;

        if (other.contacts[0].normal.y > 0)
        {
            Use();
            if (!CanUse)
                GetComponent<SpriteRenderer>().sprite = _usedSprite;
        }
    }

    protected virtual void Use()
    {
        Debug.Log($"Used {gameObject.name}");
    }
}