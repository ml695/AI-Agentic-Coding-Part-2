# 05: Drink Healing Potions after a win or a retreat

**What to build:** After a **win** (before the victory message) or a **retreat during combat** (after the combat's retreat line), if the player holds a potion and is below Maximum HP, they are asked `Drink a Healing Potion? (y/n)`, with their current HP and potion count. `y` drinks one, healing the same way as in combat. The prompt repeats until the player answers `n`, runs out of potions or reaches Maximum HP. End of input counts as `n`. Then the game ends as it does now. A defeat never gets the offer, because potions can't revive. Neither does a retreat chosen at the dragon-encounter prompt before combat, because no HP has been lost yet. All text in four languages (spec Q14, Q15; user stories 13, 14).

**Blocked by:** 03 (Drink a Healing Potion during combat)

**Status:** ready-for-agent

- [ ] After a win with a potion and below Maximum HP, the prompt appears before the victory message. `y` heals and the prompt repeats. `n` ends the game.
- [ ] After a retreat during combat with a potion and below Maximum HP, the prompt appears.
- [ ] The prompt stops repeating when potions run out or HP reaches Maximum HP.
- [ ] No prompt after a defeat, with 0 potions, or at full HP.
- [ ] Invalid input at the prompt is rejected and asked again.
- [ ] Input running out at the prompt ends the game with no infinite loop (5-second timeout pattern).
- [ ] New message keys exist in all four languages (the key-parity test passes).
