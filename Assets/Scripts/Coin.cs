using UnityEngine;

public class Coin : MonoBehaviour
{
    static int _coinsCollected;

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;
        gameObject.SetActive(false);
        _coinsCollected++;
        print(_coinsCollected);
    }
}