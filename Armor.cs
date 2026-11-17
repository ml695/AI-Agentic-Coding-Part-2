/// <summary>
/// An item the player wears. Each dragon hit that lands and isn't blocked deals
/// Protection less damage, never below 0. It doesn't change the Agility block roll.
/// </summary>
public class Armor
{
    public const int MagicalProtection = 3;

    // Canonical name; translated for display through the language file's displayArmorMap.
    public string Type { get; }
    public int Protection { get; }

    public Armor(string type, int protection)
    {
        Type = type;
        Protection = protection;
    }

    /// <summary>The Magical Armor sold by the Merchant.</summary>
    public static Armor Magical() => new Armor("magical armor", MagicalProtection);

    /// <summary>Returns how much of a hit's damage this armor absorbs (at most the whole hit).</summary>
    public int Absorb(int damage) => Math.Min(Protection, Math.Max(damage, 0));
}
