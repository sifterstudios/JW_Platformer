using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Sprite openMid;
    [SerializeField] Sprite openTop;

    [SerializeField] SpriteRenderer rendererMid;
    [SerializeField] SpriteRenderer rendererTop;
    [SerializeField] int requiredCoins = 3;
    [SerializeField] Door exit;
    [SerializeField] Canvas canvas;

    bool _open;

    void Update()
    {
        if (_open == false && Coin.CoinsCollected >= requiredCoins) Open();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_open == false)
            return;
        var player = other.GetComponent<Player>();
        if (player != null && exit != null) player.TeleportTo(exit.transform.position);
    }


    [ContextMenu("Open Door")]
    public void Open()
    {
        if (canvas != null)
            canvas.enabled = false;
        _open = true;
        rendererMid.sprite = openMid;
        rendererTop.sprite = openTop;
    }
}