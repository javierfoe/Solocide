using System;

public class Card
{
    public SpecialAbility SpecialAbility
    {
        get;
        private set;
    }

    public int DamageShielding
    {
        get;
        private set;
    }

    public int Health => DamageShielding * 2;
    
    public Card(SpecialAbility specialAbility, int damageShielding)
    {
        SpecialAbility = specialAbility;
        DamageShielding = damageShielding;
    }

    public override string ToString()
    {
        return $"{DamageShielding}-{SpecialAbility}";
    }
}
    
public enum SpecialAbility
{
    Shield,
    DoubleDamage,
    Shuffle,
    Draw
}