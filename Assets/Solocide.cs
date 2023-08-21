using System;
using System.Collections.Generic;
using System.Linq;

public class Solocide
{
    private const int MaxHandSize = 8;
    private List<Card> _deck = new (), _discard = new(), _enemies = new(), _hand = new(), _inPlay = new();
    private int _jesters;

    public Solocide(int jesters = 2)
    {
        _jesters = jesters;
        SetDeck();
        SetEnemies();
    }

    public void UseCards(List<int> cards)
    {
        foreach (var index in cards)
        {
            _inPlay.Add(_hand.RemoveElementAt(index));
        }
    }

    public void DiscardCards()
    {
        _discard.AddRange(_inPlay);
        _inPlay.Clear();
    }

    public void ShuffleCards(int amount)
    {
        var shuffle = Math.Min(_discard.Count, amount);
        _discard.Randomize();
        _deck.AddRange(_discard.RemoveElements(shuffle));
    }

    public List<Card> DrawCards(int amount)
    {
        var draw = Math.Min(Math.Min(MaxHandSize - _hand.Count, amount), _deck.Count);
        var result = _deck.RemoveElements(draw);
        _hand.AddRange(result);
        return result;
    }

    private void SetDeck()
    {
        var aux = new List<Card>();
        foreach (SpecialAbility special in Enum.GetValues(typeof(SpecialAbility)))
        {
            for (var i = 1; i < 11; ++i)
            {
                aux.Add(new Card(special, i));
            }
        }
        aux.RandomizeListTo(_deck);
    }

    private void SetEnemies()
    {
        List<Card> aux;
        for (var i = 10; i < 21; i += 5)
        {
            aux = new()
            {
                new Card(SpecialAbility.Draw, i),
                new Card(SpecialAbility.Shield, i),
                new Card(SpecialAbility.Shuffle, i),
                new Card(SpecialAbility.DoubleDamage, i)
            };
            
            aux.RandomizeListTo(_enemies);
        }
    }

    private static string EnumerableToString(IEnumerable<Card> cards)
    {
        return cards.Aggregate("", (current, card) => current + $"{card}\n");
    }

    public override string ToString()
    {
        return $"Enemies\n{EnumerableToString(_enemies)}Deck\n{EnumerableToString(_deck)}";
    }
}