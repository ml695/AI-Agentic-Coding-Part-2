[TestClass]
public class PlayerTests
{
    [TestMethod]
    public void NewPlayer_ShouldHaveDefaultValues()
    {
        Player player = new Player();

        Assert.AreEqual("", player.Name);
        Assert.AreEqual("", player.Race);
        Assert.AreEqual("", player.Occupation);

        Assert.AreEqual(0, player.Strength);
        Assert.AreEqual(0, player.Agility);
        Assert.AreEqual(0, player.HealthPoints);

        Assert.IsNull(player.Weapon);
    }

    // Creates a character from scripted console input: race, name, occupation, four rolls, proceed.
    private static Player CreateCharacter(string race, Die die)
    {
        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        try
        {
            Console.SetIn(new StringReader($"{race}\nBob\nfighter\nroll\nroll\nroll\nroll\n2\n"));
            Console.SetOut(new StringWriter());

            var messages = new Messages();
            messages.SetCurrentLanguage("English");

            var player = new Player(die);
            player.CreateCharacter(messages);
            return player;
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [TestMethod]
    [DoNotParallelize]
    public void Halfling_WhoseHealthRollFallsBelowOne_StartsWithOneHealthPoint()
    {
        // Strength 10, modifier 4 (subtracted for a Halfling), agility 10, agility bonus 2,
        // health roll 1 (1 - 4 = -3), gold 5.
        Player player = CreateCharacter("halfling", new FixedDie(10, 4, 10, 2, 1, 5));

        Assert.AreEqual(1, player.HealthPoints);
    }

    [TestMethod]
    [DoNotParallelize]
    public void Halfling_WhoseHealthRollStaysAboveOne_KeepsTheRolledHealth()
    {
        // Health roll 15 minus modifier 4 = 11.
        Player player = CreateCharacter("halfling", new FixedDie(10, 4, 10, 2, 15, 5));

        Assert.AreEqual(11, player.HealthPoints);
    }
}
