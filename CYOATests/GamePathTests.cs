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

    // language, main menu, race, name, occupation, three rolls, "proceed on an adventure"
    private const string CreateCharacterInput = "1\n1\nhuman\nBob\nfighter\nroll\nroll\nroll\n2\n";

    private static string PlayGame(string input)
    {
        Console.SetIn(new StringReader(input));
        var output = new StringWriter();
        Console.SetOut(output);

        // If the game asks for more input than the script provides, it would wait forever;
        // the timeout turns that into a test failure instead of a hung test run.
        var game = Task.Run(() => new Game().StartGame());
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
    public void ExitPath_EndsGame()
    {
        Messages messages = English();

        string output = PlayGame(CreateCharacterInput + "e\n");

        StringAssert.Contains(output, messages.GetMessage("adventure_ends"));
    }
}
