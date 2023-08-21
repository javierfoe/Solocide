using System;
using System.Collections.Generic;

public class Solocide
{
    private List<Card> _deck = new (), _discard = new(), _enemies = new(), _hand = new();
    private int _jesters;

    public Solocide(int jesters = 2)
    {
        _jesters = jesters;
        SetDeck();
        SetEnemies();
    }

    private void SetDeck()
    {
        var aux = new List<Card>();
        foreach (Special special in Enum.GetValues(typeof(Special)))
        {
            for (var i = 1; i < 11; ++i)
            {
                aux.Add(new Card(special, i));
            }
        }
        RandomizeList(aux, _deck);
    }

    private void SetEnemies()
    {
        List<Card> aux;
        for (var i = 10; i < 21; i += 5)
        {
            aux = new()
            {
                new Card(Special.Draw, i),
                new Card(Special.Shield, i),
                new Card(Special.Shuffle, i),
                new Card(Special.DoubleDamage, i)
            };
            
            RandomizeList(aux, _enemies);
        }
    }

    private void RandomizeList<T>(List<T> origin, List<T> destination)
    {
        for (var i = origin.Count; i > 0; --i)
        {
            var index = UnityEngine.Random.Range(0, i);
            destination.Add(origin[index]);
            origin.RemoveAt(index);
        }
    }
}