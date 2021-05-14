using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    readonly List<Collector> _collectors = new List<Collector>();

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;
        gameObject.SetActive(false);
        foreach (var collector in _collectors) collector.ItemPickedUp();
    }

    public void AddCollector(Collector collector)
    {
        _collectors.Add(collector);
    }
}