public class Card
{
    public Special Special
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
    
    public Card(Special special, int damageShielding)
    {
        Special = special;
        DamageShielding = damageShielding;
    }
}
    
public enum Special
{
    Shield,
    DoubleDamage,
    Shuffle,
    Draw
}