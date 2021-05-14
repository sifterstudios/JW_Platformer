using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Collector : MonoBehaviour
{
    [SerializeField] List<Collectible> _collectibles;
    [SerializeField] UnityEvent _onCollectionComplete;

    int _countCollected;
    TMP_Text _remainingText;

    void Start()
    {
        _remainingText = GetComponentInChildren<TMP_Text>();

        foreach (var collectible in _collectibles) collectible.OnPickedUp += ItemPickedUp;

        var countRemaining = _collectibles.Count - _countCollected;
        _remainingText?.SetText(countRemaining.ToString());
    }


    void OnValidate()
    {
        _collectibles = _collectibles.Distinct().ToList();
    }

    public void ItemPickedUp()
    {
        _countCollected++;
        var countRemaining = _collectibles.Count - _countCollected;

        _remainingText?.SetText(countRemaining.ToString());
        if (countRemaining > 0)
            return;

        _onCollectionComplete.Invoke();
    }
}