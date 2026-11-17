public class Dragon
{
    private readonly Die _die;

    public string Name { get; set; } = "";
    public int Strength { get; set; }
    public int Agility { get; set; }
    public int HealthPoints { get; set; }

    // Weapon is now a real shared class in the repo.
    public Weapon Weapon { get; set; }

    // Example use of taunts:
    // string taunt = dragon.GetRandomDefenseTaunt().Replace("{race}", player.Race);
    private readonly List<string> _defenseTauntKeys = new()
{
    "dragon_defense_taunt_1",
    "dragon_defense_taunt_2",
    "dragon_defense_taunt_3",
    "dragon_defense_taunt_4"
};

    private readonly List<string> _damageReplyKeys = new()
{
    "dragon_damage_reply_1",
    "dragon_damage_reply_2",
    "dragon_damage_reply_3",
    "dragon_damage_reply_4"
};

    public Dragon(Die die)
    {
        _die = die;

        Name = "Smolderfang";

        // Dragon stats are initialized using Die so the class aligns better
        // with the sequence/start-game diagrams and shared repo structure.
        Strength = _die.Roll(20);
        Agility = _die.Roll(20);
        HealthPoints = _die.Roll(20);

        // The dragon uses a placeholder weapon object for now.
        // This can be refined later if combat requirements expand.
        Weapon = new Weapon("claws", 12, "<<< claws >>>");
    }

    public int RollAttack()
    {
        return _die.Roll(20);
    }

    public bool AttackHits(int attackRoll)
    {
        return attackRoll <= Strength;
    }

    public int RollDamage()
    {
        return _die.Roll(Weapon.MaxDamage);
    }

    public string Attack(Player player, Messages messages)
    {
        var output = new List<string>();

        output.Add(string.Format(
            messages.GetMessage("dragon_attack_intro"),
            Name,
            player.Name,
            messages.TranslateWeaponForDisplay(Weapon.Type)
        ));

        if (!string.IsNullOrWhiteSpace(Weapon.AsciiArt))
            output.Add(Weapon.AsciiArt);

        int attackRoll = RollAttack();

        if (!AttackHits(attackRoll))
        {
            output.Add(string.Format(messages.GetMessage("dragon_missed_player"), Name, player.Name));
            return string.Join(Environment.NewLine, output);
        }

        output.Add(string.Format(messages.GetMessage("dragon_hit_player"), Name, player.Name));

        int defenseRoll = _die.Roll(20);

        if (defenseRoll <= player.Agility)
        {
            output.Add(string.Format(messages.GetMessage("player_defended_dragon_attack"), player.Name));
            return string.Join(Environment.NewLine, output);
        }

        int damage = RollDamage();
        int absorbed = player.Armor?.Absorb(damage) ?? 0;
        damage -= absorbed;
        player.HealthPoints -= damage;

        if (player.HealthPoints < 0)
            player.HealthPoints = 0;

        output.Add(string.Format(messages.GetMessage("dragon_damage_dealt"), Name, damage));

        if (absorbed > 0)
            output.Add(string.Format(messages.GetMessage("armor_absorbed"), absorbed));
        output.Add(string.Format(messages.GetMessage("player_health_now"), player.Name, player.HealthPoints));

        if (player.HealthPoints <= 0)
            output.Add(string.Format(messages.GetMessage("player_defeated_narrative"), player.Name));

        return string.Join(Environment.NewLine, output);
    }


    public string GetRandomDefenseTaunt(Messages messages)
    {
        int index = _die.Roll(_defenseTauntKeys.Count) - 1;
        return messages.GetMessage(_defenseTauntKeys[index]);
    }

    public string GetRandomDamageReply(Messages messages)
    {
        int index = _die.Roll(_damageReplyKeys.Count) - 1;
        return messages.GetMessage(_damageReplyKeys[index]);
    }

    public void DisplayStats(Messages messages)
    {
        Console.WriteLine(messages.GetMessage("dragon_stats_header"));
        Console.WriteLine(messages.GetMessage("stats_name") + Name);
        Console.WriteLine(messages.GetMessage("stats_strength") + Strength);
        Console.WriteLine(messages.GetMessage("stats_agility") + Agility);
        Console.WriteLine(messages.GetMessage("stats_health") + HealthPoints);
        Console.WriteLine(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(Weapon.Type));
        Console.WriteLine(messages.GetMessage("stats_damage") + Weapon.MaxDamage);
    }
}