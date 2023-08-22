using System.Collections.Generic;

public class Card
{
    private bool _selected, _available;
    public Element Element
    {
        get;
    }

    public int DamageShielding
    {
        get;
    }

    public bool Selected
    {
        get => _selected;
        set
        {
            _selected = value;
            if (value)
            {
                _available = true;
            }
        }
    }

    public bool Available
    {
        get => _available;
        set
        {
            if (_selected) return;
            _available = value;
        }
    }
    
    public Card(Element element, int damageShielding)
    {
        Element = element;
        DamageShielding = damageShielding;
    }

    public override string ToString()
    {
        return $"{(Available ? "(Available)" : "")} {DamageShielding}-{Element} {(Selected ? "(Selected)" : "")}";
    }

    public static Card CombineCards(List<Card> cards, Enemy enemy)
    {
        var elements = Element.None;
        var damageShielding = 0;
        foreach (var card in cards)
        {
            damageShielding += card.DamageShielding;
            if (card.Element == enemy.Element) continue;
            elements |= card.Element;
        }
        return new Card(elements, damageShielding);
    }
}