# 04: Find a Healing Potion on arriving at a path

**What to build:** On arriving at the south path (before the Merchant greeting) and on arriving at the north path (before the dragon appears), the game rolls a d20 with its die. On 16 or higher the player finds one **Healing Potion**: the count goes up by one and a find message is shown. A miss shows nothing. Each path gives at most one potion per game: once a path has given one, arriving there no longer rolls. The path choice menu has no roll. Every existing scripted dice sequence in `GamePathTests.cs` gets a miss roll inserted where the arrival roll happens. There's no test-only switch to turn finding off. All text in four languages (spec Q4, Q8, Q9, Q10; user stories 3, 4, 5, 6, 16).

**Blocked by:** 01 (Maximum Health Points and the potion count)

**Status:** ready-for-agent

- [ ] Arriving south with a find roll of 16+ shows the find message before the Merchant greeting and adds one potion.
- [ ] Arriving north with a find roll of 16+ shows the find message before the dragon appears and adds one potion.
- [ ] A roll below 16 shows no find message and leaves the count unchanged.
- [ ] After the south path has given a potion, going south again does not roll (the script has no roll for it, so `FixedDie` doesn't run out).
- [ ] Existing `GamePathTests` dice scripts are updated with miss rolls and pass.
- [ ] New message keys exist in all four languages (the key-parity test passes).
