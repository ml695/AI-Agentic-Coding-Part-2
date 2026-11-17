// CYOATests/GamePathTests.cs
// Plays scripted games through Game.StartGame by redirecting the console.
[TestClass]
[DoNotParallelize]
public class GamePathTests
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
        if (_originalIn != null) Console.SetIn(_originalIn);
        if (_originalOut != null) Console.SetOut(_originalOut);
    }

    // language, main menu, race, name, occupation, four rolls (strength, agility, health, gold),
    // "proceed on an adventure"
    private const string CreateCharacterInput = "1\n1\nhuman\nBob\nfighter\nroll\nroll\nroll\nroll\n2\n";

    private static string PlayGame(string input, Die? die = null)
    {
        Console.SetIn(new StringReader(input));
        var output = new StringWriter();
        Console.SetOut(output);

        // If the game asks for more input than the script provides, it would wait forever;
        // the timeout turns that into a test failure instead of a hung test run.
        var game = Task.Run(() => new Game(die).StartGame());
        bool finished = game.Wait(TimeSpan.FromSeconds(5));

        Assert.IsTrue(finished, "The game did not end; it is still waiting for input.");
        return output.ToString();
    }

    private static Messages English()
    {
        var messages = new Messages();
        messages.SetCurrentLanguage("English");
        return messages;
    }

    private static int CountOccurrences(string text, string value)
    {
        int count = 0, index = 0;
        while ((index = text.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += value.Length;
        }
        return count;
    }

    // Dragon: strength, agility, health. Player (Human): strength, modifier, agility, health, gold.
    // The last roll (9) gives 90 Gold.
    private static FixedDie DiceWith90Gold() => new FixedDie(5, 6, 7, 15, 3, 12, 9, 9);

    [TestMethod]
    public void SouthPath_ShowsMerchantWithBothItemsPricesAndGold()
    {
        Messages messages = English();

        string output = PlayGame(CreateCharacterInput + "s\n3\ne\n", DiceWith90Gold());

        StringAssert.Contains(output, messages.GetMessage("merchant_greeting"));
        StringAssert.Contains(output, "1. Magical Armor (80 Gold): reduces the damage of each dragon hit by 3.");
        StringAssert.Contains(output, "2. Magical Weapon (120 Gold): adds 4 to your weapon's Maximum Damage.");
        StringAssert.Contains(output, messages.GetMessage("merchant_leave_option"));
        StringAssert.Contains(output, "You have 90 Gold.");
    }

    [TestMethod]
    public void SouthPath_LeavingTheMerchant_KeepsGoldAndReturnsToPathChoice()
    {
        Messages messages = English();

        // South, leave, south again, leave, exit. The second visit shows whether Gold changed.
        string output = PlayGame(CreateCharacterInput + "s\n3\ns\n3\ne\n", DiceWith90Gold());

        StringAssert.Contains(output, messages.GetMessage("merchant_farewell"));
        Assert.AreEqual(3, CountOccurrences(output, messages.GetMessage("path_prompt_full")),
            "The path question should be asked again after each visit to the Merchant.");
        Assert.AreEqual(2, CountOccurrences(output, "You have 90 Gold."),
            "Leaving the Merchant should not change the player's Gold.");
        StringAssert.Contains(output, messages.GetMessage("adventure_ends"));
    }

    [TestMethod]
    public void Merchant_RejectsInvalidChoice_AndAsksAgain()
    {
        Messages messages = English();

        string output = PlayGame(CreateCharacterInput + "s\n9\n3\ne\n", DiceWith90Gold());

        StringAssert.Contains(output, messages.GetMessage("merchant_invalid"));
        Assert.AreEqual(2, CountOccurrences(output, messages.GetMessage("merchant_leave_option")),
            "The Merchant menu should be shown again after invalid input.");
        StringAssert.Contains(output, messages.GetMessage("merchant_farewell"));
    }

    // Same rolls as DiceWith90Gold, but the gold roll (15) gives 150 Gold.
    private static FixedDie DiceWith150Gold(params int[] combatRolls) =>
        new FixedDie(new[] { 5, 6, 7, 15, 3, 12, 9, 15 }.Concat(combatRolls).ToArray());

    [TestMethod]
    public void BuyingWeapon_WithEnoughGold_Deducts120_AndReturnsToPathChoice()
    {
        Messages messages = English();

        // South, buy the weapon (ends the visit), south again to see the Gold, leave, exit.
        string output = PlayGame(CreateCharacterInput + "s\n2\ns\n3\ne\n", DiceWith150Gold());

        StringAssert.Contains(output, messages.GetMessage("merchant_weapon_bought"));
        Assert.AreEqual(3, CountOccurrences(output, messages.GetMessage("path_prompt_full")),
            "Buying should end the visit and return to the path choice.");
        StringAssert.Contains(output, "You have 30 Gold."); // 150 - 120
        StringAssert.Contains(output, messages.GetMessage("adventure_ends"));
    }

    [TestMethod]
    public void BuyingWeapon_WithoutEnoughGold_ShowsCantAfford_KeepsGold_AndAsksAgain()
    {
        Messages messages = English();

        string output = PlayGame(CreateCharacterInput + "s\n2\n3\ne\n", DiceWith90Gold());

        StringAssert.Contains(output, "The Merchant shakes their head. 'That costs 120 Gold, and you only have 90.'");
        Assert.AreEqual(2, CountOccurrences(output, "You have 90 Gold."),
            "A failed purchase should leave Gold unchanged and show the menu again.");
        Assert.IsFalse(output.Contains(messages.GetMessage("merchant_weapon_bought")));
        StringAssert.Contains(output, messages.GetMessage("merchant_farewell"));
    }

    [TestMethod]
    public void AfterBuyingWeapon_CombatStatsAndAttack_ShowTheMagicalWeapon()
    {
        Messages messages = English();
        // After character creation: the player's attack roll 20 misses (strength 18),
        // the dragon's attack roll 20 misses (strength 5); then the player retreats.
        string output = PlayGame(CreateCharacterInput + "s\n2\nn\na\nr\n", DiceWith150Gold(20, 20));

        int combatStats = output.IndexOf(messages.GetMessage("combat_stats_header"), StringComparison.Ordinal);
        Assert.IsTrue(combatStats >= 0, "The combat stats should be shown before the fight.");
        string fight = output[combatStats..];
        StringAssert.Contains(fight, "Weapon: magical long sword" + Environment.NewLine);
        StringAssert.Contains(fight, "Weapon's Maximum Damage Points: 16" + Environment.NewLine); // 12 + 4
        StringAssert.Contains(fight, "As a Fighter, you withdraw your magical long sword and attack the dragon.");
        StringAssert.Contains(fight, "-)=====>" + Environment.NewLine); // the fighter's art is kept
    }

    [TestMethod]
    public void WithoutBuyingWeapon_CombatStatsAndAttack_ShowTheOrdinaryWeapon()
    {
        Messages messages = English();
        string output = PlayGame(CreateCharacterInput + "n\na\nr\n", DiceWith150Gold(20, 20));

        string fight = output[output.IndexOf(messages.GetMessage("combat_stats_header"), StringComparison.Ordinal)..];
        StringAssert.Contains(fight, "Weapon: long sword" + Environment.NewLine);
        StringAssert.Contains(fight, "Weapon's Maximum Damage Points: 12" + Environment.NewLine);
        StringAssert.Contains(fight, "As a Fighter, you withdraw your long sword and attack the dragon.");
    }

    [TestMethod]
    public void CharacterStats_ShowTheMagicalWeapon_AndUpgradedDamage()
    {
        // The game only offers the stats summary before the adventure, so this calls it directly.
        var output = new StringWriter();
        Console.SetOut(output);
        var weapon = new Weapon("dagger", 6, "-)==>");
        weapon.MakeMagical(Merchant.WeaponDamageBonus);
        var player = new Player { Name = "Bob", Weapon = weapon };

        player.DisplayStats(English());

        StringAssert.Contains(output.ToString(), "Weapon: magical dagger" + Environment.NewLine);
        StringAssert.Contains(output.ToString(), "Weapon's Maximum Damage Points: 10" + Environment.NewLine);
    }

    [TestMethod]
    public void BuyingArmor_WithEnoughGold_Deducts80_AndReturnsToPathChoice()
    {
        Messages messages = English();

        // South, buy armor (ends the visit), south again to see the Gold, leave, exit.
        string output = PlayGame(CreateCharacterInput + "s\n1\ns\n3\ne\n", DiceWith90Gold());

        StringAssert.Contains(output, messages.GetMessage("merchant_armor_bought"));
        Assert.AreEqual(3, CountOccurrences(output, messages.GetMessage("path_prompt_full")),
            "Buying should end the visit and return to the path choice.");
        StringAssert.Contains(output, "You have 10 Gold."); // 90 - 80
        StringAssert.Contains(output, messages.GetMessage("adventure_ends"));
    }

    [TestMethod]
    public void BuyingArmor_WithoutEnoughGold_ShowsCantAfford_KeepsGold_AndAsksAgain()
    {
        Messages messages = English();
        // The last roll (7) gives 70 Gold, less than the armor's 80.
        var die = new FixedDie(5, 6, 7, 15, 3, 12, 9, 7);

        string output = PlayGame(CreateCharacterInput + "s\n1\n3\ne\n", die);

        StringAssert.Contains(output, "The Merchant shakes their head. 'That costs 80 Gold, and you only have 70.'");
        Assert.AreEqual(2, CountOccurrences(output, "You have 70 Gold."),
            "A failed purchase should leave Gold unchanged and show the menu again.");
        Assert.IsFalse(output.Contains(messages.GetMessage("merchant_armor_bought")));
        StringAssert.Contains(output, messages.GetMessage("merchant_farewell"));
    }

    [TestMethod]
    public void SouthPath_LeaveThenExit_EndsGameNormally()
    {
        Messages messages = English();

        string output = PlayGame(CreateCharacterInput + "s\n3\ne\n", DiceWith90Gold());

        StringAssert.Contains(output, messages.GetMessage("merchant_farewell"));
        StringAssert.Contains(output, messages.GetMessage("adventure_ends"));
    }

    [TestMethod]
    public void Merchant_InputRunningOut_CountsAsLeaving_AndGameEnds()
    {
        Messages messages = English();

        // The script stops while the Merchant is waiting for a choice; PlayGame's timeout
        // fails the test if the game loops instead of ending.
        string output = PlayGame(CreateCharacterInput + "s\n", DiceWith90Gold());

        StringAssert.Contains(output, messages.GetMessage("merchant_farewell"));
        StringAssert.Contains(output, messages.GetMessage("adventure_ends"));
    }

    [TestMethod]
    public void SouthPath_LeaveThenNorth_ReachesTheDragon()
    {
        Messages messages = English();

        // South, leave, north, then retreat from the dragon.
        string output = PlayGame(CreateCharacterInput + "s\n3\nn\nr\n", DiceWith90Gold());

        StringAssert.Contains(output, messages.GetMessage("merchant_farewell"));
        StringAssert.Contains(output, messages.GetMessage("north_path_narrative"));
        StringAssert.Contains(output, messages.GetMessage("dragon_encounter_prompt"));
        StringAssert.Contains(output, messages.GetMessage("retreat"));
    }

    [TestMethod]
    public void NorthPath_WithoutVisitingMerchant_ReachesTheDragon()
    {
        Messages messages = English();

        string output = PlayGame(CreateCharacterInput + "n\nr\n", DiceWith90Gold());

        Assert.IsFalse(output.Contains(messages.GetMessage("merchant_greeting")),
            "The Merchant should only appear on the south path.");
        StringAssert.Contains(output, messages.GetMessage("north_path_narrative"));
        StringAssert.Contains(output, messages.GetMessage("retreat"));
    }

    [TestMethod]
    public void FixedDice_GiveRepeatableCharacterStats()
    {
        Messages messages = English();
        // Dragon: strength, agility, health. Player (Human): strength d20, strength modifier d4,
        // agility d20, health d20, gold d20. Humans add the modifier to strength and health.
        var die = new FixedDie(5, 6, 7, 15, 3, 12, 9, 4);

        // CreateCharacterInput ends with "2" (proceed), so list stats ("1") first: replace that last line.
        string input = CreateCharacterInput[..^2] + "1\n2\ne\n";
        string output = PlayGame(input, die);

        StringAssert.Contains(output, messages.GetMessage("stats_strength") + 18 + Environment.NewLine); // 15 + 3
        StringAssert.Contains(output, messages.GetMessage("stats_agility") + 12 + Environment.NewLine);
        StringAssert.Contains(output, messages.GetMessage("stats_health") + 12 + Environment.NewLine);   // 9 + 3
    }

    [TestMethod]
    public void CharacterStats_ShowMaximumHealthAndHealingPotions()
    {
        Messages messages = English();
        // Same rolls as FixedDice_GiveRepeatableCharacterStats: health 9 + 3 = 12.
        var die = new FixedDie(5, 6, 7, 15, 3, 12, 9, 4);

        // CreateCharacterInput ends with "2" (proceed), so list stats ("1") first: replace that last line.
        string input = CreateCharacterInput[..^2] + "1\n2\ne\n";
        string output = PlayGame(input, die);

        StringAssert.Contains(output, messages.GetMessage("stats_max_health") + 12 + Environment.NewLine);
        StringAssert.Contains(output, messages.GetMessage("stats_healing_potions") + 0 + Environment.NewLine);
    }

    [TestMethod]
    public void GoldRoll_IsD20TimesTen_AndShownInStats()
    {
        Messages messages = English();
        // Dragon: strength, agility, health. Player: strength, modifier, agility, health, gold.
        var die = new FixedDie(5, 6, 7, 15, 3, 12, 9, 7);

        string input = CreateCharacterInput[..^2] + "1\n2\ne\n";
        string output = PlayGame(input, die);

        StringAssert.Contains(output, messages.GetMessage("roll_gold"));
        StringAssert.Contains(output, messages.GetMessage("stats_gold") + 70 + Environment.NewLine);
    }

    [TestMethod]
    public void GoldRoll_RejectsAnythingButRoll_AndAsksAgain()
    {
        Messages messages = English();

        // Strength, agility and health rolls, then a typo at the gold prompt before "roll".
        string input = "1\n1\nhuman\nBob\nfighter\nroll\nroll\nroll\ngold\nroll\n2\ne\n";
        string output = PlayGame(input);

        Assert.AreEqual(2, CountOccurrences(output, messages.GetMessage("roll_gold")),
            "The gold prompt should be asked again after invalid input.");
        StringAssert.Contains(output, messages.GetMessage("roll_invalid"));
    }

    [TestMethod]
    public void CharacterStats_ShowNoArmor_BeforeBuying()
    {
        string input = CreateCharacterInput[..^2] + "1\n2\ne\n";
        string output = PlayGame(input, DiceWith90Gold());

        StringAssert.Contains(output, "Armor: none" + Environment.NewLine);
    }

    [TestMethod]
    public void CharacterStats_ShowTheArmorsName_OnceOwned()
    {
        // The game only offers the stats summary before the adventure, so this calls it directly.
        var output = new StringWriter();
        Console.SetOut(output);
        var player = new Player { Name = "Bob", Armor = Armor.Magical() };

        player.DisplayStats(English());

        StringAssert.Contains(output.ToString(), "Armor: Magical Armor" + Environment.NewLine);
    }

    [TestMethod]
    public void CombatStats_IncludeTheArmor_AfterBuyingIt()
    {
        Messages messages = English();
        // After character creation (90 Gold): the player's attack roll 20 misses (strength 18),
        // the dragon's attack roll 20 misses (strength 5); then the player retreats.
        var die = new FixedDie(5, 6, 7, 15, 3, 12, 9, 9, 20, 20);

        string output = PlayGame(CreateCharacterInput + "s\n1\nn\na\nr\n", die);

        int combatStats = output.IndexOf(messages.GetMessage("combat_stats_header"), StringComparison.Ordinal);
        Assert.IsTrue(combatStats >= 0, "The combat stats should be shown before the fight.");
        StringAssert.Contains(output[combatStats..], "Armor: Magical Armor" + Environment.NewLine);
    }

    [TestMethod]
    public void CombatStats_ShowNoArmor_WithoutBuying()
    {
        Messages messages = English();
        var die = new FixedDie(5, 6, 7, 15, 3, 12, 9, 9, 20, 20);

        string output = PlayGame(CreateCharacterInput + "n\na\nr\n", die);

        int combatStats = output.IndexOf(messages.GetMessage("combat_stats_header"), StringComparison.Ordinal);
        StringAssert.Contains(output[combatStats..], "Armor: none" + Environment.NewLine);
    }

    [TestMethod]
    public void ExitPath_EndsGame()
    {
        Messages messages = English();

        string output = PlayGame(CreateCharacterInput + "e\n");

        StringAssert.Contains(output, messages.GetMessage("adventure_ends"));
    }
}
