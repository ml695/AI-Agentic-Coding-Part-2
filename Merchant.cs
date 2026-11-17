/// <summary>
/// The Merchant on the south path. One call to Visit runs one visit: the greeting, the two
/// offers with prices, the player's Gold and a numbered menu (1 armor, 2 weapon, 3 leave).
/// </summary>
public class Merchant
{
    public const int ArmorPrice = 80;
    public const int ArmorProtection = 3;
    public const int WeaponPrice = 120;
    public const int WeaponDamageBonus = 4;

    public void Visit(Player player, Messages messages)
    {
        Console.WriteLine();
        Console.WriteLine(messages.GetMessage("merchant_greeting"));

        while (true)
        {
            Console.WriteLine(string.Format(messages.GetMessage("merchant_armor_offer"), ArmorPrice, ArmorProtection));
            Console.WriteLine(string.Format(messages.GetMessage("merchant_weapon_offer"), WeaponPrice, WeaponDamageBonus));
            Console.WriteLine(messages.GetMessage("merchant_leave_option"));
            Console.WriteLine(string.Format(messages.GetMessage("merchant_your_gold"), player.Gold));
            Console.Write(messages.GetMessage("enter_choice"));
            string? choice = Console.ReadLine()?.Trim();

            // End of input counts as leaving, so the game can't loop forever.
            if (choice == null || choice == "3")
            {
                Console.WriteLine(messages.GetMessage("merchant_farewell"));
                return;
            }

            if (choice == "1" || choice == "2")
            {
                // Buying arrives in tickets 04 (armor) and 05 (weapon).
                Console.WriteLine(messages.GetMessage("merchant_not_available"));
            }
            else
            {
                Console.WriteLine(messages.GetMessage("merchant_invalid"));
            }
        }
    }
}
