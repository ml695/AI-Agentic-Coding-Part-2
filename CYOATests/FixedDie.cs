// CYOATests/FixedDie.cs
// A Die that returns scripted rolls in order, so tests get repeatable outcomes.
public class FixedDie : Die
{
    private readonly Queue<int> _rolls;

    public FixedDie(params int[] rolls)
    {
        _rolls = new Queue<int>(rolls);
    }

    public override int Roll(int sides)
    {
        if (_rolls.Count == 0)
            throw new InvalidOperationException($"FixedDie ran out of scripted rolls (asked for a d{sides}).");

        int roll = _rolls.Dequeue();
        if (roll < 1 || roll > sides)
            throw new InvalidOperationException($"FixedDie was scripted to roll {roll} on a d{sides}, which is impossible.");

        return roll;
    }
}
