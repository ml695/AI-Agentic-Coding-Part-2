using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
[TestClass]
public class DragonTests
{

    private Messages GetMessages(string language = "English")
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    [TestMethod]
    public void NewDragon_InitializesRequiredProperties()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Assert.AreEqual("Smolderfang", dragon.Name);
        Assert.IsTrue(dragon.Strength >= 1 && dragon.Strength <= 20);
        Assert.IsTrue(dragon.Agility >= 1 && dragon.Agility <= 20);
        Assert.IsTrue(dragon.HealthPoints >= 1 && dragon.HealthPoints <= 20);

        Assert.IsNotNull(dragon.Weapon);
        Assert.AreEqual("claws", dragon.Weapon.Type);
        Assert.AreEqual(12, dragon.Weapon.MaxDamage);
        Assert.IsFalse(string.IsNullOrWhiteSpace(dragon.Weapon.AsciiArt));
    }

    [TestMethod]
    public void GetRandomDefenseTaunt_ReturnsExpectedTaunt()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);
        Messages messages = GetMessages();

        string taunt = dragon.GetRandomDefenseTaunt(messages);

        string[] validTaunts =
        {
        messages.GetMessage("dragon_defense_taunt_1"),
        messages.GetMessage("dragon_defense_taunt_2"),
        messages.GetMessage("dragon_defense_taunt_3"),
        messages.GetMessage("dragon_defense_taunt_4")
    };

        CollectionAssert.Contains(validTaunts, taunt);
    }

    [TestMethod]
    public void GetRandomDamageReply_ReturnsExpectedReply()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);
        Messages messages = GetMessages();

        string reply = dragon.GetRandomDamageReply(messages);

        string[] validReplies =
        {
        messages.GetMessage("dragon_damage_reply_1"),
        messages.GetMessage("dragon_damage_reply_2"),
        messages.GetMessage("dragon_damage_reply_3"),
        messages.GetMessage("dragon_damage_reply_4")
    };

        CollectionAssert.Contains(validReplies, reply);
    }

    [TestMethod]
    public void Attack_ReturnsNonEmptyResultString()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Player player = new Player
        {
            Name = "TestHero",
            Agility = 10,
            HealthPoints = 30
        };

        string result = dragon.Attack(player, GetMessages());

        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
    }

    [TestMethod]
    public void Attack_DoesNotIncreasePlayerHealth()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Player player = new Player
        {
            Name = "TestHero",
            Agility = 10,
            HealthPoints = 30
        };

        int startingHealth = player.HealthPoints;

        string result = dragon.Attack(player, GetMessages());

        Assert.IsTrue(player.HealthPoints <= startingHealth);
    }

    [TestMethod]
    public void Attack_WithFixedDice_DealsScriptedDamage()
    {
        // Dragon stats: strength 15, agility 10, health 10.
        // Attack roll 5 hits (5 <= 15), block roll 18 fails (18 > agility 10), damage roll 7.
        var die = new FixedDie(15, 10, 10, 5, 18, 7);
        Dragon dragon = new Dragon(die);
        Player player = new Player { Name = "TestHero", Agility = 10, HealthPoints = 30 };
        Messages messages = GetMessages();

        string result = dragon.Attack(player, messages);

        Assert.AreEqual(23, player.HealthPoints);
        StringAssert.Contains(result, string.Format(messages.GetMessage("dragon_damage_dealt"), dragon.Name, 7));
    }

    [TestMethod]
    public void Attack_KeepsPlayerHealthNonNegative()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Player player = new Player
        {
            Name = "TestHero",
            Agility = 0,
            HealthPoints = 1
        };

        string result = dragon.Attack(player, GetMessages());

        Assert.IsTrue(player.HealthPoints >= 0);
    }
}

[TestClass]
[DoNotParallelize]
public class GamePbi18Tests
{
    private TextReader? _originalIn;
    private TextWriter? _originalOut;

    [TestInitialize]
    public void Setup()
    {
        _originalIn = Console.In;
        _originalOut = Console.Out;
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (_originalIn != null)
            Console.SetIn(_originalIn);

        if (_originalOut != null)
            Console.SetOut(_originalOut);
    }

    [TestMethod]
    public void DragonDisplayStats_UsesLocalizedLabels()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        dragon.DisplayStats(messages);

        string output = writer.ToString();

        StringAssert.Contains(output, messages.GetMessage("dragon_stats_header"));
        StringAssert.Contains(output, messages.GetMessage("stats_name"));
        StringAssert.Contains(output, messages.GetMessage("stats_strength"));
        StringAssert.Contains(output, messages.GetMessage("stats_agility"));
        StringAssert.Contains(output, messages.GetMessage("stats_health"));
        StringAssert.Contains(output, messages.GetMessage("stats_weapon"));
        StringAssert.Contains(output, messages.GetMessage("stats_damage"));
    }

    [TestMethod]
    public void HandleDragonEncounter_AttackPath_DisplaysEncounterFlowText()
    {
        Game game = new Game();
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Player player = new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 20,
            Agility = 20,
            HealthPoints = 25,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };

        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        // "a" enters combat, then "r" retreats from Combat immediately
        Console.SetIn(new StringReader("a\nr\n"));

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        MethodInfo? method = typeof(Game).GetMethod(
            "HandleDragonEncounter",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        Assert.IsNotNull(method, "HandleDragonEncounter method was not found.");

        method!.Invoke(game, new object[] { player, dragon, messages });

        string output = writer.ToString();

        StringAssert.Contains(output, messages.GetMessage("north_path_narrative"));
        StringAssert.Contains(output, string.Format(messages.GetMessage("dragon_intro"), dragon.Name));
        StringAssert.Contains(output, messages.GetMessage("dragon_stats_intro"));
        StringAssert.Contains(output, messages.GetMessage("dragon_encounter_prompt"));
    }

    [TestMethod]
    public void DragonEncounter_MessageKeys_AreLoaded()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        Assert.AreNotEqual("[north_path_narrative]", messages.GetMessage("north_path_narrative"));
        Assert.AreNotEqual("[dragon_intro]", messages.GetMessage("dragon_intro"));
        Assert.AreNotEqual("[dragon_stats_intro]", messages.GetMessage("dragon_stats_intro"));
        Assert.AreNotEqual("[dragon_encounter_prompt]", messages.GetMessage("dragon_encounter_prompt"));
        Assert.AreNotEqual("[dragon_stats_header]", messages.GetMessage("dragon_stats_header"));
    }
}
