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

    [TestMethod]
    public void SouthPath_ShowsNarrativeAndEndsGame()
    {
        Messages messages = English();

        string output = PlayGame(CreateCharacterInput + "s\n");

        StringAssert.Contains(output, messages.GetMessage("south_path_narrative"));
        Assert.AreEqual(1, CountOccurrences(output, messages.GetMessage("path_prompt_full")),
            "The path question should not be asked again after taking the south path.");
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
    public void ExitPath_EndsGame()
    {
        Messages messages = English();

        string output = PlayGame(CreateCharacterInput + "e\n");

        StringAssert.Contains(output, messages.GetMessage("adventure_ends"));
    }
}
