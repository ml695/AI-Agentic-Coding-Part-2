# 04: Buy Magical Armor

**What to build:** At the Merchant, choosing `1` buys **Magical Armor** for 80 Gold if the player can afford it. A purchase ends the visit and returns the player to the path choice (one item per visit; confirmed with the product owner, so no "already bought" message is needed). If the player can't afford it, they see a clear message, their Gold is unchanged, and they are asked again. Once owned, the armor shows in the character stats and in the combat stats before the fight, and each dragon hit that lands and isn't blocked deals 3 less damage, never below 0, with a message reporting how much the armor absorbed. The Agility block roll is unaffected. All text in four languages (spec A3, A5, A6; user stories 6, 9, 10, 12, 13, 14, 15).

**Blocked by:** 03 (Merchant on the south path)

**Status:** ready-for-agent

- [x] Buying armor with enough Gold deducts 80 Gold and returns to the path choice.
- [x] Trying to buy armor without enough Gold shows the can't-afford message, leaves Gold unchanged, and asks again.
- [x] The character stats show the armor ("none" before buying, the armor's name after).
- [x] The combat stats shown before the fight include the armor.
- [x] With armor, an unblocked dragon hit deals its damage minus 3, never below 0, and the absorbed-damage message appears (tested through the dragon's attack with fixed dice).
- [x] Without armor, dragon damage is unchanged (regression test).
- [x] New message keys and armor display names exist in all four languages (the key-parity test passes).
