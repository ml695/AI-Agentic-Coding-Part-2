using System;

public class Die
{
    private readonly Random _random = new Random();

    // Virtual so tests can supply scripted rolls.
    public virtual int Roll(int sides)
    {
        return _random.Next(1, sides + 1);
    }
}