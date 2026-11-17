public class Weapon
{
    public string Type { get; set; }
    public int MaxDamage { get; set; }
    public string AsciiArt { get; set; }

    public Weapon(string type, int maxDamage, string asciiArt)
    {
        Type = type;
        MaxDamage = maxDamage;
        AsciiArt = asciiArt;
    }

    // Set by the Merchant's Magical Weapon upgrade. The weapon keeps its type and ASCII art.
    public bool IsMagical { get; private set; }

    /// <summary>Makes this weapon magical and raises its Maximum Damage by the bonus.</summary>
    public void MakeMagical(int damageBonus)
    {
        IsMagical = true;
        MaxDamage += damageBonus;
    }
}