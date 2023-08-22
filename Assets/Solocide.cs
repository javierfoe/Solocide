using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.XR;

public class Solocide
{
    private const int MaxHandSize = 8, MaxComboCardStrength = 6, MaxComboStrength= 12;
    private readonly List<Card> _deck = new (), _discard = new(), _enemies = new(), _hand = new(), _inPlay = new(), _selectedCards = new();
    private int _jesters;
    private Enemy _currentEnemy;

    public readonly UnityEvent<Card> AddCardToHandEvent = new();
    public readonly UnityEvent<Enemy> UpdateEnemyCardEvent = new();
    public readonly UnityEvent<int> RemoveCardFromHandEvent = new(), UpdateDeckCountEvent = new(), UpdateDiscardCountEvent = new(), UpdatePlayCountEvent = new(), UpdateJestersCountEvent = new();
    public readonly UnityEvent<int, Card> UpdateHandCardEvent = new();
    public readonly UnityEvent ClearHandEvent = new();
    
    public Solocide(int jesters = 2)
    {
        _jesters = jesters;
        SetDeck();
        SetEnemies();
        FillHand();
    }

    public void AvailableCardsAfterSelection()
    {
        var notSelectedCards = _hand.FindAll((card) => !card.Selected);

        var count = _selectedCards.Count;
        if (count == 0) return;
        var card = _selectedCards.Find((card) => card.DamageShielding > 1);
        if (count == 1 && card == null)
        {
            SetAvailabilityCards(notSelectedCards, true);
            return;
        }
        if (count == 2 && card == null)
        {
            SetAvailabilityAllCards(false);
            return;
        }
        
        var combinedCard = CombineCards(_selectedCards);
        if (card.DamageShielding < MaxComboCardStrength)
        { 
            if(combinedCard.DamageShielding < MaxComboStrength)
            {
                notSelectedCards.ForEach((notSelectedCard) =>
                {
                    if (notSelectedCard.DamageShielding + combinedCard.DamageShielding < MaxComboStrength)
                    {
                        SetAvailabilityCard(notSelectedCard, true);
                    }
                });
            }
        }
        else switch (count)
        {
            case 1:
                notSelectedCards.ForEach((notSelectedCard) => SetAvailabilityCard(notSelectedCard, notSelectedCard.DamageShielding == 1));
                break;
            case 2:
                SetAvailabilityCards(notSelectedCards, false);
                break;
        }
    }

    public void UseJester()
    {
        _discard.AddRange(_hand);
        TriggerUpdateDiscardEvent();
        
        _hand.Clear();
        ClearHandEvent.Invoke();
        
        FillHand();
        
        UpdateJestersCountEvent.Invoke(--_jesters);
    }

    public void AttackCards(List<int> cards)
    {
        var attackCard = CombineCards(cards);
        PlayCards(attackCard);
        TriggerUpdatePlayEvent();
    }

    public void DefenseCards(List<int> cards)
    {
        AddCardsPlay(cards);
        DiscardCards();
        TriggerUpdatePlayEvent();
    }

    public void DiscardCards()
    {
        _discard.AddRange(_inPlay);
        _inPlay.Clear();
        TriggerUpdateDiscardEvent();
        TriggerUpdatePlayEvent();
    }

    public void ShuffleCards(int amount)
    {
        var shuffle = Math.Min(_discard.Count, amount);
        _discard.Randomize();
        _deck.AddRange(_discard.RemoveElements(shuffle));
        
        TriggerUpdateDiscardEvent();
        TriggerUpdateDeckEvent();
    }

    public void DrawCards(int amount)
    {
        var draw = Math.Min(Math.Min(MaxHandSize - _hand.Count, amount), _deck.Count);
        var drawnCards = _deck.RemoveElements(draw);
        _hand.AddRange(drawnCards);
        
        foreach (var card in drawnCards)
        {
            AddCardToHandEvent.Invoke(card);
        }
        TriggerUpdateDeckEvent();
    }

    private void SetAvailabilityCard(Card card, bool availability)
    {
        card.Available = availability;
        TriggerUpdateHandCardEvent(card);
    }

