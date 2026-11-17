# 03: Drink a Healing Potion during combat

**What to build:** The combat prompt gains `p` / `potion`. With a potion and below Maximum HP, drinking rolls d8 + 2, raises HP up to Maximum HP (never above), lowers the count by one, and reports the HP gained and the new HP. Drinking uses the player's turn: the player doesn't attack, the dragon attacks, then the prompt returns. With 0 potions ("You have no Healing Potions.") or at full HP ("You are already at full health."), the drink is refused: no potion is used, no turn passes, and the prompt is shown again. All text in four languages (spec Q3, Q5, Q12, Q13; user stories 10, 11, 12).

**Blocked by:** 01 (Maximum Health Points and the potion count)

**Status:** ready-for-agent

- [ ] The combat prompt lists the potion option.
- [ ] `p` heals d8 + 2, lowers the count, and the dragon attacks next, with no player attack that round (fixed dice).
- [ ] The heal stops at Maximum HP.
- [ ] `p` with 0 potions shows the no-potions message, uses no turn, and prompts again.
- [ ] `p` at full HP shows the full-health message, uses no potion and no turn, and prompts again.
- [ ] `a` and `r` behave as before (regression).
- [ ] New message keys exist in all four languages (the key-parity test passes).
