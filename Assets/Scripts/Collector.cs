using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Collector : MonoBehaviour
{
    [SerializeField] List<Collectible> _collectibles;
    [SerializeField] UnityEvent _onCollectionComplete;
    TMP_Text _remainingText;

    void Start()
    {
        _remainingText = GetComponentInChildren<TMP_Text>();
    }

    void Update()
    {
        var countRemaining = 0;
        foreach (var collectible in _collectibles)
            if (collectible.isActiveAndEnabled)
                countRemaining++;
        _remainingText?.SetText(countRemaining.ToString());
        if (countRemaining > 0)
            return;

        _onCollectionComplete.Invoke();
    }

    void OnValidate()
    {
        _collectibles = _collectibles.Distinct().ToList();
    }
}