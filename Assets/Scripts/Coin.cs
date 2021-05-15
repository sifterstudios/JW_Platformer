using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public static int CoinsCollected;
    [SerializeField] List<AudioClip> _clips;

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;

        CoinsCollected++;

        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;

        if (_clips.Count > 0)
        {
            var randomIndex = Random.Range(0, _clips.Count);
            var clip = _clips[randomIndex];
            GetComponent<AudioSource>().PlayOneShot(clip);
        }

        GetComponent<AudioSource>().Play();


        ScoreSystem.Add(100);
    }
}