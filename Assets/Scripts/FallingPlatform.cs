using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public bool PlayerInside;

    [Tooltip("Resets the wiggle timer when no players are on the platform")] [SerializeField]
    bool _resetOnEmpty;

    [SerializeField] float _fallSpeed = 3;
    [Range(0.1f, 5f)] [SerializeField] float _fallAfterSeconds = 9;
    [Range(0.005f, 0.1f)] [SerializeField] float _shakeX = 0.005f;
    [Range(0.005f, 0.1f)] [SerializeField] float _shakeY = 0.005f;
    readonly HashSet<Player> _playersInTrigger = new HashSet<Player>();
    bool _falling;
    Vector3 _initialPosition;
    float _wiggleTimer;

    void Start()
    {
        _initialPosition = transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;

        _playersInTrigger.Add(player);

        PlayerInside = true;
        if (_playersInTrigger.Count == 1)
            StartCoroutine(WiggleAndFall());
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (_falling)
            return;
        var player = other.GetComponent<Player>();
        if (player == null)
            return;
        _playersInTrigger.Remove(player);
        if (_playersInTrigger.Count == 0)
        {
            PlayerInside = false;
            StopCoroutine(WiggleAndFall());
        }
    }

    IEnumerator WiggleAndFall()
    {
        Debug.Log("Waiting to wiggle");
        yield return new WaitForSeconds(0.25f);
        Debug.Log("Wiggling");
        if (_resetOnEmpty)
            _wiggleTimer = 0;

        while (_wiggleTimer < _fallAfterSeconds)
        {
            var randomX = Random.Range(-_shakeX, _shakeX);
            var randomY = Random.Range(-_shakeY, _shakeY);
            transform.position = _initialPosition + new Vector3(randomX, randomY);
            var randomDelay = Random.Range(0.005f, 0.01f);
            yield return new WaitForSeconds(randomDelay);
            _wiggleTimer += randomDelay;
        }

        Debug.Log("Falling");
        _falling = true;
        foreach (var col in GetComponents<Collider2D>()) col.enabled = false;

        float fallTimer = 0;
        while (fallTimer < 3f)
        {
            transform.position += Vector3.down * (Time.deltaTime * _fallSpeed);
            fallTimer += Time.deltaTime;
            Debug.Log(fallTimer);
            yield return null;
        }

        Destroy(gameObject);
    }
}