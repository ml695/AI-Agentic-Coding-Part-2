# 03: Merchant on the south path (browse and leave)

**What to build:** Taking the south path now leads to the **Merchant** instead of ending the game. The Merchant greets the player, describes the two items (**Magical Armor**: reduces each dragon hit by 3; **Magical Weapon**: +4 Maximum Damage to your current weapon) with their prices (80 and 120 Gold), shows the player's Gold, and offers a numbered menu: `1` armor, `2` weapon, `3` leave. Leaving returns the player to the path choice, where they can go north to the dragon or exit. Buying (`1`/`2`) is delivered by tickets 04 and 05; until then those choices may simply be accepted as "not yet available" or treated per the implementer's judgement, as long as the game doesn't break. All text in four languages (spec A1, A6; user stories 3, 4, 5, 8, 11, 17, 18, 20, 21).

**Blocked by:** 02 (Gold at character creation)

**Status:** ready-for-agent

- [x] Going south shows the Merchant greeting, both items with their effects and prices, and the player's Gold.
- [x] Choosing `3` buys nothing, leaves Gold unchanged, and returns to the path choice.
- [x] Invalid input at the Merchant shows an invalid-choice message and asks again.
- [x] If console input runs out during the visit, it is treated as leaving and the game ends without looping (5-second timeout test pattern).
- [x] Going south, leaving, then north reaches the dragon encounter.
- [x] Going south, leaving, then exit ends the game normally.
- [x] Going north without visiting the Merchant works exactly as before.
- [x] The existing "south path ends the game" test is changed to assert the return to the path choice.
- [x] New message keys exist in all four languages (the key-parity test passes).
- [x] **Merchant**, **Magical Armor** and **Magical Weapon** are defined in `GLOSSARY.md`.

## Comments

- Implemented. `Merchant.cs` runs one visit; choosing `1` or `2` shows a "not ready yet"
  message and asks again until tickets 04/05 add buying. The unused `south_path_narrative`
  key was removed. End of input at the path prompt now counts as exit (needed so input
  running out at the Merchant ends the game).