    private void SetAvailabilityCards(List<Card> cards, bool availability)
    {
        cards.ForEach((card) => SetAvailabilityCard(card, availability));
    }

    private void SetAvailabilityAllCards(bool availability)
    {
        SetAvailabilityCards(_hand, availability);
    }

    private Card CombineCards(List<int> cards)
    {
        var currentUsedCards = AddCardsPlay(cards);
        return CombineCards(currentUsedCards);
    }

    private Card CombineCards(List<Card> cards)
    {
        return Card.CombineCards(cards, _currentEnemy);
    }

    private void FillHand()
    {
        DrawCards(MaxHandSize);
    }

    private void PlayCards(Card card)
    {
        var element = card.Element;
        var damageShielding = card.DamageShielding;
        
        if (element.HasFlag(Element.Water))
        {
            ShuffleCards(damageShielding);
        }

        if (element.HasFlag(Element.Wind))
        {
            DrawCards(damageShielding);
        }

        var attackResult = _currentEnemy.Damage(damageShielding, element);
        TriggerUpdateEnemyCardEvent();

        switch (attackResult)
        {
            case Attack.Alive:
                return;
            case Attack.Dead:
                _inPlay.Add(_currentEnemy);
                break;
            case Attack.Recruit:
                _deck.Insert(0, _currentEnemy);
                break;
        }

        DiscardCards();
        
        _enemies.RemoveAt(0);
        SetNewEnemy();
    }

    private List<Card> AddCardsPlay(List<int> cards)
    {
        var currentUsedCards = new List<Card>();
        foreach (var index in cards)
        {
            currentUsedCards.Add(RemoveCardFromHand(index));
        }
        _inPlay.AddRange(currentUsedCards);
        return currentUsedCards;
    }

    private Card RemoveCardFromHand(int index)
    {
        RemoveCardFromHandEvent.Invoke(index);
        return _hand.RemoveElementAt(index);
    }

    private void TriggerUpdateDeckEvent()
    {
        UpdateDeckCountEvent.Invoke(_deck.Count);
    }

    private void TriggerUpdateDiscardEvent()
    {
        UpdateDiscardCountEvent.Invoke(_discard.Count);
    }

    private void TriggerUpdatePlayEvent()
    {
        UpdatePlayCountEvent.Invoke(_inPlay.Count);
    }

    private void TriggerUpdateEnemyCardEvent()
    {
        UpdateEnemyCardEvent.Invoke(_currentEnemy);
    }

    private void TriggerUpdateHandCardEvent(Card card)
    {
        UpdateHandCardEvent.Invoke(_hand.IndexOf(card), card);
    }

    private void SetDeck()
    {
        var aux = new List<Card>();
        foreach (Element element in Enum.GetValues(typeof(Element)))
        {
            if(element == Element.None) continue;
            for (var i = 1; i < 11; ++i)
            {
                aux.Add(new Card(element, i));
            }
        }
        aux.RandomizeListTo(_deck);
    }

    private void SetEnemies()
    {
        for (var i = 10; i < 21; i += 5)
        {
            List<Card> aux = new()
            {
                new Card(Element.Wind, i),
                new Card(Element.Earth, i),
                new Card(Element.Water, i),
                new Card(Element.Fire, i)
            };

            aux.RandomizeListTo(_enemies);
        }
        SetNewEnemy();
    }

    private void SetNewEnemy()
    {
        _currentEnemy = new Enemy(_enemies[0]);
        TriggerUpdateEnemyCardEvent();
    }

    private static string EnumerableToString(IEnumerable<Card> cards)
    {
        return cards.Aggregate("", (current, card) => current + $"{card}\n");
    }

    public string CurrentState()
    {
        return $"CurrentEnemy: {_currentEnemy}\n\nHand:\n{EnumerableToString(_hand)}\nEnemies:{_enemies.Count}\nDeck:{_deck.Count}\nDiscard:{_discard.Count}";
    }

    public override string ToString()
    {
        return $"Enemies\n{EnumerableToString(_enemies)}Deck\n{EnumerableToString(_deck)}";
    }
}