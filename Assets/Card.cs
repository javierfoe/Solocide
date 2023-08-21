using System;
using System.Collections.Generic;

public class Card
{
    public Element Element
    {
        get;
        private set;
    }

    public int DamageShielding
    {
        get;
        private set;
    }
    
    public Card(Element element, int damageShielding)
    {
        Element = element;
        DamageShielding = damageShielding;
    }

    public override string ToString()
    {
        return $"{DamageShielding}-{Element}";
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