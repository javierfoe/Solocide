public class Enemy : Card
{
    public int Health
    {
        get;
        private set;
    }

    public int Strength
    {
        get;
        private set;
    }

    public int MaxHealth => DamageShielding * 2;
    
    public Enemy(Card card) : base(card.Element, card.DamageShielding)
    {
        Health = MaxHealth;
        Strength = DamageShielding;
    }

    public Attack Damage(int damage, Element element)
    {
        damage *= element.HasFlag(Element.Fire) ? 2 : 1;
        Health -= damage;
        Strength -= element.HasFlag(Element.Earth) ? damage : 0;
        return Health < 0 ? Attack.Dead : Health > 0 ? Attack.Alive : Attack.Recruit;
    }
    
    public override string ToString()
    {
        return $"{Health}-{Strength}-{Element}";
    }
}